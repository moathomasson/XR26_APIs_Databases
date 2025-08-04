using System;
using Newtonsoft.Json;

namespace WeatherApp.Data
{
    /// <summary>
    /// Main weather data structure matching OpenWeatherMap API response
    /// </summary>
    [Serializable]
    public class WeatherData
    {
        [JsonProperty("main")] // Maps to "main" object in JSON
        public MainWeatherInfo Main { get; set; }

        [JsonProperty("weather")] // Maps to "weather" array
        public WeatherDescription[] Weather { get; set; }

        [JsonProperty("name")] // Maps to "name" of the city
        public string CityName { get; set; }

        [JsonProperty("cod")] // Maps to response status code
        public int StatusCode { get; set; }

        // Computed properties
        public float TemperatureInCelsius => Main?.Temperature - 273.15f ?? 0f;
        public string PrimaryDescription => Weather?.Length > 0 ? Weather[0].Description : "Unknown";

        public bool IsValid => StatusCode == 200 && Main != null && !string.IsNullOrEmpty(CityName);
    }

    [Serializable]
    public class MainWeatherInfo
    {
        [JsonProperty("temp")]
        public float Temperature { get; set; }

        [JsonProperty("feels_like")]
        public float FeelsLike { get; set; }

        [JsonProperty("humidity")]
        public int Humidity { get; set; }

        [JsonProperty("pressure")]
        public int Pressure { get; set; }
    }

    [Serializable]
    public class WeatherDescription
    {
        [JsonProperty("main")]
        public string Main { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }
    }
}
