using Hangfire;
using Hangfire.InMemory;
using HangFireNet8.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IJobService, JobService>();

//Add Hangfire 
builder.Services.AddHangfire(config =>
{
    config.UseInMemoryStorage()
          .UseFilter(new AutomaticRetryAttribute { Attempts = 2 });
});

// Add Hangfire server
builder.Services.AddHangfireServer(options =>
{
    options.ServerName = "Test_Server";
    options.WorkerCount = 4;
    options.Queues = new[] { "default", "queue1", "queue2" };
});


var app = builder.Build();


// Route to hangfire dashboard
app.UseHangfireDashboard();


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
