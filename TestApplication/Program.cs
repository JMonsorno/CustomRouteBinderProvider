using PeterKottas.DotNetCore.WindowsService;

namespace TestApplication
{
    class Program
    {
        static int Main()
        {
            var resultCode = ServiceRunner<TestApplicationService>.Run(config =>
                config.Service(s =>
                {
                    s.ServiceFactory((arguments, controller) => new TestApplicationService());
                    s.OnStart((service, arguments) => service.Start());
                    s.OnStop(service => service.Stop());
                }));

            return resultCode;
        }
    }
}
