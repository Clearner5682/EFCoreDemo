using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Hangfire;
using Hangfire.Common;
using Hangfire.AspNetCore;
using IServices;

namespace Web
{
    public class TaskManager
    {
        IBackgroundJobClient _backgroundJobClient;
        IRecurringJobManager _recurringJobManager;
        IMoguGoodItemService _moguGoodItemService;
        public TaskManager(IBackgroundJobClient backgroundJobClient
            ,IRecurringJobManager recurringJobManager
            ,IMoguGoodItemService moguGoodItemService)
        {
            // 任务调度的客户端，实际任务应该是存在与JobStorage中，客户端应该可以有多个
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
            _moguGoodItemService = moguGoodItemService;
        }

        public void RegisterTasks()
        {
            string jobId= BackgroundJob.Enqueue(() => Console.WriteLine("OneTime Job Executed"));
            RecurringJob.AddOrUpdate("CollectMoguGoodItemImage",()=>_moguGoodItemService.CollectGoodItemImage(), $"*/{2} * * * *");
        }
    }
}
