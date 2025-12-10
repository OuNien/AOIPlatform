using AOI.Infrastructure.Messaging;
using Microsoft.Extensions.Options;
using UI.GatewayApi;

var builder = WebApplication.CreateBuilder(args);

// Config
builder.Services.Configure<UIOptions>(builder.Configuration.GetSection("UI"));

// MQ
var host = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
builder.Services.AddSingleton<IMessageBus>(new RabbitMqMessageBus(host));

// Result store
builder.Services.AddSingleton<ResultStore>();

// ­I´º worker ¡÷ ­q¾\ aoi.gateway.{group}
builder.Services.AddHostedService<GatewayWorker>();

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
