using CronJob.API.Services.Interfaces;
using CronJob.API.Services.JobScheduler;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddQuartz(q =>
{
    q.UsePersistentStore(options =>
    {
        options.UseSqlServer("connectionString");
    });
});

builder.Services.AddQuartzHostedService(q => 
    q.WaitForJobsToComplete = true);

builder.Services.AddScoped<IJobSchedulerService, JobSchedulerService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
