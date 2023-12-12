using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Xunit;
using CollabApp.mvc.Logging;

namespace CollabApp.Tests.UnitTests.Logging
{
    public class FileLoggerTests
    {
        FileLoggerProvider loggerProvider = new FileLoggerProvider("test_log.txt");

        [Theory]
        [InlineData(LogLevel.Information, "Info message")]
        [InlineData(LogLevel.Error, "Error message")]
        [InlineData(LogLevel.Critical, "Critical message")]
        [InlineData(LogLevel.Trace, "Trace message")]
        [InlineData(LogLevel.Warning, "Warning message")]
        [InlineData(LogLevel.Debug, "Debug message")]
        [InlineData(LogLevel.None, "Other message")]
        public void LogMessageIsWrittenToFile(LogLevel logLevel, string message)
        {
            var logger = new FileLogger("TestCategory", loggerProvider);

            switch (logLevel)
            {
                case LogLevel.Information:
                    logger.LogInformation(message);
                    break;
                case LogLevel.Error:
                    logger.LogError(message);
                    break;
                case LogLevel.Critical:
                    logger.LogCritical(message);
                    break;
                case LogLevel.Trace:
                    logger.LogTrace(message);
                    break;
                case LogLevel.Warning:
                    logger.LogWarning(message);
                    break;
                case LogLevel.Debug:
                    logger.LogDebug(message);
                    break;
                default:
                    logger.Log(LogLevel.None, message);
                    break;
            }

            var logContent = File.ReadAllText(LogFilePath);
            Assert.Contains(message, logContent);
        }

        [Fact]
        public void LogMessageWithExceptionIsWrittenToFile()
        {
            var logger = new FileLogger("Exception", loggerProvider);

            try
            {
                throw new InvalidOperationException("Test exception");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception occured");
            }

            var logContent = File.ReadAllText(LogFilePath);
            Assert.Contains("Test exception", logContent);
        }
    }
}