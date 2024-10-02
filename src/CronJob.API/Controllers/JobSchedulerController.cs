using CronJob.API.Dto.Request;
using CronJob.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CronJob.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobSchedulerController : ControllerBase
    {
        private readonly IJobSchedulerService _jobSchedulerService;

        public JobSchedulerController(IJobSchedulerService jobSchedulerService)
        {
            _jobSchedulerService = jobSchedulerService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterJob()
        {
            // Exemplo de como chamar o serviço para registrar um job
            //await _jobSchedulerService.RegisterJobAsync<JobDtoRequest>(
            //    jobName: "JobExemplo",
            //    triggerName: "TriggerExemplo",
            //    interval: TimeSpan.FromMinutes(5) // Define o intervalo de execução
            //);

            //await _jobSchedulerService.RegisterJobAsync<JobDtoRequest>(new JobDtoRequest('teste', 'te', new TimeSpan(1,1,1));

            return Ok("Job registrado com sucesso!");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateJob(UpdateJobDtoRequest request)
        {
            await _jobSchedulerService.UpdateJobAsync<UpdateJobDtoRequest>(request).ConfigureAwait(false);

            return Ok("Job atualizado com sucesso!");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteJob(DeleteJobDtoRequest request)
        {
            await _jobSchedulerService.RemoveJobAsync(request).ConfigureAwait(false);

            return Ok();
        }
    }
}
