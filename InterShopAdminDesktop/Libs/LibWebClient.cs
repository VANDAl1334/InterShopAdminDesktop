using System;
using System.Data.Common;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using InterShopAdminDesktop.Views;

namespace InterShopAdminDesktop.Libs;

/// <summary>
/// Класс для вызова API 
/// </summary>
public static class LibWebClient
{
    private static string token;

    /// <summary>
    /// Токен доступа
    /// </summary>
    public static string Token
    {
        get
        {
            return token;
        }
        set
        {
            token = value; 
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);           
        }
    }

    /// <summary>
    /// Адрес сервера
    /// </summary>
    private static HttpClient httpClient = new();
    public static Uri BaseURL
    {
        get
        {
            return httpClient.BaseAddress;
        }
        set
        {            
            httpClient = new();
            httpClient.Timeout = TimeSpan.FromSeconds(5);
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.BaseAddress = value;
        }
    }

    static LibWebClient()
    {
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Асинхронно делает запрос к API по указанному URL и возвращает ответ 
    /// </summary>
    /// <param name="method">Тип</param>
    /// <param name="url">URL</param>
    /// <param name="content">Данные запроса</param>
    /// <returns></returns>
    public static async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, object content)
    {
        HttpResponseMessage response = new();
        MessageBox messageBox;
        if (method == HttpMethod.Post)
        {
        a:
            try
            {
                response = await httpClient.PostAsync(url, JsonContent.Create(content));
            }
            catch (Exception ex)
            {
                messageBox = new("Попробовать еще раз?", MessageBoxTypes.Question);
                bool result = await messageBox.ShowDialog(App.CurrentWindow);
                if (result)
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(10);
                    goto a;
                }
                (App.CurrentWindow as AuthWnd).Btn_Auth1.IsEnabled = true;
                Console.WriteLine("Запрос отменен из-за тайм-аута." + ex);
            }
        }
        else if (method == HttpMethod.Put)
        {
            response = await httpClient.PutAsync(url, JsonContent.Create(content));
        }
        else if (method == HttpMethod.Get)
        {
            StringBuilder queryParameters = new();
            if (content != null)
            {
                FieldInfo[] fields = content.GetType().GetFields();
                foreach (FieldInfo field in fields)
                {
                    queryParameters.Append($"{field.Name}={field.GetValue(content)}&");
                }
                queryParameters.Remove(queryParameters.Length - 1, 1);
            }
            response = await httpClient.GetAsync($"{url}?{queryParameters}");
        }
        else if (method == HttpMethod.Patch)
        {
            response = await httpClient.PatchAsync(url, JsonContent.Create(content));
        }

        // доделать
        else if (method == HttpMethod.Delete)
        {
            response = await httpClient.DeleteAsync(url);
        }

        return response;
    }
}