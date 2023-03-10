using RabbitMQ.Client;
using RabbitMQ.EventBus;
using RabbitMQ.EventBus.Producer;
using WeatherMicroService.Middlewares;
using WeatherMicroService.RPC;
using WeatherMicroService.Services;
using WeatherMicroService.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<OpenWeatherApiService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region RabbitMQ Configuration

builder.Services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
{
    var factory = new ConnectionFactory()
    {
        HostName = builder.Configuration["EventBus:HostName"]
    };
    factory.AutomaticRecoveryEnabled = true;
    factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(5);
    factory.TopologyRecoveryEnabled = true;

    if (!string.IsNullOrWhiteSpace(builder.Configuration["EventBus:UserName"]))
        factory.UserName = builder.Configuration["EventBus:UserName"];

    if (!string.IsNullOrWhiteSpace(builder.Configuration["EventBus:Password"]))
        factory.Password = builder.Configuration["EventBus:Password"];

    var retryCount = 3;

    if (!string.IsNullOrWhiteSpace(builder.Configuration["EventBus:RetryCount"]))
        retryCount = int.Parse(builder.Configuration["EventBus:RetryCount"]);

    return new DefaultRabbitMQPersistentConnection(factory, retryCount);
});
builder.Services.AddScoped<EventBusRabbitMQProducer>();
builder.Services.AddSingleton<RpcClient>();
builder.Services.AddSingleton<RpcServer>();

#endregion


builder.Services.Configure<WeatherApiSettings>(
    builder.Configuration.GetSection(WeatherApiSettings.Position));

builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection(MongoSettings.Position));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRequestCulture();

app.UseAuthorization();

app.MapControllers();

app.Run();
