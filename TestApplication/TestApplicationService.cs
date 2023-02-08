using CustomRouteBinderProviderLibrary;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using System;
using System.ComponentModel.Design;

namespace TestApplication
{
    internal class TestApplicationService : IMicroService
    {
        public TestApplicationService()
        {
            Container = new ServiceContainer();
        }

        private IWebHost WebHost { get; set; }
        private IServiceContainer Container { get; }
        
        public void Start()
        {
            IWebHostBuilder builder =
                Microsoft.AspNetCore.WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .ConfigureServices(services =>
                {
                    services.AddMvcCore(options =>
                    {
                        options.ModelBinderProviders.Insert(0, new CustomRouteBinderProvider());
                    });
                });

            WebHost = builder.Build();
            WebHost.Start();

            foreach (var address in WebHost.ServerFeatures.Get<IServerAddressesFeature>().Addresses)
            {
                Console.WriteLine($"Web API initialized at {address}");
            }
        }

        public void Stop()
        {
            WebHost?.Dispose();
        }
    }
}
