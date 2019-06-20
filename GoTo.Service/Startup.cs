using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GoTo.Service.Repositories;
using GoTo.Service.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace GoTo.Service {
    internal class Startup {
        private readonly IConfiguration config;

        public Startup(IConfiguration config) {
            this.config = config;
        }

        public void ConfigureServices(IServiceCollection services) {
            services.AddOptions();

            // Register repositories
            services.AddSingleton<ITripOfferRepository, InMemoryTripOfferRepository>();
            services.AddSingleton<IDestinationRepository, InMemoryDestinationRepository>()
                .Configure<InMemoryDestinationRepository.Settings>(config.GetSection("geo"));
            services.AddSingleton<IUserRepository, InMemoryUserRepository>();

            // Register services
            services.AddSingleton<IPublicTransportTripProvider, OEBBPublicTransportTripProvider>()
                .Configure<OEBBProviderOptions>(config.GetSection("oebb"));
            services.AddSingleton<IPublicTransportTripProvider, GMapsPublicTransportTripProvider>();

            services.AddSingleton<ITripSearcher, ExactMatchTripSearcher>();

            services.AddMvc()
              .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info {
                    Title = "GoTo API",
                    Version = "v1",
                    Description = "The GoTo REST API allows for offering trips and searching for public transport trips."
                });

                var xmlDocFile = Path.Combine(AppContext.BaseDirectory, "GoTo.Service.xml");
                c.IncludeXmlComments(xmlDocFile);
            });

            services.AddCors(options => {
                options.AddDefaultPolicy(
                    builder => {
                        builder.WithOrigins("http://localhost:4200")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors();
            app.UseStaticFiles();

            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.DocumentTitle = "GoTo API";

                c.SwaggerEndpoint("/swagger/v1/swagger.json", "GoTo API V1");
            });

            // Handle client routes
            app.Run(async (context) => {
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));
            });
        }
    }
}
