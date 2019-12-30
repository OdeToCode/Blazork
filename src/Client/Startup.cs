using Blazork.Client.Models;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Blazork.Client
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            if (services is null)
            {
                throw new System.ArgumentNullException(nameof(services));
            }

            services.AddSingleton<GameModel>();
            services.BuildServiceProvider();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
