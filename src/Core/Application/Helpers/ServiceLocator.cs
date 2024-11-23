using Microsoft.Extensions.DependencyInjection;

namespace Application.Helpers
{
    public static class ServiceLocator
    {
        private static IServiceProvider _serviceProvider;

        public static void Configure(IServiceProvider serviceProvider)
        {
            _serviceProvider =
                serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public static T GetService<T>()
            where T : class => _serviceProvider.GetService<T>();
    }
}
