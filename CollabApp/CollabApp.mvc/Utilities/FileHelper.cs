namespace CollabApp.mvc.Utilities
{
    public static class FileHelper
    {
        public static bool IsPdfFile(this string fileType)
        {
            return fileType.Equals("PDF Document", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsAudioFile(this string fileType)
        {
            return fileType.Contains("Audio", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsVideoFile(this string fileType)
        {
            return fileType.Contains("Video", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsImageFile(this string fileType)
        {
            return fileType.Contains("Image", StringComparison.OrdinalIgnoreCase);
        }
    }   
}
