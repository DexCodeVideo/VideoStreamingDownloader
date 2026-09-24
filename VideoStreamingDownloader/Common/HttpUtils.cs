using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace VideoStreamingDownloader.Common
{
    internal static class HttpUtils
    {
        internal static async Task<T> GetRequestDeserializedResponse<T>(string url, Dictionary<string, string> cookies = null)
        {
            string json = await GetRequestStringResponse(url, cookies);
            return JsonSerializer.Deserialize<T>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        internal static async Task<string> GetRequestStringResponse(string url, Dictionary<string, string> cookies = null)
        {
            var cookieContainer = new CookieContainer();

            if (cookies != null)
            {
                foreach (var cookie in cookies)
                {
                    cookieContainer.Add(
                        new Uri(url),
                        new Cookie(cookie.Key, cookie.Value)
                    );
                }
            }

            var handler = new HttpClientHandler
            {
                CookieContainer = cookieContainer,
                UseCookies = true
            };

            using (HttpClient client = new HttpClient(handler))
            {
                var response = await client.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
        }

        internal static async Task<byte[]> GetRequestByteArrayResponse(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadAsByteArrayAsync();
            }
        }
    }
}
