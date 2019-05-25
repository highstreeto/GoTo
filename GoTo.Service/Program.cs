using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace GoTo.Service
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 1 && args[0] == "generate-swagger")
            {
                string swaggerJson = GenerateSwagger();
                System.IO.File.WriteAllText("swagger.json", swaggerJson);
            }
            else
            {
                CreateWebHostBuilder(args)
                    .Build()
                    .Run();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseWebRoot(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
                .UseStartup<Startup>();

        private static string GenerateSwagger()
        {
            var host = CreateWebHostBuilder(new string[] { }).Build();
            var sw = (ISwaggerProvider)host.Services.GetService(typeof(ISwaggerProvider));
            var doc = sw.GetSwagger("v1", null, "/");
            return JsonConvert.SerializeObject(
                doc,
                Formatting.Indented,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ContractResolver = new SwaggerContractResolver(new JsonSerializerSettings())
                });
        }

    }
}
