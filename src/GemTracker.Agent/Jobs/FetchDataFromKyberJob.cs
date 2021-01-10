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
    public class FetchDataFromKyberJob : IJob
    {
        private static readonly Logger Logger = LogManager.GetLogger("GEM");
        private readonly IConfigurationService _configurationService;
        private readonly IFileService _fileService;

        private readonly IDexchange<KyberToken, Gem> _dexchange;
        private readonly IFetchDataForKyber _fetchDataForKyber;
        private readonly INotificationFromKyber _notificationFromKyber;

        private static readonly DexType Type = DexType.KYBER;
        private readonly string Dex = Type.GetDescription().ToUpperInvariant();
        public FetchDataFromKyberJob(
            IConfigurationService configurationService,
            IFileService fileService,
            IDexchange<KyberToken, Gem> dexchange,
            IFetchDataForKyber fetchDataForKyber,
            INotificationFromKyber notificationFromKyber)
        {
            _configurationService = configurationService;
            _fileService = fileService;
            _dexchange = dexchange;
            _fetchDataForKyber = fetchDataForKyber;
            _notificationFromKyber = notificationFromKyber;
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

                        var latestNotActive = latestAll.ListResponse.Where(t => !t.Active).ToList();
                        var latestActive = latestAll.ListResponse.Where(t => t.Active).ToList();

                        var loadedNotActive = loadedAll.OldListDeleted.ToList();
                        var loadedActive = loadedAll.OldListAdded.ToList();

                        var recentlyAddedToActive =
                            DexTokenCompare.AddedTokens(loadedActive, latestActive, TokenActionType.KYBER_ADDED_TO_ACTIVE, DexType.KYBER);
                        var recentlyDeletedFromActive =
                            DexTokenCompare.DeletedTokens(loadedActive, latestActive, TokenActionType.KYBER_DELETED_FROM_ACTIVE, DexType.KYBER);

                        var recentlyAddedToNotActive =
                            DexTokenCompare.AddedTokens(loadedNotActive, latestNotActive, TokenActionType.KYBER_ADDED_TO_NOT_ACTIVE, DexType.KYBER);
                        var recentlyDeletedFromNotActive =
                            DexTokenCompare.DeletedTokens(loadedNotActive, latestNotActive, TokenActionType.KYBER_DELETED_FROM_NOT_ACTIVE, DexType.KYBER);

                        loadedAll.OldListDeleted.AddRange(recentlyAddedToNotActive);
                        foreach (var item in recentlyDeletedFromNotActive)
                        {
                            var toDelete = loadedAll.OldListDeleted.FirstOrDefault(t => t.Id == item.Id);
                            if (!(toDelete is null))
                            {
                                loadedAll.OldListDeleted.Remove(toDelete);
                            }
                        }
                        loadedAll.OldListAdded.AddRange(recentlyAddedToActive);
                        foreach (var item in recentlyDeletedFromActive)
                        {
                            var toDelete = loadedAll.OldListAdded.FirstOrDefault(t => t.Id == item.Id);
                            if (!(toDelete is null))
                            {
                                loadedAll.OldListAdded.Remove(toDelete);
                            }
                        }

                        await _fileService.SetAsync(PathTo.Deleted(Type, storagePath), loadedAll.OldListDeleted);
                        await _fileService.SetAsync(PathTo.Added(Type, storagePath), loadedAll.OldListAdded);

                        await _fileService.SetAsync(PathTo.All(Type, storagePath), latestAll.ListResponse);

                        if (cfg.JobConfig.Notify)
                        {
                            Logger.Info($"{Dex}|TELEGRAM|ON");

                            if (recentlyAddedToActive.AnyAndNotNull())
                            {
                                var filledForSend = await _fetchDataForKyber.FetchData(recentlyAddedToActive);
                                if (filledForSend.AnyAndNotNull())
                                {

                                }
                            }

                            if (recentlyDeletedFromActive.AnyAndNotNull())
                            {
                                var filledForSend = await _fetchDataForKyber.FetchData(recentlyDeletedFromActive);
                                if (filledForSend.AnyAndNotNull())
                                {

                                }
                            }

                            if (recentlyAddedToNotActive.AnyAndNotNull())
                            {
                                var filledForSend = await _fetchDataForKyber.FetchData(recentlyAddedToNotActive);
                                if (filledForSend.AnyAndNotNull())
                                {

                                }
                            }

                            if (recentlyDeletedFromNotActive.AnyAndNotNull())
                            {
                                var filledForSend = await _fetchDataForKyber.FetchData(recentlyDeletedFromNotActive);
                                if (filledForSend.AnyAndNotNull())
                                {

                                }
                            }

                            var notifiedAddedToActive = await _notificationFromKyber.SendAsync(recentlyAddedToActive);

                            if (notifiedAddedToActive.Success)
                                Logger.Info($"{Dex}|TELEGRAM|ADDED TO ACTIVE|SENT");
                            else
                                Logger.Warn($"{Dex}|TELEGRAM|ADDED TO ACTIVE|{notifiedAddedToActive.Message}");

                            var notifiedDeletedFromActive = await _notificationFromKyber.SendAsync(recentlyDeletedFromActive);

                            if (notifiedDeletedFromActive.Success)
                                Logger.Info($"{Dex}|TELEGRAM|DELETED FROM ACTIVE|SENT");
                            else
                                Logger.Warn($"{Dex}|TELEGRAM|DELETED FROM ACTIVE|{notifiedDeletedFromActive.Message}");

                            var notifiedAddedToNotActive = await _notificationFromKyber.SendAsync(recentlyAddedToNotActive);

                            if (notifiedAddedToNotActive.Success)
                                Logger.Info($"{Dex}|TELEGRAM|ADDED TO NOT ACTIVE|SENT");
                            else
                                Logger.Warn($"{Dex}|TELEGRAM|ADDED TO NOT ACTIVE|{notifiedAddedToNotActive.Message}");

                            var notifiedDeletedFromNotActive = await _notificationFromKyber.SendAsync(recentlyDeletedFromNotActive);

                            if (notifiedDeletedFromNotActive.Success)
                                Logger.Info($"{Dex}|TELEGRAM|DELETED FROM NOT ACTIVE|SENT");
                            else
                                Logger.Warn($"{Dex}|TELEGRAM|DELETED FROM NOT ACTIVE|{notifiedDeletedFromNotActive.Message}");
                        }
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