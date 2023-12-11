
namespace CollabApp.mvc.Logging
{
    public class FileLoggerOptions
    {
		public bool UseUtcTimestamp { get; set; }
        public LogLevel MinLevel { get; set; } = LogLevel.Trace;
    }
}