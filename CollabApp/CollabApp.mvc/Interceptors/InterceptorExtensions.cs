
using Castle.DynamicProxy;

namespace CollabApp.mvc.Interceptors
{
    public static class InterceptorExtensions
    {
        public static void Intercept<TInterceptor, TInterface, TImplementation>(this IServiceCollection services) where TInterceptor : class, IInterceptor where TInterface : class where TImplementation : class, TInterface
        {
            services.AddTransient<TInterface>(serviceProvider =>
            {
                var proxyGenerator = new ProxyGenerator();
                var service = serviceProvider.GetRequiredService<TImplementation>();
                var interceptor = serviceProvider.GetRequiredService<TInterceptor>();
                
                return proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(service, interceptor);
            });

            services.AddTransient<TImplementation>();
        }
    }
}
