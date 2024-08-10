using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;

public class EgrulCommand : IBotCommand
{
    private readonly ICompanyInfoService _companyInfoService;
    private readonly ITelegramBotClient _botClient;

    public EgrulCommand(ICompanyInfoService companyInfoService, ITelegramBotClient botClient)
    {
        _companyInfoService = companyInfoService;
        _botClient = botClient;
    }

    public async Task<string> Execute(string message, long chatId)
    {
        var innNumbers = message.Replace("/egrul", "").Trim().Split(' ');

        foreach (var inn in innNumbers)
        {
            if (!IsValidInn(inn))
            {
                return $"ИНН {inn} некорректен. Пожалуйста, проверьте и попробуйте снова.";
            }

            var pdfFilePath = await _companyInfoService.GetEgrulPdfByInn(inn);

            if (pdfFilePath.Contains("Ошибка"))
            {
                return pdfFilePath;
            }
            else
            {
                await SendPdfFileToUser(chatId, pdfFilePath);
                return $"Выписка из ЕГРЮЛ для ИНН {inn} успешно отправлена.";
            }
        }

        return "Не удалось обработать запрос. Проверьте введенные ИНН.";
    }

    private bool IsValidInn(string inn)
    {
        return inn.Length == 10 || inn.Length == 12;
    }

    private async Task SendPdfFileToUser(long chatId, string pdfFilePath)
    {
        using (var stream = File.OpenRead(pdfFilePath))
        {
            var file = new InputOnlineFile(stream, Path.GetFileName(pdfFilePath));
            await _botClient.SendDocumentAsync(chatId, file);
        }
    }
}
