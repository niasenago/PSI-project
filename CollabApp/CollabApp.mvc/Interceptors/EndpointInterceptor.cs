using Castle.DynamicProxy;
using System;
using System.Collections.Generic;

namespace CollabApp.mvc.Interceptors
{
    public class EndpointCounterInterceptor : IInterceptor
    {
        private readonly ILogger<EndpointCounterInterceptor> _logger;
        private readonly Dictionary<string, int> endpointCount = new Dictionary<string, int>();

        public EndpointCounterInterceptor(ILogger<EndpointCounterInterceptor> logger)
        {
            _logger = logger;
        }

        public void Intercept(IInvocation invocation)
        {
            string endpointName = $"{invocation.Method.DeclaringType?.FullName}.{invocation.Method.Name}";

            if (!endpointCount.ContainsKey(endpointName))
                endpointCount[endpointName] = 1;
            else
                endpointCount[endpointName]++;

            _logger.LogInformation($"Endpoint {endpointName} has been called {endpointCount[endpointName]} times.");

            invocation.Proceed();
        }
    }
}