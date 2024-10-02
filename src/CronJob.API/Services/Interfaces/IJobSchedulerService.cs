using CronJob.API.Dto.Request;
using Quartz;

namespace CronJob.API.Services.Interfaces
{
    public interface IJobSchedulerService
    {
        Task StartJobsFromDatabaseAsync();
        Task GetAllSchedulersAsync();
        Task RegisterJobAsync<TJob>(string jobName, string triggerName, TimeSpan interval) where TJob : IJob;
        Task UpdateJobAsync<TJob>(UpdateJobDtoRequest request) where TJob : IJob;
        Task RemoveJobAsync(DeleteJobDtoRequest request);
    }
}
