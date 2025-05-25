using Hangfire;
using Hangfire.States;
using HangFireNet8.Services;
using Microsoft.AspNetCore.Mvc;

namespace HangFireNet8.Controllers
{

    [ApiController]
    public class JobsController : Controller
    {
        private readonly IJobService _jobService;
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly IRecurringJobManager _recurringJobManager;
        public JobsController(IJobService jobService, IBackgroundJobClient backgroundJobs, IRecurringJobManager recurringJobManager)
        {
            _jobService = jobService;
            _backgroundJobs = backgroundJobs;
            _recurringJobManager = recurringJobManager;
        }

        /// <summary>
        /// Fire and Forget jobs are executed only once and almost immediately after creation
        /// </summary>
        /// <returns></returns>
        [HttpGet("jobs/fire-and-forget")]
        public IActionResult FireAndForget()
        {
            Console.WriteLine($"Request: {DateTime.Now}");

            // Inserting into a specific queue
            _backgroundJobs.Create<IJobService>(job => job.SumNumber(1, 3), new EnqueuedState("queue1"));

            _backgroundJobs.Enqueue<IJobService>(job => job.SendMessage("FireAndForget"));

            return Ok();
        }

        /// <summary>
        /// Delayed jobs are also executed only once, but not immediately. They run after a specified time interval.
        /// </summary>
        /// <returns></returns>
        [HttpGet("jobs/job-delayed")]
        public IActionResult JobDelayed()
        {
            Console.WriteLine($"Request: {DateTime.Now}");
            var jobDelayed = _backgroundJobs.Schedule<IJobService>(job =>
                                                    job.SendMessage($"JobDelayed"),
                                                    TimeSpan.FromSeconds(30));

            ContinueWith(jobDelayed);

            return Ok();
        }

        /// <summary>
        /// Continuation jobs are executed when their parent job has finished
        /// </summary>
        /// <returns></returns>
        private void ContinueWith(string jobId)
        {
            Console.WriteLine($"Request: {DateTime.Now}");

            // jobId is the ID of the service that this method will wait for before starting execution.
            _backgroundJobs.ContinueJobWith<IJobService>(jobId, job => job.SendMessage("ContinuedWith"));
        }

        /// <summary>
        /// Recurring jobs are executed multiple times on a specific CRON schedule
        /// </summary>
        /// <returns></returns>
        [HttpGet("jobs/recuring-job")]
        public IActionResult RecurringJobAddOrUpdate()
        {
            _recurringJobManager.AddOrUpdate<IJobService>(
                recurringJobId: "Recurrent-Job-Every-Minute",
                methodCall: job => job.SendMessage("RecurringJob-AddOrUpdate"),
                cronExpression: Cron.Minutely,
                queue: "queue1"
             );

            return Ok();
        }

        /// <summary>
        /// Recurring jobs are executed multiple times on a specific CRON schedule
        /// </summary>
        /// <returns></returns>
        [HttpGet("jobs/recuring-job30secs")]
        public IActionResult RecurringJobIntervalSmallerThan1min()
        {
            // Re-schedule the same job in 30 seconds
            _recurringJobManager.AddOrUpdate<IJobService>(
     "Job-At-Second-30", job => job.SendMessage("at :30"), "*/1 * * * *"
 );

            return Ok();
        }

        [HttpGet("jobs/exception-job")]
        public IActionResult ExceptionJob()
        {
            BackgroundJob.Enqueue<IJobService>(x =>
                x.JobException("Enqueue Job And Throw Exception"));

            return Ok();
        }
    }
}
