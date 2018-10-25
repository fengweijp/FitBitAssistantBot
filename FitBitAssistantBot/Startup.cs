namespace FitBitAssistantBot
{
    #region References
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Integration;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Configuration;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    #endregion

    public class Startup
    {
        private ILoggerFactory _loggerFactory;

        private bool _isProduction = false;

        public Startup(IHostingEnvironment environment)
        {
            _isProduction = environment.IsProduction();

            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration {get;}

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddBot<FitBitAssistantBot>(options =>
            {
                var secretKey = Configuration.GetSection("botFileSecret")?.Value;
                var botFilePath = Configuration.GetSection("botFilePath")?.Value;

                var botconfig = BotConfiguration.Load(botFilePath ?? @".\FitBitAssistantBot.bot", secretKey);
                services.AddSingleton(sp => botconfig ?? throw new InvalidOperationException($"The .bot config file could not be loaded. ({botconfig})"));

                var environment = _isProduction ? "production" : "development";

                var service = botconfig.Services.Where(x => x.Type == "endpoint" && x.Name == environment).FirstOrDefault();

                if(!(service is EndpointService endpointService))
                {
                    throw new InvalidOperationException($"The .bot file does not contain an endpoint with name '{environment}'.");

                }
                options.CredentialProvider = new SimpleCredentialProvider(endpointService.AppId, endpointService.AppPassword);

                ILogger logger = _loggerFactory.CreateLogger<FitBitAssistantBot>();

                options.OnTurnError = async (context , exception)  =>
                {
                    logger.LogError($"Exception Caught: {exception}");
                    await DialogHelpers.SenErrorMessageAsync(context);
                };

                IStorage dataStore = new MemoryStorage();

                var conversationState = new ConversationState(dataStore);
                options.State.Add(conversationState);

                var userState = new UserState(dataStore);
                options.State.Add(userState);

            });

            services.AddSingleton<FitBitAssistantBotAccessors>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
                if(options == null)
                {
                    throw new InvalidOperationException("BotFrameworkOptions must be configured prior to setting up the State Accessors");
                }
                var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();
                if (conversationState == null)
                {
                    throw new InvalidOperationException("ConversationState must be defined and added before adding conversation-scoped state accessors.");
                }

                var userState = options.State.OfType<UserState>().FirstOrDefault();
                if (userState == null)
                {
                    throw new InvalidOperationException("UserState must be defined and added before adding conversation-scoped state accessors.");
                }

                var accessors = new FitBitAssistantBotAccessors(conversationState, userState)
                {
                    CommandState = userState.CreateProperty<string>(FitBitAssistantBotAccessors.CommandStateName),
                    ConversationDialogState = userState.CreateProperty<DialogState>(FitBitAssistantBotAccessors.DialogStateName),

                };
                return accessors;

            });
        }

        public void Configure(IApplicationBuilder application, IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            application.UseDefaultFiles()
            .UseStaticFiles()
            .UseBotFramework();
        }

    }

}
