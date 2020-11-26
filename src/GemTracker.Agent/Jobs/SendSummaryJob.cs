using GemTracker.Shared.Domain;
using GemTracker.Shared.Domain.DTOs;
using GemTracker.Shared.Domain.Enums;
using GemTracker.Shared.Domain.Statics;
using GemTracker.Shared.Extensions;
using GemTracker.Shared.Services;
using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GemTracker.Agent.Jobs
{
    [DisallowConcurrentExecution]
    public class SendSummaryJob : IJob
    {
        private static readonly Logger Logger = LogManager.GetLogger("GEM");

        private readonly IConfigurationService _configurationService;
        private readonly ITwitterService _twitterService;
        private readonly IFileService _fileService;
        public SendSummaryJob(
            IConfigurationService configurationService,
            ITwitterService twitterService,
            IFileService fileService
            )
        {
            _configurationService = configurationService;
            _twitterService = twitterService;
            _fileService = fileService;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var jobConfigFileName = context.JobDetail.JobDataMap["FileName"] as string;
                var storagePath = context.JobDetail.JobDataMap["StoragePath"] as string;
                var interval = context.JobDetail.JobDataMap["Interval"] as int?;

                var cfg = await _configurationService.GetJobConfigAsync(jobConfigFileName);

                var added = await _fileService.GetAsync<List<Gem>>(PathTo.Added(DexType.UNISWAP, storagePath));
                var deleted = await _fileService.GetAsync<List<Gem>>(PathTo.Deleted(DexType.UNISWAP, storagePath));

                if (added.AnyAndNotNull())
                {
                    Logger.Info($"V2|SUMMARY|ADDED|{added.Count}");

                    var newestAdded = added
                        .Where(
                            g =>
                            g.Recently == TokenAction.ADDED &&
                            (DateTime.UtcNow - g.DateTime).TotalMinutes < interval)
                        .OrderByDescending(g => g.DateTime)
                        .ToList();

                    Logger.Info($"V2|SUMMARY|ADDED|NEWEST|{newestAdded.Count}");

                    if (newestAdded.AnyAndNotNull())
                    {
                        var msg = Msg.ForTwitterSummary(newestAdded, TokenAction.ADDED, interval.Value);

                        var sent = await _twitterService.SendMessageAsync(msg);

                        if (sent.Success)
                        {
                            Logger.Info($"V2|SUMMARY|ADDED|NEWEST|SENT");
                        }
                        else
                            Logger.Error($"{sent.Message}");
                    }
                }

                if (deleted.AnyAndNotNull())
                {
                    Logger.Info($"V2|SUMMARY|DELETED|NEWEST|{deleted.Count}");

                    var newestDeleted = deleted
                        .Where(
                            g =>
                            g.Recently == TokenAction.DELETED &&
                            (DateTime.Now - g.DateTime).TotalMinutes < interval)
                        .OrderByDescending(g => g.DateTime)
                        .ToList();

                    Logger.Info($"V2|SUMMARY|DELETED|NEWEST|{newestDeleted.Count}");

                    if (newestDeleted.AnyAndNotNull())
                    {
                        var msg = Msg.ForTwitterSummary(newestDeleted, TokenAction.DELETED, interval.Value);

                        var sent = await _twitterService.SendMessageAsync(msg);

                        if (sent.Success)
                        {
                            Logger.Info($"V2|SUMMARY|DELETED|NEWEST|SENT");
                        }
                        else
                            Logger.Error($"{sent.Message}");
                    }
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