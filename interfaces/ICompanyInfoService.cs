public interface ICompanyInfoService
{
    Task<List<string>> GetCompaniesByInn(string[] inns);
    Task<List<string>> GetOkvedByInn(string[] inns);
    Task<string> GetEgrulPdfByInn(string inn);
}
