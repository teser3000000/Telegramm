using Microsoft.Extensions.Configuration;

public class BotConfiguration
{
    public string TelegramToken { get; }
    public string ApiBaseUrl { get; }
    public string ApiKey { get; }

    public BotConfiguration(IConfiguration configuration)
    {
        TelegramToken = Environment.GetEnvironmentVariable("TelegramToken") ?? configuration["BotConfig:TelegramToken"];
        ApiBaseUrl = configuration["BotConfig:ApiBaseUrl"];
        ApiKey = Environment.GetEnvironmentVariable("CompanyInfoApiKey") ?? configuration["BotConfig:ApiKeys:CompanyInfoApiKey"];


        // Проверка на null
        if (string.IsNullOrEmpty(TelegramToken))
            throw new ArgumentNullException(nameof(TelegramToken), "TelegramToken не может быть null.");

        if (string.IsNullOrEmpty(ApiBaseUrl))
            throw new ArgumentNullException(nameof(ApiBaseUrl), "ApiBaseUrl не может быть null.");

        if (string.IsNullOrEmpty(ApiKey))
            throw new ArgumentNullException(nameof(ApiKey), "ApiKey не может быть null.");
    }
}
