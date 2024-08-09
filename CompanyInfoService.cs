using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;

public class CompanyInfoService : ICompanyInfoService
{
    private readonly HttpClient _client;
    private readonly BotConfiguration _config;

    public CompanyInfoService(HttpClient client, BotConfiguration config)
    {
        _client = client;
        _config = config;
    }

    public async Task<List<string>> GetCompaniesByInn(string[] inns)
    {
        var companyInfoList = new List<string>();

        foreach (var inn in inns)
        {
            var requestContent = new { query = inn };
            var url = $"{_config.ApiBaseUrl}/findById/party";
            var response = await SendPostRequest(url, requestContent);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var companyInfo = ExtractCompanyInfo(content);
                companyInfoList.Add($"ИНН: {inn} - {companyInfo}");
            }
            else
            {
                HandleError(response.StatusCode, companyInfoList, inn);
            }
        }

        return companyInfoList;
    }

    public async Task<List<string>> GetOkvedByInn(string[] inns)
    {
        var okvedInfoList = new List<string>();

        foreach (var inn in inns)
        {
            var requestContentInn = new { query = inn };
            var urlInn = $"{_config.ApiBaseUrl}/findById/party";
            var responseInn = await SendPostRequest(urlInn, requestContentInn);

            if (responseInn.IsSuccessStatusCode)
            {
                var contentInn = await responseInn.Content.ReadAsStringAsync();
                var okvedCode = ExtractOkvedCode(contentInn);

                if (!string.IsNullOrEmpty(okvedCode))
                {
                    var requestContentOkved = new { query = okvedCode };
                    var urlOkved = $"{_config.ApiBaseUrl}/suggest/okved2";
                    var responseOkved = await SendPostRequest(urlOkved, requestContentOkved);

                    if (responseOkved.IsSuccessStatusCode)
                    {
                        var contentOkved = await responseOkved.Content.ReadAsStringAsync();
                        var okvedInfo = ExtractOkvedInfo(contentOkved);
                        okvedInfoList.Add($"ИНН: {inn} - ОКВЭД: {okvedInfo}");
                    }
                    else
                    {
                        HandleError(responseOkved.StatusCode, okvedInfoList, inn);
                    }
                }
                else
                {
                    okvedInfoList.Add($"ИНН: {inn} - ОКВЭД не найден.");
                }
            }
            else
            {
                HandleError(responseInn.StatusCode, okvedInfoList, inn);
            }
        }

        return okvedInfoList;
    }

    public async Task<string> GetEgrulPdfByInn(string inn)
    {
        var url = $"{_config.ApiBaseUrl}/egrul?inn={inn}&apiKey={_config.ApiKey}";
        var response = await _client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var pdfFilePath = $"EGRUL_{inn}.pdf";

            using (var fs = new System.IO.FileStream(pdfFilePath, System.IO.FileMode.Create))
            {
                await response.Content.CopyToAsync(fs);
            }

            return pdfFilePath;
        }
        else
        {
            return $"Не удалось получить выписку из ЕГРЮЛ по ИНН {inn}: {response.ReasonPhrase}";
        }
    }

    private async Task<HttpResponseMessage> SendPostRequest(string url, object content)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", $"Token {_config.ApiKey}");
        request.Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");

        return await _client.SendAsync(request);
    }

    private void HandleError(System.Net.HttpStatusCode statusCode, List<string> list, string inn)
    {
        if (statusCode == System.Net.HttpStatusCode.NotFound)
        {
            list.Add($"ИНН: {inn} - Информация не найдена.");
        }
        else
        {
            list.Add($"ИНН: {inn} - Ошибка при получении данных: {statusCode}");
        }
    }

    private string ExtractCompanyInfo(string jsonResponse)
    {
        var data = JsonConvert.DeserializeObject<JObject>(jsonResponse);
        var suggestions = data["suggestions"];

        if (suggestions != null)
        {
            var result = new StringBuilder();

            foreach (var suggestion in suggestions)
            {
                var name = suggestion["value"]?.ToString();
                var address = suggestion["data"]?["address"]?["value"]?.ToString();
                var okved = suggestion["data"]?["okved"]?.ToString();

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(address))
                {
                    result.AppendLine("Информация о компании:");
                    result.AppendLine($"Наименование: {name}");
                    result.AppendLine($"Адрес: {address}");
                    result.AppendLine($"okved: {okved}");
                    result.AppendLine(new string('-', 40));
                }
            }

            return result.ToString().Trim();
        }

        return "Информация не найдена.";
    }

    private string ExtractOkvedCode(string jsonResponse)
    {
        var data = JsonConvert.DeserializeObject<JObject>(jsonResponse);
        var suggestions = data["suggestions"];

        return suggestions?.FirstOrDefault()?["data"]?["okved"]?.ToString();
    }

    private string ExtractOkvedInfo(string jsonResponse)
    {
        var data = JsonConvert.DeserializeObject<JObject>(jsonResponse);
        var suggestions = data["suggestions"];

        if (suggestions != null && suggestions.Any())
        {
            var result = new StringBuilder();
            var name = suggestions[0]?["value"]?.ToString();
            var code = suggestions[0]?["data"]?["kod"]?.ToString();

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(code))
            {
                result.AppendLine($"Наименование: {name}");
                result.AppendLine($"Код ОКВЭД: {code}");
            }

            return result.ToString().Trim();
        }

        return "Информация о ОКВЭД не найдена.";
    }
}
