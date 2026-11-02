using System;

namespace ClickerTestTask.Features.Weather.Data
{
    [Serializable]
    public class WeatherForecastResponse
    {
        public WeatherProperties properties;
    }

    [Serializable]
    public class WeatherProperties
    {
        public WeatherPeriod[] periods;
    }

    [Serializable]
    public class WeatherPeriod
    {
        public string name;
        public int temperature;
        public string temperatureUnit;
        public string shortForecast;
        public string icon;
        public bool isDaytime;
    }
}