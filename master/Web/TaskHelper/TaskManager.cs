using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.Common;
using Hangfire.AspNetCore;

namespace Web
{
    public class TaskManager
    {
        IBackgroundJobClient _backgroundJobClient;
        IRecurringJobManager _recurringJobManager;
        public TaskManager(IBackgroundJobClient backgroundJobClient,IRecurringJobManager recurringJobManager)// 任务调度的客户端，实际任务应该是存在与JobStorage中，客户端应该可以有多个
        {
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
        }

        public void RegisterTasks()
        {
            string jobId= BackgroundJob.Enqueue(() => Console.WriteLine("OneTime Job Executed"));
            RecurringJob.AddOrUpdate("CollectMoguGoodItemImage",() => Console.WriteLine("CollectMoguGoodItemImage Executed"), Cron.Minutely());
        }
    }
}
