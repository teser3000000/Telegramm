public class OkvedCommand : IBotCommand
{
    private readonly ICompanyInfoService _companyInfoService;

    public OkvedCommand(ICompanyInfoService companyInfoService)
    {
        _companyInfoService = companyInfoService;
    }

    public async Task<string> Execute(string message, long chatId)
    {
        var innNumbers = message.Replace("/okved", "").Trim().Split(' ');

        if (innNumbers.Length == 0 || string.IsNullOrWhiteSpace(innNumbers[0]))
        {
            return "Пожалуйста, укажите ИНН после команды /okved.";
        }

        foreach (var inn in innNumbers)
        {
            if (!IsValidInn(inn))
            {
                return $"ИНН {inn} некорректен. Пожалуйста, проверьте и попробуйте снова.";
            }
        }

        var results = await _companyInfoService.GetOkvedByInn(innNumbers);
        return string.Join("\n", results);
    }

    private bool IsValidInn(string inn)
    {
        return inn.Length == 10 || inn.Length == 12;
    }
}
