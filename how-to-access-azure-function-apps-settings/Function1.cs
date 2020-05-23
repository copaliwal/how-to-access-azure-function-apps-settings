using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace how_to_access_azure_function_apps_settings
{
    public class Function1
    {
        private readonly Employee _employee;

        public Function1(IOptions<Employee> employee)
        {
            _employee = employee.Value;
        }

        [FunctionName("Function1")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }


        [FunctionName("UsingEnvironmentVariable")]
        public async Task<IActionResult> UsingEnvironmentVariable(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string value = Environment.GetEnvironmentVariable("ConfigKey1");

            string responseMessage = string.IsNullOrEmpty(value)
                ? $"Failed!! The function app not able to find the value for provided Configuration key!!"
                : $"Success !! {value}";

            return new OkObjectResult(responseMessage);
        }


        [FunctionName("UsingConfigurationBuilder")]
        public async Task<IActionResult> UsingConfigurationBuilder(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ExecutionContext context, // <- You'll need this to add the local.settings.json file for local execution
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var config = new ConfigurationBuilder()
                                .SetBasePath(context.FunctionAppDirectory)
                                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                                .AddEnvironmentVariables()
                                .Build();

            var value = config["ConfigKey1"];

            // To read Connection string from ConnectionString Section of settings file
            //var connection = config.GetConnectionString("SqlConnectionString"); 

            string responseMessage = string.IsNullOrEmpty(value)
                ? $"Failed!! The function app not able to find the value for provided Configuration key!!"
                : $"Success !! {value}";

            return new OkObjectResult(responseMessage);
        }


        [FunctionName("UsingOptions")]
        public async Task<IActionResult> UsingOptions(
                [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
                ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            string responseMessage = _employee == null
                ? $"Failed!! The function app not able to find the value for provided Configuration key!!"
                : $"Success !! {JsonConvert.SerializeObject(_employee)}";

            return new OkObjectResult(responseMessage);
        }
    }
}
