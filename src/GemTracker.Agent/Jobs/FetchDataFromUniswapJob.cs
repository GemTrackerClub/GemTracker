using GemTracker.Shared.Domain;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using NLog;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GemTracker.Agent.Jobs
{
    [DisallowConcurrentExecution]
    public class FetchDataFromUniswapJob : IJob
    {
        private static readonly Logger Logger = LogManager.GetLogger("GEM");
        private readonly IConfigurationService _configurationService;
        private readonly IUniswapService _uniswapService;
        private readonly IFileService _fileService;
        private readonly ITelegramService _telegramService;
        public FetchDataFromUniswapJob(
            IConfigurationService configurationService,
            IUniswapService uniswapService,
            IFileService fileService,
            ITelegramService telegramService)
        {
            _configurationService = configurationService;
            _uniswapService = uniswapService;
            _fileService = fileService;
            _telegramService = telegramService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var jobConfigFileName = context.JobDetail.JobDataMap["FileName"] as string;
                var storagePath = context.JobDetail.JobDataMap["StoragePath"] as string;

                var cfg = await _configurationService.GetJobConfigAsync(jobConfigFileName);

                var uniswap = new U(_uniswapService, _fileService);
                uniswap.SetPaths(storagePath);

                var latestAll = await uniswap.FetchFromUniswap();

                if (latestAll.Success)
                {
                    Logger.Info($"V2|GRAPH|LATEST|{latestAll.Tokens.Count()}");

                    var loadedAll = await uniswap.LoadFromStorage();

                    if (loadedAll.Success)
                    {
                        Logger.Info($"V2|GRAPH|LOADED ALL|{loadedAll.OldList.Count()}");
                        Logger.Info($"V2|GRAPH|LOADED ALL DELETED|{loadedAll.OldListDeleted.Count()}");
                        Logger.Info($"V2|GRAPH|LOADED ALL ADDED|{loadedAll.OldListAdded.Count()}");

                        var recentlyDeletedAll = uniswap.CheckDeleted(loadedAll.OldList, latestAll.Tokens);
                        var recentlyAddedAll = uniswap.CheckAdded(loadedAll.OldList, latestAll.Tokens);

                        loadedAll.OldListDeleted.AddRange(recentlyDeletedAll);
                        loadedAll.OldListAdded.AddRange(recentlyAddedAll);

                        await _fileService.SetAsync(uniswap.StorageFilePathDeleted, loadedAll.OldListDeleted);
                        await _fileService.SetAsync(uniswap.StorageFilePathAdded, loadedAll.OldListAdded);

                        await _fileService.SetAsync(uniswap.StorageFilePath, latestAll.Tokens);

                        if (cfg.JobConfig.Notify)
                        {
                            Logger.Info($"V2|GRAPH|TELEGRAM|ON");

                            var telegramNotification = new T(_telegramService);

                            var notifiedAboutDeleted = await telegramNotification.Notify(recentlyDeletedAll);

                            if (notifiedAboutDeleted.Success)
                                Logger.Info($"V2|GRAPH|TELEGRAM|DELETED|SENT");
                            else
                                Logger.Warn($"V2|GRAPH|TELEGRAM|DELETED|{notifiedAboutDeleted.Message}");

                            var notifiedAboutAdded = await telegramNotification.Notify(recentlyAddedAll);

                            if (notifiedAboutAdded.Success)
                                Logger.Info($"V2|GRAPH|TELEGRAM|ADDED|SENT");
                            else
                                Logger.Warn($"V2|GRAPH|TELEGRAM|ADDED|{notifiedAboutAdded.Message}");
                        }
                        else
                            Logger.Info($"V2|GRAPH|TELEGRAM|OFF");
                    }
                    else
                        Logger.Error($"V2|GRAPH|{loadedAll.Message}");
                }
                else
                    Logger.Error($"V2|GRAPH|{latestAll.Message}");

                if (cfg.Success)
                    Logger.Info($"Job: {cfg.JobConfig.Label} - DONE");
            }
            catch (Exception e)
            {
                Logger.Fatal($"{e.GetFullMessage()}");
            }
        }
    }
}