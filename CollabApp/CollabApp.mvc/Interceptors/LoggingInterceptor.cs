
using Castle.DynamicProxy;

namespace CollabApp.mvc.Interceptors
{
    public class LoggingInterceptor : IInterceptor
    {
        private readonly ILogger<LoggingInterceptor> _logger;

        public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
        {
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            try
            {
                var method = $"{invocation.TargetType.FullName}.{invocation.Method.Name}";
                var arguments = string.Join(", ", invocation.Arguments.Select(a => a != null ? a.ToString() : "null"));

                _logger.LogInformation($"Calling method \'{method}\' with arguments \'{arguments}\'");

                invocation.Proceed();

                _logger.LogInformation($"Finished executing \'{method}\'. Returned \'{invocation.ReturnValue}\'");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Cought exception while executing \'{invocation.Method.Name}\': \'{ex.Message}\'");
                throw;
            }
        }
    }
}