using Telegram.Bot;

public class InnCommand : IBotCommand
{
    private readonly ICompanyInfoService _companyInfoService;
    private readonly ITelegramBotClient _botClient;

    public InnCommand(ICompanyInfoService companyInfoService, ITelegramBotClient botClient)
    {
        _companyInfoService = companyInfoService;
        _botClient = botClient;
    }

    public async Task<string> Execute(string message, long chatId)
    {
        try
        {
            var innNumbers = message.Replace("/inn", "").Trim().Split(' ');

            foreach (var inn in innNumbers)
            {
                if (!IsValidInn(inn))
                {
                    return $"ИНН {inn} некорректен. Пожалуйста, проверьте и попробуйте снова.";
                }
            }

            var results = await _companyInfoService.GetCompaniesByInn(innNumbers);
            var resultMessage = string.Join("\n", results);

            const int MaxMessageLength = 4096;
            if (resultMessage.Length > MaxMessageLength)
            {
                var parts = SplitMessage(resultMessage, MaxMessageLength);
                foreach (var part in parts)
                {
                    await SendMessage(chatId, part); 
                }
                return null; 
            }
            return resultMessage;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при выполнении команды /inn: {ex.Message}");
            Console.WriteLine(ex.StackTrace); 

            return "Произошла ошибка при обработке вашего запроса. Пожалуйста, попробуйте позже.";
        }
    }

    private IEnumerable<string> SplitMessage(string message, int maxLength)
    {
        for (int i = 0; i < message.Length; i += maxLength)
        {
            yield return message.Substring(i, Math.Min(maxLength, message.Length - i));
        }
    }

    private async Task SendMessage(long chatId, string message)
    {
        await _botClient.SendTextMessageAsync(chatId, message);
    }

    private bool IsValidInn(string inn)
    {
        return inn.Length == 10 || inn.Length == 12;
    }
}
