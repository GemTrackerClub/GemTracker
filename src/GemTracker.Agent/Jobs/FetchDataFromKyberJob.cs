using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
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
            }
            catch (Exception e)
            {
                Logger.Fatal($"{e.GetFullMessage()}");
            }
        }
    }
}