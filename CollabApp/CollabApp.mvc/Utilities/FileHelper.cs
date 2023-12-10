namespace CollabApp.mvc.Utilities
{
    public class FileHelper
    {
        private static readonly HashSet<string> audioTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "MP3 Audio", "WAV Audio"
        };

        private static readonly HashSet<string> videoTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "MP4 Video", "AVI Video", "WMV Video", "MOV Video", "FLV Video", "MKV Video"
        };

        private static readonly HashSet<string> imageTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "JPEG file", "PNG file", "GIF file"
        };

        public static bool IsPdfFile(string fileType)
        {
            return fileType.Equals("PDF Document", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsAudioFile(string fileType)
        {
            return audioTypes.Contains(fileType);
        }

        public static bool IsVideoFile(string fileType)
        {
            return videoTypes.Contains(fileType);
        }
    }
}
