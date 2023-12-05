
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Concurrent;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

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
                logBuilder.Append(timeStamp.ToString("o"));
                logBuilder.Append('\t');
                logBuilder.Append(GetLogLevelString(logLevel));
                logBuilder.Append("\t[");
                logBuilder.Append(logName);
                logBuilder.Append("]");
                logBuilder.Append("\t[");
                logBuilder.Append(eventId);
                logBuilder.Append("]\t");
                logBuilder.Append(message);

                if(null != exception)
                    logBuilder.AppendLine(exception.ToString());

                loggerProvider.WriteEntry(logBuilder.ToString());
            }
        }
    }
}