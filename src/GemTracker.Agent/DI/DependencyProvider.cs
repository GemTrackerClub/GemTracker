using GemTracker.Agent.Jobs;
using GemTracker.Shared.Domain.Configs;
using GemTracker.Shared.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;

namespace GemTracker.Agent.DI
{
    public class DependencyProvider
    {
        public static IServiceProvider Get(AgentApp app)
        {
            var services = new ServiceCollection();

            #region Logging
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageTemplates = true,
                    CaptureMessageProperties = true
                });
            });
            #endregion

            #region Jobs
            services.AddTransient<FetchDataFromUniswapJob>();
            #endregion

            #region Services
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddTransient<IUniswapService, UniswapService>();
            services.AddTransient<IFileService, FileService>();

            services.AddTransient<ITelegramService>(
                s => new TelegramService(
                    app.TelegramConfig.ApiKey,
                    app.TelegramConfig.ChatId));
            #endregion

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}