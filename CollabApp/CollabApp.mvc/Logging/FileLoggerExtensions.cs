
namespace CollabApp.mvc.Logging
{
    public static class FileLoggerExtensions
    {

        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string filePath)
        {
            builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>(
                (srvPrv) =>
                {
                    return new FileLoggerProvider(filePath);
                }
            ));
            return builder;
        }

        public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string fileName, Action<FileLoggerOptions> configure)
        {
            builder.Services.Add(ServiceDescriptor.Singleton<ILoggerProvider, FileLoggerProvider>(
                serviceProvider =>
                {
                    var options = new FileLoggerOptions();
                    configure(options);
                    return new FileLoggerProvider(fileName, options);
                }
            ));

            return builder;
        }

        public static ILoggerFactory AddFile(this ILoggerFactory factory, string fileName, Action<FileLoggerOptions> configure) 
        {
            var fileLoggerOptions = new FileLoggerOptions();
            configure(fileLoggerOptions);
            factory.AddProvider(new FileLoggerProvider(fileName, fileLoggerOptions));
            
            return factory;
        }
    }
}