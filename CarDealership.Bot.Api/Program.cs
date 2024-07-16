using CarDealership.Bot.Api.Constants;
using CarDealership.Bot.Api.Controllers;
using CarDealership.Bot.Api.NotifHandlers;
using CarDealership.Bot.Api.RabbitMQ;
using CarDealership.Bot.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

services.AddControllers();

services.AddDbContext<CDBotDbContext>(
        options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString(nameof(CDBotDbContext)));
        });

var telegramBotToken = config["TelegramBotToken"];
var botClient = new TelegramBotClient(telegramBotToken);


services.AddSingleton(botClient);

var notifications = LoadMessages(NotificationConstants.notificationsFileName);
services.AddSingleton(notifications);
NotifGetter.Init(notifications);
services.AddHostedService<TelegramBotBackgroundJobs>();
services.AddHostedService<RabbitMQListener>();

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
