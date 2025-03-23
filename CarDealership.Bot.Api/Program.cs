using CarDealership.Bot.Api.Constants;
using CarDealership.Bot.Api.Controllers;
using CarDealership.Bot.Api.External.GrpcClients.Abstraction;
using CarDealership.Bot.Api.External.GrpcClients.Implementation;
using CarDealership.Bot.Api.NotifHandlers;
using CarDealership.Bot.Api.RabbitMQ;
using CarDealership.Bot.Api.Settings;
using CarDealership.Bot.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5265, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
    });

    options.ListenAnyIP(7282, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });
});

var services = builder.Services;
var config = builder.Configuration;

services.AddControllers();

services.ConfigureAppSettings(config);

services.AddSingleton<CDBotDbContext>();


var telegramBotToken = config["TelegramBotToken"];

var botClient = new TelegramBotClient(telegramBotToken);


services.AddSingleton(botClient);

var notifications = LoadMessages(NotificationConstants.notificationsFileName);
services.AddSingleton(notifications);
NotifGetter.Init(notifications);
services.AddHostedService<TelegramBotBackgroundJobs>();
services.AddHostedService<RabbitMQListener>();

var storageGrpcServerUrl = Environment.GetEnvironmentVariable("STORAGE_GRPC_SERVER_URL") ?? "http://localhost:7292";
services.AddGrpcClient<Storage.FileStorage.FileStorageClient>(options =>
{
    options.Address = new Uri(storageGrpcServerUrl);
});

services.AddScoped<IStorageGrpcServiceClient, StorageGrpcServiceClient>();


services.AddDataAccess();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapControllers();

app.Run();


Dictionary<string, string> LoadMessages(string path)
{
    var json = File.ReadAllText(path);
    return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
}
