using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace PresenceHTTPClient.APIClients;

public abstract class BaseAPIClient
{
    protected readonly HttpClient _httpClient;
    protected readonly ILogger _logger;
    protected readonly JsonSerializerOptions _jsonOptions;

    protected BaseAPIClient(IHttpClientFactory httpClientFactory, ILogger logger)
    {
        _httpClient = httpClientFactory.CreateClient("PresenceApi");
        _logger = logger;
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    protected async Task<T> GetAsync<T>(string endpoint)
    {
        try
        {
            var fullEndpoint = endpoint.StartsWith("api/") ? endpoint : $"api/{endpoint}";
            _logger.LogDebug("Отправка запроса получения на {Endpoint}", fullEndpoint);
            
            var response = await _httpClient.GetAsync(fullEndpoint);
            
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Конечная точка: {Endpoint} вернул ошибку 404", fullEndpoint);
                return default;
            }
            
            return await HandleResponse<T>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении ответа от {Endpoint}", endpoint);
            return default;
        }
    }
    
    protected async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            _logger.LogDebug("Отправка POST запроса к {Endpoint} с данными {@Data}", endpoint, data);
            var response = await _httpClient.PostAsJsonAsync(endpoint, data, _jsonOptions);
            return await HandleResponse<TResponse>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка запроса POST к {Endpoint}", endpoint);
            return default;
        }
    }

    protected async Task<bool> DeleteAsync(string endpoint)
    {
        try
        {
            _logger.LogDebug("Отправка DELETE запроса к {Endpoint}", endpoint);
            var response = await _httpClient.DeleteAsync(endpoint);
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Запрос DELETE к {Endpoint} не выполнен со статусом {StatusCode}", 
                    endpoint, response.StatusCode);
            }
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка в запросе DELETE, {Endpoint}", endpoint);
            return false;
        }
    }

    private async Task<T> HandleResponse<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            _logger.LogError("Запрос не выполнен со статусом {StatusCode}. Ответ: {Response}", 
                response.StatusCode, content);
            return default;
        }

        try
        {
            var result = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
            _logger.LogDebug("Запрос выполнен успешно, ответ {@Response}", result);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка десериализации ответа до типа {Type}", typeof(T).Name);
            return default;
        }
    }
}