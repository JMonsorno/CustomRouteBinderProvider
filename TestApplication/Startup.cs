using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace TestApplication
{
    internal class Startup : StartupBase
    {
        public override void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
