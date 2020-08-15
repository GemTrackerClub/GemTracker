using GemTracker.Shared.Domain;
using GemTracker.Shared.Domain.DTOs;
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

                var cgApi = UniswapApiVersion.V2;
                var cge = UniswapEndpoint.GRAPH;

                var uniswap = new U(_uniswapService, _fileService);
                uniswap.SetPaths(storagePath, cgApi, cge);

                var latestAll = await uniswap.FetchFromUniswap(cfg.JobConfig.ToFetch);

                if (latestAll.Success)
                {
                    Logger.Info($"{cgApi.GetDescription()}|{cge.GetDescription()}|LATEST|{latestAll.Tokens.Count()}");

                    var loadedAll = await uniswap.LoadFromStorage();

                    if (loadedAll.Success)
                    {
                        Logger.Info($"{cgApi.GetDescription()}|{cge.GetDescription()}|LOADED ALL|{loadedAll.OldList.Count()}");
                        Logger.Info($"{cgApi.GetDescription()}|{cge.GetDescription()}|LOADED ALL DELETED|{loadedAll.OldListDeleted.Count()}");
                        Logger.Info($"{cgApi.GetDescription()}|{cge.GetDescription()}|LOADED ALL ADDED|{loadedAll.OldListAdded.Count()}");

                        var recentlyDeletedAll = uniswap.CheckDeleted(loadedAll.OldList, latestAll.Tokens);
                        var recentlyAddedAll = uniswap.CheckAdded(loadedAll.OldList, latestAll.Tokens);

                        loadedAll.OldListDeleted.AddRange(recentlyDeletedAll);
                        loadedAll.OldListAdded.AddRange(recentlyAddedAll);

                        await _fileService.SetAsync(uniswap.StorageFilePathDeleted, loadedAll.OldListDeleted);
                        await _fileService.SetAsync(uniswap.StorageFilePathAdded, loadedAll.OldListAdded);

                        await _fileService.SetAsync(uniswap.StorageFilePath, latestAll.Tokens);

                        if (cfg.JobConfig.Notify)
                        {
                            Logger.Info($"{cgApi.GetDescription()}|{cge.GetDescription()}|TELEGRAM|ON");

                            var telegramNotification = new T(_telegramService);

                            var notifiedAboutDeleted = await telegramNotification.Notify(recentlyDeletedAll, cgApi, UniswapEndpoint.GRAPH);

                            if (notifiedAboutDeleted.Success)
                                Logger.Info($"{cgApi.GetDescription()}|{cge.GetDescription()}|TELEGRAM|DELETED|SENT");
                            else
                                Logger.Warn($"{cgApi.GetDescription()}|{cge.GetDescription()}|TELEGRAM|DELETED|{notifiedAboutDeleted.Message}");

                            var notifiedAboutAdded = await telegramNotification.Notify(recentlyAddedAll, cgApi, UniswapEndpoint.GRAPH);

                            if (notifiedAboutAdded.Success)
                                Logger.Info($"{cgApi.GetDescription()}|{cge.GetDescription()}|TELEGRAM|ADDED|SENT");
                            else
                                Logger.Warn($"{cgApi.GetDescription()}|{cge.GetDescription()}|TELEGRAM|ADDED|{notifiedAboutAdded.Message}");
                        }
                        else
                            Logger.Info($"{cgApi.GetDescription()}|{cge.GetDescription()}|TELEGRAM|OFF");
                    }
                    else
                        Logger.Error($"{cgApi.GetDescription()}|{cge.GetDescription()}|{loadedAll.Message}");
                }
                else
                    Logger.Error($"{cgApi.GetDescription()}|{cge.GetDescription()}|{latestAll.Message}");

                var availableApis = EnumExt.GetValues<UniswapApiVersion>();
                var cge1k = UniswapEndpoint.REST;

                foreach (var api in availableApis)
                {
                    var uniswap1k = new U(_uniswapService, _fileService);
                    uniswap1k.SetPaths(storagePath, api, cge1k);

                    var latest1k = await uniswap1k.FetchFromUniswap();

                    if (latest1k.Success)
                    {
                        Logger.Info($"{api.GetDescription()}|{cge1k.GetDescription()}|LATEST|{latest1k.Tokens.Count()}");

                        var loaded1k = await uniswap1k.LoadFromStorage();

                        if (loaded1k.Success)
                        {
                            Logger.Info($"{api.GetDescription()}|{cge1k.GetDescription()}|LOADED ALL|{loaded1k.OldList.Count()}");
                            Logger.Info($"{api.GetDescription()}|{cge1k.GetDescription()}|LOADED ALL DELETED|{loaded1k.OldListDeleted.Count()}");
                            Logger.Info($"{api.GetDescription()}|{cge1k.GetDescription()}|LOADED ALL ADDED|{loaded1k.OldListAdded.Count()}");

                            var recentlyDeletedAll = uniswap1k.CheckDeleted(loaded1k.OldList, latest1k.Tokens);
                            var recentlyAddedAll = uniswap1k.CheckAdded(loaded1k.OldList, latest1k.Tokens);

                            loaded1k.OldListDeleted.AddRange(recentlyDeletedAll);
                            loaded1k.OldListAdded.AddRange(recentlyAddedAll);

                            await _fileService.SetAsync(uniswap1k.StorageFilePathDeleted, loaded1k.OldListDeleted);
                            await _fileService.SetAsync(uniswap1k.StorageFilePathAdded, loaded1k.OldListAdded);

                            await _fileService.SetAsync(uniswap1k.StorageFilePath, latest1k.Tokens);

                            if (cfg.JobConfig.Notify)
                            {
                                Logger.Info($"{api.GetDescription()}|{cge1k.GetDescription()}|TELEGRAM|ON");

                                var telegramNotification = new T(_telegramService);

                                var notifiedAboutDeleted = await telegramNotification.Notify(recentlyDeletedAll, api, cge1k);

                                if (notifiedAboutDeleted.Success)
                                    Logger.Info($"{api.GetDescription()}|{cge1k.GetDescription()}|TELEGRAM|DELETED|SENT");
                                else
                                    Logger.Warn($"{api.GetDescription()}|{cge1k.GetDescription()}|TELEGRAM|DELETED|{notifiedAboutDeleted.Message}");

                                var notifiedAboutAdded = await telegramNotification.Notify(recentlyAddedAll, api, cge1k);

                                if (notifiedAboutAdded.Success)
                                    Logger.Info($"{api.GetDescription()}|{cge1k.GetDescription()}|TELEGRAM|ADDED|SENT");
                                else
                                    Logger.Warn($"{api.GetDescription()}|{cge1k.GetDescription()}|TELEGRAM|ADDED|{notifiedAboutAdded.Message}");
                            }
                            else
                                Logger.Info($"{api.GetDescription()}|{cge1k.GetDescription()}|TELEGRAM|OFF");
                        }
                        else
                            Logger.Error($"{api.GetDescription()} - {loaded1k.Message}");
                    }
                    else
                        Logger.Error($"{api.GetDescription()} - {latest1k.Message}");
                }

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