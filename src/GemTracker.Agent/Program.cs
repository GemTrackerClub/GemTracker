using GemTracker.Agent.DI;
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
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);

            LogManager.Configuration.Variables["fileName"] = $"{appId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";
            LogManager.Configuration.Variables["archiveFileName"] = $"{appId}-{DateTime.UtcNow.ToString("ddMMyyyy")}.log";

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

                #region Fetch Data
                var fdfu = app.Jobs.FirstOrDefault(j => j.Name == "j-fetch-data-from-uniswap");

                IJobDetail fdfuJob = JobBuilder.Create<FetchDataFromUniswapJob>()
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

                #region SendAdvertisement
                var sa = app.Jobs.FirstOrDefault(j => j.Name == "j-send-advertisement");

                IJobDetail saJob = JobBuilder.Create<SendAdvertisement>()
                    .WithIdentity($"{sa.Name}Job")
                    .Build();

                saJob.JobDataMap["FileName"] = sa.Name;
                saJob.JobDataMap["StoragePath"] = app.StoragePath;

                var saBuilder = TriggerBuilder.Create()
                    .WithIdentity($"{sa.Name}Trigger")
                    .StartNow();

                saBuilder.WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(sa.IntervalInMinutes)
                        .RepeatForever());

                var saTrigger = saBuilder.Build();
                #endregion

                if (fdfu.IsActive)
                    await _scheduler.ScheduleJob(fdfuJob, fdfuTrigger);

                if (sa.IsActive)
                    await _scheduler.ScheduleJob(saJob, saTrigger);

                await Task.Delay(TimeSpan.FromSeconds(30));

                Console.ReadKey();
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