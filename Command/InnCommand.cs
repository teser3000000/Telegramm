public class InnCommand : IBotCommand
{
    private readonly ICompanyInfoService _companyInfoService;

    public InnCommand(ICompanyInfoService companyInfoService)
    {
        _companyInfoService = companyInfoService;
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
            return string.Join("\n", results);
        }
        catch (Exception ex)
        {
            // Логирование ошибки
            Console.WriteLine($"Ошибка в InnCommand: {ex.Message}");
            return "Произошла ошибка при обработке вашего запроса. Пожалуйста, попробуйте позже.";
        }
    }

    private bool IsValidInn(string inn)
    {
        return inn.Length == 10 || inn.Length == 12;
    }
}

