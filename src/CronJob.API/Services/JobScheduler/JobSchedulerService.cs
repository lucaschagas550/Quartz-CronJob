using CronJob.API.Dto.Request;
using CronJob.API.Services.Interfaces;
using Quartz;
using Quartz.Impl.Matchers;

namespace CronJob.API.Services.JobScheduler
{
    public class JobSchedulerService : IJobSchedulerService
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<JobSchedulerService> _logger;

        public JobSchedulerService(
            ISchedulerFactory schedulerFactory,
            ILogger<JobSchedulerService> logger)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        public async Task StartJobsFromDatabaseAsync()
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            // Opcional: iniciar o scheduler se ele ainda não estiver iniciado
            if (!scheduler.IsStarted)
            {
                await scheduler.Start();
            }

            // Listar todos os jobs já registrados no banco de dados
            var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

            foreach (var jobKey in jobKeys)
            {
                var jobDetail = await scheduler.GetJobDetail(jobKey);
                var triggers = await scheduler.GetTriggersOfJob(jobKey);

                // Para cada job, associe as triggers e agende o job se não estiver agendado
                foreach (var trigger in triggers)
                {
                    var nextFireTime = trigger.GetNextFireTimeUtc();

                    if (nextFireTime.HasValue && nextFireTime.Value > DateTimeOffset.UtcNow)
                    {
                        Console.WriteLine($"Job {jobKey.Name} agendado para {nextFireTime}");
                    }

                    // Opcional: reativar jobs se necessário
                    if (!await scheduler.CheckExists(trigger.Key))
                    {
                        await scheduler.ScheduleJob(jobDetail, trigger);
                    }
                }
            }
        }

        public async Task GetAllSchedulersAsync()
        {
            try
            {
                var schedulers = await _schedulerFactory.GetAllSchedulers().ConfigureAwait(false);

                foreach (var scheduler in schedulers)
                {
                    // Para cada scheduler, obtenha todos os job keys (os nomes dos jobs)
                    var jobKeys = await scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

                    foreach (var jobKey in jobKeys)
                        Console.WriteLine($"Scheduler: {scheduler.SchedulerName}, Job: {jobKey.Name}, Group: {jobKey.Group}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar obter os nomes dos jobs.");
            }
        }


        public async Task RegisterJobAsync<TJob>(string jobName, string triggerName, TimeSpan interval) where TJob : IJob
        {
            try
            {
                var scheduler = await _schedulerFactory.GetScheduler();

                var jobDetail = JobDetailCreate<TJob>(jobName);
                var trigger = TriggerCreate(triggerName, interval);

                await scheduler.ScheduleJob(jobDetail, trigger);

                Console.WriteLine($"Job {jobName} registrado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task UpdateJobAsync<TJob>(UpdateJobDtoRequest request) where TJob : IJob
        {
            try
            {
                var scheduler = await _schedulerFactory.GetScheduler();

                // Criar a chave do job (JobKey)
                var jobKey = new JobKey(request.JobName, "DefaultGroup");

                if (await scheduler.CheckExists(jobKey).ConfigureAwait(false))
                {
                    await scheduler.DeleteJob(jobKey);
                    Console.WriteLine($"Job '{request.JobName}' foi removido para atualização.");
                }

                var jobDetail = JobDetailCreate<TJob>(request.JobName);
                var trigger = TriggerCreate(request.TriggerName, request.Interval);

                // Agendar o novo job com a trigger atualizada
                await scheduler.ScheduleJob(jobDetail, trigger);

                Console.WriteLine($"Job '{request.JobName}' foi atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        public async Task RemoveJobAsync(DeleteJobDtoRequest request)
        {
            try
            {
                var scheduler = await _schedulerFactory.GetScheduler().ConfigureAwait(false);

                // Criar a chave do job (JobKey) baseado no nome e no grupo
                var jobKey = new JobKey(request.JobName, "DefaultGroup");

                if (await scheduler.DeleteJob(jobKey).ConfigureAwait(false))
                    Console.WriteLine($"Job '{request.JobName}' no grupo DefaultGroup foi removido com sucesso.");
                else
                    Console.WriteLine($"Job '{request.JobName}' no grupo DefaultGroup não foi encontrado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
        }

        private static ITrigger TriggerCreate(string triggerName, TimeSpan newInterval)
        {
            return TriggerBuilder.Create()
                .WithIdentity(triggerName, "DefaultGroup")
                .StartNow()
                .WithSimpleSchedule(x => x.WithInterval(newInterval).RepeatForever())
                .Build();
        }

        private static IJobDetail JobDetailCreate<TJob>(string jobName) where TJob : IJob
        {
            return JobBuilder.Create<TJob>()
                .WithIdentity(jobName, "DefaultGroup")
                .Build();
        }
    }
}
