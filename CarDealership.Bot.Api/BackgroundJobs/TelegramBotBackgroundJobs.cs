using CarDealership.Bot.Api.Constants;
using CarDealership.Bot.Api.NotifHandlers;
using CarDealership.Bot.Api.RabbitMQ;
using CarDealership.Bot.DataAccess.Repositories;
using CarDealership.Bot.DataAccess.Repositories.Impl;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CarDealership.Bot.Api.Controllers
{

    public class TelegramBotBackgroundJobs : BackgroundService
    {
        private readonly TelegramBotClient _client;
        private readonly IServiceProvider _serviceProvider;
        private int _offset;

        public TelegramBotBackgroundJobs(TelegramBotClient client, IServiceProvider serviceProvider, Dictionary<string, string> notifications)
        {
            _client = client;
            _serviceProvider = serviceProvider;
            _offset = 0;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var updates = await _client.GetUpdatesAsync(_offset, cancellationToken: stoppingToken);

                foreach (var update in updates)
                {
                    await HandleUpdate(update);
                    _offset = update.Id + 1;
                }

                await Task.Delay(1000, stoppingToken);
            }
        }


        private async Task HandleUpdate(Telegram.Bot.Types.Update update)
        {
            var message = update.Message;
            if (message?.Text?.StartsWith("/start") == true)
            {
                await HandleStartCommand(message.Chat.Id);

            }
            else
            {
                if (message?.Contact != null)
                {
                    await HandleContact(message.Contact.PhoneNumber, message.Chat.Id);
                }
                else if (message?.Text == NotifGetter.GetNotificationOnErrorSetEmpty(NotificationConstants.getShowroomInfoBtn))
                {
                    await HandleShowroomInfo(message.Chat.Id);  
                }
                await ShowAppropriateKeyboard(message.Chat.Id);
            }
        }

        private async Task HandleStartCommand(long chatId)
        {
            var greetings = NotifGetter.GetNotification(NotificationConstants.greetings);
            await _client.SendTextMessageAsync(chatId, greetings);

            var subscriptionBtn = NotifGetter.GetNotificationOnErrorSetEmpty(NotificationConstants.subscriptionBtn);
            var askForSubscription = NotifGetter.GetNotificationOnErrorSetEmpty(NotificationConstants.askForSubscription);
            var showroomInfoBtn = NotifGetter.GetNotificationOnErrorSetEmpty(NotificationConstants.getShowroomInfoBtn);
            if (!string.IsNullOrWhiteSpace(subscriptionBtn) && !string.IsNullOrWhiteSpace(askForSubscription))
            {
                var requestPhoneKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton(subscriptionBtn) { RequestContact = true },
                    new KeyboardButton(showroomInfoBtn)
                })
                {
                    ResizeKeyboard = true,
                    OneTimeKeyboard = true
                };

                await _client.SendTextMessageAsync(chatId, askForSubscription, replyMarkup: requestPhoneKeyboard);
            }
        }

        private async Task HandleContact(string phoneNumber, long chatId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var userChatRepository = scope.ServiceProvider.GetRequiredService<IUserChatRepository>();

                await userChatRepository.AddOrUpdateUserChatMapping(phoneNumber, chatId);
            }

            var thanksForSubscription = NotifGetter.GetNotification(NotificationConstants.thanksForSubscription);

            await _client.SendTextMessageAsync(chatId, thanksForSubscription);
        }

        private async Task HandleShowroomInfo(long chatId)
        {
            var showroomInfo = NotifGetter.GetNotification(NotificationConstants.showroomInfo);
            await _client.SendTextMessageAsync(chatId, showroomInfo);
        }

        private async Task ShowAppropriateKeyboard(long chatId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var userChatRepository = scope.ServiceProvider.GetRequiredService<IUserChatRepository>();
                var userHasProvidedContact = await userChatRepository.HasProvidedContact(chatId);

                if (userHasProvidedContact)
                {
                    await ShowKeyboardAfterSubscription(chatId);
                }
                else
                {
                    await ShowKeyboardBeforeSubscription(chatId);
                }
            }
        }

        private async Task ShowKeyboardBeforeSubscription(long chatId)
        {
            var showroomInfoBtn = NotifGetter.GetNotificationOnErrorSetEmpty(NotificationConstants.getShowroomInfoBtn);
            var subscriptionBtn = NotifGetter.GetNotificationOnErrorSetEmpty(NotificationConstants.subscriptionBtn);

            if (!string.IsNullOrEmpty(showroomInfoBtn) && !string.IsNullOrEmpty(subscriptionBtn))
            {
                var defaultKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton(subscriptionBtn) { RequestContact = true },
                    new KeyboardButton(showroomInfoBtn)
                })
                {
                    ResizeKeyboard = true,
                    OneTimeKeyboard = true
                };

                await _client.SendTextMessageAsync(chatId, "Выберите опцию", replyMarkup: defaultKeyboard);
            }
        }

        private async Task ShowKeyboardAfterSubscription(long chatId)
        {
            var showroomInfoBtn = NotifGetter.GetNotificationOnErrorSetEmpty(NotificationConstants.getShowroomInfoBtn);

            if (!string.IsNullOrEmpty(showroomInfoBtn))
            {
                var showroomInfoKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton(showroomInfoBtn)
                })
                {
                    ResizeKeyboard = true,
                    OneTimeKeyboard = false
                };

                await _client.SendTextMessageAsync(chatId, "Выберите опцию", replyMarkup: showroomInfoKeyboard);
            }
        }
    }
}
