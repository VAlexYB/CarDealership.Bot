namespace CarDealership.Bot.Api.NotifHandlers
{
    public static class NotifFormatter
    {
        public static string FormatNotif(string template, Dictionary<string, string> parameters)
        {
            foreach (var parameter in parameters)
            {
                template = template.Replace(parameter.Key, parameter.Value);
            }
            return template;
        }
    }
}
