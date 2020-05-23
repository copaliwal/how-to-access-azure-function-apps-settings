using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(how_to_access_azure_function_apps_settings.Startup))]
namespace how_to_access_azure_function_apps_settings
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<Employee>()
               .Configure<IConfiguration>((settings, configuration) =>
               {
                   configuration.GetSection("Employee").Bind(settings);
               });
        }
    }
}
