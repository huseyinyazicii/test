using QueueDemo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<INameQueueService, NameQueueService>();
builder.Services.AddSingleton(typeof(IBackgroundTaskQueue<string>), typeof(NameQueue));
builder.Services.AddHostedService<Queue1HostedService>();
builder.Services.AddHostedService<Queue2HostedService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
