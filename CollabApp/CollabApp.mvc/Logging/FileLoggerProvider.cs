
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
    public class FileLoggerProvider : ILoggerProvider 
    {
        private string LogFilePath;
        private readonly ConcurrentDictionary<string, FileLogger> loggers = new ConcurrentDictionary<string, FileLogger>();
        private readonly BlockingCollection<string> entryQueue = new BlockingCollection<string>(1024);
        private readonly Task processQueueTask;

        internal FileLoggerOptions Options { get; private set; }

        public LogLevel MinLevel 
        {
            get => Options.MinLevel; 
            set { Options.MinLevel = value; }
        }

        public bool UseUtcTimestamp 
        { 
            get => Options.UseUtcTimestamp; 
            set { Options.UseUtcTimestamp = value; } 
        }

		public FileLoggerProvider(string fileName) : this(fileName, new FileLoggerOptions()) 
        {
            
		}

        public FileLoggerProvider(string filePath, FileLoggerOptions options) 
        {
            Options = options;
            LogFilePath = filePath;

            processQueueTask = Task.Factory.StartNew(ProcessQueue, this, TaskCreationOptions.LongRunning);
        }

        public ILogger CreateLogger(string categoryName) 
        {
            return loggers.GetOrAdd(categoryName, new FileLogger(categoryName, this));
        }

        public void Dispose() 
        {
            entryQueue.CompleteAdding();
            processQueueTask.Wait(1500);

            loggers.Clear();
        }

        internal void WriteEntry(string message) 
        {
            if(!entryQueue.IsAddingCompleted) 
            {
                entryQueue.Add(message);
                return;
            }
        }

        private void ProcessQueue() 
        {
            var writeMessageFailed = false;
            foreach(var message in entryQueue.GetConsumingEnumerable()) 
            {
                try {
                    if(!writeMessageFailed)
                    {
                        using(StreamWriter outputFile = new StreamWriter(LogFilePath, true))
                        {
                            outputFile.WriteLine(message);
                        }
                    }
                } 
                catch (Exception ex) 
                {
                    entryQueue.CompleteAdding();
                    writeMessageFailed = true;
                }
            }
        }

        private static void ProcessQueue(object state) 
        {
            var fileLogger = (FileLoggerProvider)state;
            fileLogger.ProcessQueue();
        }
    }
}