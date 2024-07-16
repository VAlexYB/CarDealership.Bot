using IModel = RabbitMQ.Client.IModel;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using Telegram.Bot;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using CarDealership.Bot.DataAccess.Repositories;
using CarDealership.Bot.Api.Constants;
using CarDealership.Bot.Api.NotifHandlers;


namespace CarDealership.Bot.Api.RabbitMQ
{
    public class RabbitMQListener : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly string _queueName;
        private readonly IModel _channel;
        private readonly TelegramBotClient _botClient;

        public RabbitMQListener(IServiceScopeFactory serviceScopeFactory, TelegramBotClient botClient, IConfiguration configuration, Dictionary<string, string> notifications)
        {
            _serviceScopeFactory = serviceScopeFactory;

            var factory = new ConnectionFactory()
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"]
            };

            _queueName = configuration["RabbitMQ:Queues:CDQueue"];
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();
            _channel.QueueDeclare(queue: _queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            _botClient = botClient;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                var messageInfo = JsonSerializer.Deserialize<MessageInfo>(message);

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var userChatRepository = scope.ServiceProvider.GetRequiredService<IUserChatRepository>();

                    var chatId = await userChatRepository.GetChatIdByPhoneNumber(messageInfo.PhoneNumber);

                    if(chatId.HasValue)
                    {
                        string notificationTemplate = NotifGetter.GetNotification($"{messageInfo.Type}_status_{messageInfo.Status}");

                        var parameters = new Dictionary<string, string> { { NotificationConstants.orderId, messageInfo.Id } };
                        string notificationMessage = NotifFormatter.FormatNotif(notificationTemplate, parameters);
                        await _botClient.SendTextMessageAsync(chatId.Value, notificationMessage);
                    }
                }
            };
            _channel.BasicConsume(queue: _queueName,
                                  autoAck: true,
                                  consumer: consumer);
            return Task.CompletedTask;
        }

        public async override void Dispose()
        {
            await _botClient.CloseAsync();
            base.Dispose();
        }
    }
}
