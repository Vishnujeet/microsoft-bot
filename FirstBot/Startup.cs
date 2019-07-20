// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EmptyBot v4.3.0

using FirstBot.Bots;
using FirstBot.Dialogs;
using FirstBot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FirstBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Create the credential provider to be used with the Bot Framework Adapter.
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();

            // Create the Bot Framework Adapter.
            services.AddSingleton<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();

            ConfigureState(services);

            ConfigDialogs(services);
            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, DialogBot<MainDialog>>();
        }

        public void ConfigureState(IServiceCollection services)
        {
           // services.AddSingleton<IStorage, MemoryStorage>();
           var storageAccount = "DefaultEndpointsProtocol=https;AccountName=botdemoidstorage;AccountKey=ZERM51zZ3RD9aQ/Sem4pJ5lOrwwBCvJT94WTP/rmLpj32thBXNrqrfIrq3hYLmQMKg/eSWnGPgwKwkE7aaM9jw==;EndpointSuffix=core.windows.net";
           var storageConntainer =
               "mystatedata";
           services.AddSingleton<IStorage>(new AzureBlobStorage(storageAccount,storageConntainer));
            services.AddSingleton<UserState>();
            services.AddSingleton<ConversationState>();
            services.AddSingleton<BotStateService>();
        }

        public void ConfigDialogs(IServiceCollection service)
        {
            service.AddSingleton<MainDialog>();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
