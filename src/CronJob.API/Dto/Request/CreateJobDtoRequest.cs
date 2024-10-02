using Quartz;

namespace CronJob.API.Dto.Request
{
    public class CreateJobDtoRequest : IJob
    {
        public string JobName { get; set; } = string.Empty;
        public string TriggerName { get; set; } = string.Empty;
        public TimeSpan Interval { get; set; }

        public CreateJobDtoRequest(string jobName, string triggerName, TimeSpan interval)
        {
            JobName = jobName;
            TriggerName = triggerName;
            Interval = interval;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await Task.CompletedTask;
        }
    }
}
