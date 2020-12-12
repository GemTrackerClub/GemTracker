using GemTracker.Agent.Jobs;
using GemTracker.Shared.Dexchanges;
using GemTracker.Shared.Dexchanges.Abstract;
using GemTracker.Shared.Domain.Configs;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Fetchers;
using GemTracker.Shared.Notifications.Abstract;
using GemTracker.Shared.Notifications.Responses;
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
            services.AddTransient<FetchDataFromKyberJob>();
            services.AddTransient<SendSummaryJob>();
            #endregion

            #region Services
            services.AddTransient<IConfigurationService, ConfigurationService>();
            services.AddTransient<IUniswapService, UniswapService>();
            services.AddTransient<IKyberService, KyberService>();
            services.AddTransient<IFileService, FileService>();

            services.AddTransient<ITelegramService>(
                s => new TelegramService(app.Telegram));
            services.AddTransient<ITwitterService>(
                s => new TwitterService(
                    app.TwitterConfig.ApiKey,
                    app.TwitterConfig.ApiSecret,
                    app.TwitterConfig.AccessToken,
                    app.TwitterConfig.AccessSecret
                    ));
            services.AddTransient<IEtherScanService>(
                s => new EtherScanService(app.EtherScan.ApiKey));
            services.AddTransient<IEthPlorerService>(
                s => new EthPlorerService(app.EthPlorer.ApiKey));
            #endregion

            #region Dexchanges
            services.AddTransient<IDexchange<KyberToken, Gem>, KyberDexchange>();
            services.AddTransient<IDexchange<Token, Gem>, UniDexchange>();
            #endregion

            #region Fetchers
            services.AddTransient<IFetchDataForKyber, FetchDataForKyber>();
            services.AddTransient<IFetchDataForUniswap, FetchDataForUniswap>();
            #endregion

            #region Notifications
            services.AddTransient<INotificationFromUniswap, TelegramNotificationFromUniswap>();
            services.AddTransient<INotificationFromKyber, TelegramNotificationFromKyber>();
            #endregion

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}