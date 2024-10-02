using Quartz;

namespace CronJob.API.Dto.Request
{
    public class UpdateJobDtoRequest : IJob
    {
        public string JobName { get; set; } = string.Empty;
        public string TriggerName { get; set; } = string.Empty;
        public TimeSpan Interval { get; set; }

        public UpdateJobDtoRequest(string jobName, string triggerName, TimeSpan interval)
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
