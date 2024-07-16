using CarDealership.Bot.Api.Constants;

namespace CarDealership.Bot.Api.NotifHandlers
{
    public static class NotifGetter
    {
        private static Dictionary<string, string> _notifs;
        
        public static void Init(Dictionary<string, string> notifs)
        {
            _notifs = notifs ?? throw new ArgumentNullException(nameof(notifs));
        }

        public static string GetNotification(string key)
        {
            EnsureNotifsInitialized();
            return _notifs.TryGetValue(key, out var value) ? value : _notifs[NotificationConstants.errorNotif];
        }

        public static string GetNotificationOnErrorSetEmpty(string key)
        {
            EnsureNotifsInitialized();
            return _notifs.TryGetValue(key, out var value) ? value : string.Empty;
        }

        private static void EnsureNotifsInitialized()
        {
            if (_notifs == null)
            {
                throw new InvalidOperationException("Словарь уведомлений не инициализирован.");
            }
        }
    }
}
