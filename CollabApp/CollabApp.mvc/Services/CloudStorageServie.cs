using CollabApp.mvc.Utilities;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Options;

namespace CollabApp.mvc.Services
{
    public interface ICloudStorageService
    {
        Task<string> GetSignedUrlAsync(string fileNameToRead, int timeOutInMinutes=30);
        Task<string> UploadFileAsync(IFormFile fileToUpload, string fileNameToSave);
        Task DeleteFileAsync(string fileNameToDelete);
        Task<string> GetFileType(string fileNameToRead);
    }
    public class CloudStorageService : ICloudStorageService
    {
        private readonly GCSConfigOptions _options;
        private readonly ILogger<CloudStorageService> _logger;
        private readonly GoogleCredential _googleCredential;

        public CloudStorageService(IOptions<GCSConfigOptions> options, ILogger<CloudStorageService> logger)
        {
            _options = options.Value;
            _logger = logger;

            try
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                if (environment == Environments.Production)
                {
                    // Store the json file in Secrets.
                    _googleCredential = GoogleCredential.FromJson(_options.GCPStorageAuthFile);
                }
                else
                {
                    _googleCredential = GoogleCredential.FromFile(_options.GCPStorageAuthFile);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                throw;
            }
        }
        public async Task DeleteFileAsync(string fileNameToDelete)
        {
            try
            {
                using (var storageClient = StorageClient.Create(_googleCredential))
                {
                    await storageClient.DeleteObjectAsync(_options.GoogleCloudStorageBucketName, fileNameToDelete);
                }
                _logger.LogInformation($"File {fileNameToDelete} deleted");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured while deleting file {fileNameToDelete}: {ex.Message}");
                throw;
            }
        }

        public async Task<string> GetSignedUrlAsync(string fileNameToRead, int timeOutInMinutes = 30)
        {
            try
            {
                var sac = _googleCredential.UnderlyingCredential as ServiceAccountCredential;
                var urlSigner = UrlSigner.FromServiceAccountCredential(sac);

                // provides limited permission and time to make a request: time here is mentioned for 30 minutes.
                var signedUrl = await urlSigner.SignAsync(_options.GoogleCloudStorageBucketName, fileNameToRead, TimeSpan.FromMinutes(timeOutInMinutes));
                _logger.LogInformation($"Signed url obtained for file {fileNameToRead}");
                return signedUrl.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occured while obtaining signed url for file {fileNameToRead}: {ex.Message}");
                throw;
            }
        }
        public async Task<string> GetFileType(string fileNameToRead)
        {
            try
            {
                // You can extract the file extension from the provided fileNameToRead.
                string fileExtension = Path.GetExtension(fileNameToRead);
                
                Dictionary<string, string> fileExtensionsToTypes = new Dictionary<string, string>
                {
                    { ".pdf", "PDF Document" },
                    { ".jpg", "JPEG Image" },
                    { ".png", "PNG Image" },
                    { ".doc", "Microsoft Word Document" },
                    { ".docx", "Microsoft Word Document" },
                    { ".xls", "Microsoft Excel Spreadsheet" },
                    { ".xlsx", "Microsoft Excel Spreadsheet" },
                    { ".ppt", "Microsoft PowerPoint Presentation" },
                    { ".pptx", "Microsoft PowerPoint Presentation" },
                    { ".txt", "Text File" },
                    { ".zip", "Zip Archive" },
                    { ".rar", "RAR Archive" },
                    { ".mp3", "MP3 Audio" },
                    { ".mp4", "MP4 Video" },
                };

                if (fileExtensionsToTypes.ContainsKey(fileExtension.ToLower()))
                {
                    return fileExtensionsToTypes[fileExtension.ToLower()];
                }
                else
                {
                    // Return a default value or handle unknown file types as needed.
                    return "Unknown File Type";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while determining the file type for {fileNameToRead}: {ex.Message}");
                throw;
            }
        }

        public async Task<string> UploadFileAsync(IFormFile fileToUpload, string fileNameToSave)
        {
            try
            {
                _logger.LogInformation($"Uploading: file {fileNameToSave} to storage {_options.GoogleCloudStorageBucketName}");
                using (var memoryStream = new MemoryStream())
                {
                    await fileToUpload.CopyToAsync(memoryStream);
                    // Create Storage Client from Google Credential
                    using (var storageClient = StorageClient.Create(_googleCredential))
                    {
                        // upload file stream 
                        var uploadedFile = await storageClient.UploadObjectAsync(_options.GoogleCloudStorageBucketName, fileNameToSave, fileToUpload.ContentType, memoryStream);
                        _logger.LogInformation($"Uploaded: file {fileNameToSave} to storage {_options.GoogleCloudStorageBucketName}");
                        return uploadedFile.MediaLink;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while uploading file {fileNameToSave}: {ex.Message}");
                throw;
            }
        }
    }
}