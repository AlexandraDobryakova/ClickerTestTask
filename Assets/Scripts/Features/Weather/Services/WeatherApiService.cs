using ClickerTestTask.Infrastructure.Network;
using ClickerTestTask.Features.Weather.Data;

namespace ClickerTestTask.Features.Weather.Services
{
    public sealed class WeatherApiService
    {
        private const string ForecastUrl = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";

        private readonly RequestQueue _requestQueue;
        private readonly UnityWebRequestClient _httpClient;

        public WeatherApiService(
            RequestQueue requestQueue,
            UnityWebRequestClient httpClient)
        {
            _requestQueue = requestQueue;
            _httpClient = httpClient;
        }

        public RequestHandle<WeatherForecastResponse> GetForecast()
        {
            return _requestQueue.Enqueue<WeatherForecastResponse>(
                token => _httpClient.GetAsync<WeatherForecastResponse>(
                    ForecastUrl,
                    token,
                    "application/geo+json"));
        }

        public RequestHandle<UnityEngine.Texture2D> GetIcon(string url)
        {
            return _requestQueue.Enqueue<UnityEngine.Texture2D>(
                token => _httpClient.GetTextureAsync(url, token));
        }
    }
}