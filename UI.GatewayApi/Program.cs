using AOI.Infrastructure.Messaging;
using AOIPlatform.Hubs;
using Microsoft.Extensions.Options;
using UI.GatewayApi;
using UI.GatewayApi.Dtos;
using UI.GatewayApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Config
builder.Services.Configure<UIOptions>(builder.Configuration.GetSection("UI"));

// MQ
var host = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
builder.Services.AddSingleton<IMessageBus>(new RabbitMqMessageBus(host));

// Result store
builder.Services.AddSingleton<ResultStore>();
builder.Services.AddSingleton<GrabProgressStore>();

// ­I´º worker ¡÷ ­q¾\ aoi.gateway.{group}
builder.Services.AddHostedService<GatewayWorker>();
builder.Services.AddHostedService<GrabProgressSubscriber>();

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ±Ò¥Î SignalR
builder.Services.AddSignalR(); 

var app = builder.Build();

app.MapHub<GrabStatusHub>("/grabStatusHub"); // µù¥U Hub Endpoint

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
