
using System.Text;

namespace CollabApp.mvc.Logging 
{
    public class FileLogger : ILogger 
    {
        private readonly string logName;
        private readonly FileLoggerProvider loggerProvider;

        public FileLogger(string logName, FileLoggerProvider loggerProvider) 
        {
            this.logName = logName;
            this.loggerProvider = loggerProvider;
        }

        public IDisposable BeginScope<TState>(TState state) 
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel) 
        {
            return logLevel >= loggerProvider.MinLevel;
        }

        private string GetLogLevelString(LogLevel logLevel) 
        {
            switch (logLevel) 
            {
                case LogLevel.Trace:
                    return "TRCE";
                case LogLevel.Debug:
                    return "DBUG";
                case LogLevel.Information:
                    return "INFO";
                case LogLevel.Warning:
                    return "WARN";
                case LogLevel.Error:
                    return "FAIL";
                case LogLevel.Critical:
                    return "CRIT";
            }

            return logLevel.ToString().ToUpper();
        }
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if(!IsEnabled(logLevel))
                return;

            if (null == formatter)
                throw new ArgumentNullException(nameof(formatter));

            string message = formatter(state, exception);

            var logBuilder = new StringBuilder();
            if(!string.IsNullOrEmpty(message)) 
            {
                DateTime timeStamp = loggerProvider.UseUtcTimestamp ? DateTime.UtcNow : DateTime.Now;
                logBuilder.Append(timeStamp.ToString("yyyy-MM-dd HH:mm:ss"));
                logBuilder.Append(' ');
                logBuilder.Append(GetLogLevelString(logLevel));
                logBuilder.Append(" [");
                logBuilder.Append(logName);
                logBuilder.Append(']');
                logBuilder.Append(" [");
                logBuilder.Append(eventId);
                logBuilder.Append("] ");
                logBuilder.Append(message);

                if(null != exception)
                    logBuilder.AppendLine(exception.ToString());

                loggerProvider.WriteEntry(logBuilder.ToString());
            }
        }
    }
}