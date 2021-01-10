using GemTracker.Shared.Dexchanges.Abstract;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Domain.Enums;
using GemTracker.Shared.Domain.Statics;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Fetchers;
using GemTracker.Shared.Notifications.Abstract;
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
        private readonly IFileService _fileService;

        private readonly IDexchange<Token, Gem> _dexchange;
        private readonly IFetchDataForUniswap _fetchDataForUniswap;
        private readonly INotificationFromUniswap _notificationFromUniswap;

        private static readonly DexType Type = DexType.UNISWAP;
        private readonly string Dex = Type.GetDescription().ToUpperInvariant();
        public FetchDataFromUniswapJob(
            IConfigurationService configurationService,
            IFileService fileService,
            IDexchange<Token, Gem> dexchange,
            IFetchDataForUniswap fetchDataForUniswap,
            INotificationFromUniswap notificationFromUniswap)
        {
            _configurationService = configurationService;
            _fileService = fileService;
            _dexchange = dexchange;
            _fetchDataForUniswap = fetchDataForUniswap;
            _notificationFromUniswap = notificationFromUniswap;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var jobConfigFileName = context.JobDetail.JobDataMap["FileName"] as string;
                var storagePath = context.JobDetail.JobDataMap["StoragePath"] as string;

                var cfg = await _configurationService.GetJobConfigAsync(jobConfigFileName);

                var latestAll = await _dexchange.FetchAllAsync();

                if (latestAll.Success)
                {
                    Logger.Info($"{Dex}|LATEST|{latestAll.ListResponse.Count()}");

                    var loadedAll = await _dexchange.LoadAllAsync(storagePath);

                    if (loadedAll.Success)
                    {
                        Logger.Info($"{Dex}|LOADED ALL|{loadedAll.OldList.Count()}");
                        Logger.Info($"{Dex}|LOADED ALL DELETED|{loadedAll.OldListDeleted.Count()}");
                        Logger.Info($"{Dex}|LOADED ALL ADDED|{loadedAll.OldListAdded.Count()}");

                        var recentlyDeletedAll =
                            DexTokenCompare.DeletedTokens(loadedAll.OldList, latestAll.ListResponse, TokenActionType.DELETED, DexType.UNISWAP);
                        var recentlyAddedAll =
                            DexTokenCompare.AddedTokens(loadedAll.OldList, latestAll.ListResponse, TokenActionType.ADDED, DexType.UNISWAP);

                        loadedAll.OldListDeleted.AddRange(recentlyDeletedAll);
                        loadedAll.OldListAdded.AddRange(recentlyAddedAll);

                        await _fileService.SetAsync(PathTo.Deleted(Type, storagePath), loadedAll.OldListDeleted);
                        await _fileService.SetAsync(PathTo.Added(Type, storagePath), loadedAll.OldListAdded);

                        await _fileService.SetAsync(PathTo.All(Type, storagePath), latestAll.ListResponse);

                        if (cfg.JobConfig.Notify)
                        {
                            Logger.Info($"{Dex}|TELEGRAM|ON");

                            if (recentlyAddedAll.AnyAndNotNull())
                            {
                                var filledForSend = await _fetchDataForUniswap.FetchData(recentlyAddedAll);

                                if (filledForSend.AnyAndNotNull())
                                {

                                }
                            }

                            if (recentlyDeletedAll.AnyAndNotNull())
                            {
                                var filledForSend = await _fetchDataForUniswap.FetchData(recentlyDeletedAll);

                                if (filledForSend.AnyAndNotNull())
                                {

                                }
                            }

                            var notifiedAboutDeleted = await _notificationFromUniswap.SendAsync(recentlyDeletedAll);

                            if (notifiedAboutDeleted.Success)
                                Logger.Info($"{Dex}|TELEGRAM|DELETED|SENT");
                            else
                                Logger.Warn($"{Dex}|TELEGRAM|DELETED|{notifiedAboutDeleted.Message}");

                            var notifiedAboutAdded = await _notificationFromUniswap.SendAsync(recentlyAddedAll);

                            if (notifiedAboutAdded.Success)
                                Logger.Info($"{Dex}|TELEGRAM|ADDED|SENT");
                            else
                                Logger.Warn($"{Dex}|TELEGRAM|ADDED|{notifiedAboutAdded.Message}");
                        }
                        else
                            Logger.Info($"{Dex}|TELEGRAM|OFF");
                    }
                    else
                        Logger.Error($"{Dex}|{loadedAll.Message}");
                }
                else
                    Logger.Error($"{Dex}|{latestAll.Message}");

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