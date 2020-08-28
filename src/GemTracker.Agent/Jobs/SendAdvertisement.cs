using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GemTracker.Agent.Jobs
{
    [DisallowConcurrentExecution]
    public class SendAdvertisement : IJob
    {
        private static readonly Logger Logger = LogManager.GetLogger("GEM");
        private readonly IConfigurationService _configurationService;
        private readonly IFileService _fileService;
        private readonly ITelegramService _telegramService;
        public SendAdvertisement(
            IConfigurationService configurationService,
            IFileService fileService,
            ITelegramService telegramService)
        {
            _configurationService = configurationService;
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
                var storageAdv = Path.Combine(storagePath, $"advertisements.json");

                var listOfAdv = await _fileService.GetAsync<IEnumerable<Adv>>(storageAdv);

                if (listOfAdv.AnyAndNotNull())
                {
                    var storedList = listOfAdv.ToList();

                    var toSend = listOfAdv.FirstOrDefault(a => !a.IsPublished);

                    if (!(toSend is null))
                    {
                        var response = await _telegramService.SendMessageAsync(toSend.Content);

                        if (response.Success)
                        {
                            var sent = new Adv
                            {
                                IsPublished = true,
                                Content = toSend.Content
                            };

                            storedList.Remove(toSend);
                            storedList.Add(sent);

                            await _fileService.SetAsync(storageAdv, storedList);

                            Logger.Info($"Job: {cfg.JobConfig.Label} - Message sent with content: {toSend.Content}. Storage updated.");
                        }
                        else
                            Logger.Error($"Job: {cfg.JobConfig.Label} - {response.Message}");
                    }
                    else
                        Logger.Info($"Job: {cfg.JobConfig.Label} - Nothing to send");
                }
                else
                    Logger.Info($"Job: {cfg.JobConfig.Label} - Empty list");

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