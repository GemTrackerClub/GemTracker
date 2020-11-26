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
    public class FetchDataFromKyberJob : IJob
    {
        private static readonly Logger Logger = LogManager.GetLogger("GEM");
        private readonly IConfigurationService _configurationService;
        private readonly IKyberService _kyberService;
        private readonly IFileService _fileService;
        private readonly ITelegramService _telegramService;
        private readonly IEtherScanService _etherScanService;
        private readonly IEthPlorerService _ethPlorerService;
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
                    Logger.Info($"V2|KYBER|LATEST|{latestAll.List.Data.Count()}");

                    var loadedAll = await kyber.LoadAllAsync();

                    if (loadedAll.Success)
                    {
                        Logger.Info($"V2|KYBER|LOADED ALL|{loadedAll.OldList.Count()}");
                        Logger.Info($"V2|KYBER|LOADED ALL DELETED|{loadedAll.OldListDeleted.Count()}");
                        Logger.Info($"V2|KYBER|LOADED ALL ADDED|{loadedAll.OldListAdded.Count()}");

                        var latestNotActive = latestAll.List.Data.Where(t => !t.Active).ToList();
                        var latestActive = latestAll.List.Data.Where(t => t.Active).ToList();

                        var loadedNotActive = loadedAll.OldListDeleted.ToList();
                        var loadedActive = loadedAll.OldListAdded.ToList();

                        var recentlyAddedToActive = kyber.CheckAdded(loadedActive, latestActive);
                        var recentlyDeletedFromActive = kyber.CheckDeleted(loadedActive, latestActive);

                        var recentlyAddedToNotActive = kyber.CheckAdded(loadedNotActive, latestNotActive);
                        var recentlyDeletedFromNotActive = kyber.CheckDeleted(loadedNotActive, latestNotActive);

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

                        await _fileService.SetAsync(kyber.StorageFilePath, latestAll.List.Data);

                        if (cfg.JobConfig.Notify)
                        {
                            Logger.Info($"V2|KYBER|TELEGRAM|ON");
                        }
                    }
                    else
                        Logger.Error($"V2|KYBER|{loadedAll.Message}");
                }
                else
                    Logger.Error($"V2|KYBER|{latestAll.Message}");

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