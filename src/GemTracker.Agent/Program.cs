﻿using GemTracker.Agent.DI;
using GemTracker.Agent.Factories;
using GemTracker.Agent.Jobs;
using GemTracker.Shared.Domain.Configs;
using GemTracker.Shared.Extensions;
using Microsoft.Extensions.Configuration;
using NLog;
using Quartz;
using Quartz.Impl;
using System;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;

namespace GemTracker.Agent
{
    class Program
    {
        private static readonly string appId = "gem-runtime";

        private static ISchedulerFactory _schedulerFactory;
        private static IScheduler _scheduler;

        private static readonly Logger Logger = LogManager.GetLogger("GEM");

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static async Task<int> Main()
        {
            var cancellationTokenSource = new CancellationTokenSource();

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);

            AppDomain.CurrentDomain.ProcessExit += (s, e) => cancellationTokenSource.Cancel();
            Console.CancelKeyPress += (s, e) => cancellationTokenSource.Cancel();

            LogManager.Configuration.Variables["fileName"] = $"{appId}-{DateTime.UtcNow:ddMMyyyy}.log";
            LogManager.Configuration.Variables["archiveFileName"] = $"{appId}-{DateTime.UtcNow:ddMMyyyy}.log";

            var cfgBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile($"appsettings.{appId}.json");

            var configuration = cfgBuilder.Build();

            var app = configuration.Get<AgentApp>();

            try
            {
                var servicesProvider = DependencyProvider.Get(app);

                var jobFactory = new JobFactory(servicesProvider);

                _schedulerFactory = new StdSchedulerFactory();

                _scheduler = await _schedulerFactory.GetScheduler();

                _scheduler.JobFactory = jobFactory;

                await _scheduler.Start();

                #region Fetch Data Uniswap
                var fdfu = app.Jobs.FirstOrDefault(j => j.Name == "j-fetch-data-from-uniswap");

                var fdfuJob = JobBuilder.Create<FetchDataFromUniswapJob>()
                    .WithIdentity($"{fdfu.Name}Job")
                    .Build();

                fdfuJob.JobDataMap["FileName"] = fdfu.Name;
                fdfuJob.JobDataMap["StoragePath"] = app.StoragePath;

                var fdfuBuilder = TriggerBuilder.Create()
                    .WithIdentity($"{fdfu.Name}Trigger")
                    .StartNow();

                fdfuBuilder.WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(fdfu.IntervalInMinutes)
                        .RepeatForever());

                var fdfuTrigger = fdfuBuilder.Build();
                #endregion

                #region Fetch Data Kyber
                var fdfk = app.Jobs.FirstOrDefault(j => j.Name == "j-fetch-data-from-kyber");

                var fdfkJob = JobBuilder.Create<FetchDataFromKyberJob>()
                    .WithIdentity($"{fdfk.Name}Job")
                    .Build();

                fdfkJob.JobDataMap["FileName"] = fdfk.Name;
                fdfkJob.JobDataMap["StoragePath"] = app.StoragePath;

                var fdfkBuilder = TriggerBuilder.Create()
                    .WithIdentity($"{fdfk.Name}Trigger")
                    .StartNow();

                fdfkBuilder.WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(fdfk.IntervalInMinutes)
                        .RepeatForever());

                var fdfkTrigger = fdfkBuilder.Build();
                #endregion

                #region Send Summary
                var ss = app.Jobs.FirstOrDefault(j => j.Name == "j-send-summary");

                var ssJob = JobBuilder.Create<SendSummaryJob>()
                    .WithIdentity($"{ss.Name}Job")
                    .Build();

                ssJob.JobDataMap["FileName"] = ss.Name;
                ssJob.JobDataMap["StoragePath"] = app.StoragePath;
                ssJob.JobDataMap["Interval"] = ss.IntervalInMinutes;

                var ssBuilder = TriggerBuilder.Create()
                    .WithIdentity($"{ss.Name}Trigger")
                    .StartNow();

                ssBuilder.WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(ss.IntervalInMinutes)
                        .RepeatForever());

                var ssTrigger = ssBuilder.Build();
                #endregion

                if (fdfu.IsActive)
                    await _scheduler.ScheduleJob(fdfuJob, fdfuTrigger);

                if (fdfk.IsActive)
                    await _scheduler.ScheduleJob(fdfkJob, fdfkTrigger);

                if (ss.IsActive)
                    await _scheduler.ScheduleJob(ssJob, ssTrigger);

                await Task.Delay(TimeSpan.FromSeconds(30));

                await Task.Delay(-1, cancellationTokenSource.Token).ContinueWith(t => { });
            }
            catch (SchedulerException ex)
            {
                Logger.Fatal($"{ex.GetFullMessage()}");
            }

            LogManager.Shutdown();

            return 0;
        }
        #region Unhandled
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Logger.Fatal($"{e.GetFullMessage()}");
            Logger.Fatal($"{args.IsTerminating}");
        }
        #endregion
    }
}