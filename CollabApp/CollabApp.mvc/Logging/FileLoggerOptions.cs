
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
    public class FileLoggerOptions
    {
		public bool UseUtcTimestamp { get; set; }
        public LogLevel MinLevel { get; set; } = LogLevel.Trace;
    }
}