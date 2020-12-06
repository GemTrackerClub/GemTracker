using GemTracker.Shared.Domain;
using GemTracker.Shared.Domain.Enums;
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
    public class FetchDataFromKyberJob : IJob
    {
        private static readonly Logger Logger = LogManager.GetLogger("GEM");
        private readonly IConfigurationService _configurationService;
        private readonly IKyberService _kyberService;
        private readonly IFileService _fileService;
        private readonly ITelegramService _telegramService;
        private readonly IEtherScanService _etherScanService;
        private readonly IEthPlorerService _ethPlorerService;
        private readonly string Dex = DexType.KYBER.GetDescription().ToUpperInvariant();
        public FetchDataFromKyberJob(
            IConfigurationService configurationService,
            IKyberService kyberService,
            IFileService fileService,
            ITelegramService telegramService,
            IEtherScanService etherScanService,
            IEthPlorerService ethPlorerService)
        {
            _configurationService = configurationService;
            _kyberService = kyberService;
            _fileService = fileService;
            _telegramService = telegramService;
            _etherScanService = etherScanService;
            _ethPlorerService = ethPlorerService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var jobConfigFileName = context.JobDetail.JobDataMap["FileName"] as string;
                var storagePath = context.JobDetail.JobDataMap["StoragePath"] as string;

                var cfg = await _configurationService.GetJobConfigAsync(jobConfigFileName);

                var kyber = new Kyber(_kyberService, _fileService);
                kyber.SetPaths(storagePath);

                var latestAll = await kyber.FetchAllAsync();

                if (latestAll.Success)
                {
                    Logger.Info($"{Dex}|LATEST|{latestAll.ObjectResponse.Data.Count()}");

                    var loadedAll = await kyber.LoadAllAsync();

                    if (loadedAll.Success)
                    {
                        Logger.Info($"{Dex}|LOADED ALL|{loadedAll.OldList.Count()}");
                        Logger.Info($"{Dex}|LOADED ALL DELETED|{loadedAll.OldListDeleted.Count()}");
                        Logger.Info($"{Dex}|LOADED ALL ADDED|{loadedAll.OldListAdded.Count()}");

                        var latestNotActive = latestAll.ObjectResponse.Data.Where(t => !t.Active).ToList();
                        var latestActive = latestAll.ObjectResponse.Data.Where(t => t.Active).ToList();

                        var loadedNotActive = loadedAll.OldListDeleted.ToList();
                        var loadedActive = loadedAll.OldListAdded.ToList();

                        var recentlyAddedToActive = kyber.CheckAdded(loadedActive, latestActive, TokenActionType.KYBER_ADDED_TO_ACTIVE);
                        var recentlyDeletedFromActive = kyber.CheckDeleted(loadedActive, latestActive, TokenActionType.KYBER_DELETED_FROM_ACTIVE);

                        var recentlyAddedToNotActive = kyber.CheckAdded(loadedNotActive, latestNotActive, TokenActionType.KYBER_ADDED_TO_NOT_ACTIVE);
                        var recentlyDeletedFromNotActive = kyber.CheckDeleted(loadedNotActive, latestNotActive, TokenActionType.KYBER_DELETED_FROM_NOT_ACTIVE);

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

                        await _fileService.SetAsync(kyber.StorageFilePathDeleted, loadedAll.OldListDeleted);
                        await _fileService.SetAsync(kyber.StorageFilePathAdded, loadedAll.OldListAdded);

                        await _fileService.SetAsync(kyber.StorageFilePath, latestAll.ObjectResponse.Data);

                        if (cfg.JobConfig.Notify)
                        {
                            Logger.Info($"{Dex}|TELEGRAM|ON");

                            var telegramNotification = new KyberNtf(
                                _telegramService,
                                _etherScanService,
                                _ethPlorerService);

                            var notifiedAddedToActive = await telegramNotification.SendAsync(recentlyAddedToActive);

                            if (notifiedAddedToActive.Success)
                                Logger.Info($"{Dex}|TELEGRAM|ADDED TO ACTIVE|SENT");
                            else
                                Logger.Warn($"{Dex}|TELEGRAM|ADDED TO ACTIVE|{notifiedAddedToActive.Message}");

                            var notifiedDeletedFromActive = await telegramNotification.SendAsync(recentlyDeletedFromActive);

                            if (notifiedDeletedFromActive.Success)
                                Logger.Info($"{Dex}|TELEGRAM|DELETED FROM ACTIVE|SENT");
                            else
                                Logger.Warn($"{Dex}|TELEGRAM|DELETED FROM ACTIVE|{notifiedDeletedFromActive.Message}");

                            var notifiedAddedToNotActive = await telegramNotification.SendAsync(recentlyAddedToNotActive);

                            if (notifiedAddedToNotActive.Success)
                                Logger.Info($"{Dex}|TELEGRAM|ADDED TO NOT ACTIVE|SENT");
                            else
                                Logger.Warn($"{Dex}|TELEGRAM|ADDED TO NOT ACTIVE|{notifiedAddedToNotActive.Message}");

                            var notifiedDeletedFromNotActive = await telegramNotification.SendAsync(recentlyDeletedFromNotActive);

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