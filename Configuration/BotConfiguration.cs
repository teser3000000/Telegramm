using Microsoft.Extensions.Configuration;

namespace Telegramm.Configuration
{
    public class BotConfiguration
    {
        public string TelegramToken { get; }
        public string ApiBaseUrl { get; }
        public string ApiKey { get; }

        public BotConfiguration(IConfiguration configuration)
        {
            TelegramToken = configuration["BotConfig:TelegramToken"];
            ApiBaseUrl = configuration["BotConfig:ApiBaseUrl"];
            ApiKey = configuration["BotConfig:ApiKeys:CompanyInfoApiKey"];
        }
    }

}
