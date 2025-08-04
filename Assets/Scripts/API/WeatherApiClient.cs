using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using WeatherApp.Data;
using WeatherApp.Config;

namespace WeatherApp.Services
{
    /// <summary>
    /// Modern API client for fetching weather data
    /// </summary>
    public class WeatherApiClient : MonoBehaviour
    {
        [Header("API Configuration")]
        [SerializeField] private string baseUrl = "https://api.openweathermap.org/data/2.5/weather";

        /// <summary>
        /// Fetches weather data for a specific city using async/await pattern
        /// </summary>
        public async Task<WeatherData> GetWeatherDataAsync(string city)
        {
            // Validate input city name
            if (string.IsNullOrWhiteSpace(city))
            {
                Debug.LogError("City name cannot be empty");
                return null;
            }

            // Check if API key is properly configured
            if (!ApiConfig.IsApiKeyConfigured())
            {
                Debug.LogError("API key not configured. Please set up your config.json file in StreamingAssets.");
                return null;
            }

            // Construct URL with city and API key
            string apiKey = ApiConfig.OpenWeatherMapApiKey;
            string url = $"{baseUrl}?q={city}&appid={apiKey}";

            // Send GET request using UnityWebRequest and await response
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                await request.SendWebRequest();

                // Handle different types of network responses
                switch (request.result)
                {
                    case UnityWebRequest.Result.Success:
                        return ParseWeatherData(request.downloadHandler.text);

                    case UnityWebRequest.Result.ConnectionError:
                        Debug.LogError($"Network connection failed: {request.error}");
                        break;

                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError($"HTTP Error {request.responseCode}: {request.error}");
                        break;

                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError($"Data processing error: {request.error}");
                        break;
                }

                return null;
            }
        }

        // Parses JSON string into WeatherData object using Newtonsoft.Json
        private WeatherData ParseWeatherData(string jsonString)
        {
            try
            {
                return JsonConvert.DeserializeObject<WeatherData>(jsonString);
            }
            catch (JsonException ex)
            {
                Debug.LogError($"JSON parsing failed: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Example usage on Start
        /// </summary>
        private async void Start()
        {
            var weatherData = await GetWeatherDataAsync("London");

            if (weatherData != null && weatherData.IsValid)
            {
                Debug.Log($"Weather in {weatherData.CityName}: {weatherData.TemperatureInCelsius:F1}°C");
                Debug.Log($"Description: {weatherData.PrimaryDescription}");
            }
            else
            {
                Debug.LogError("Failed to get weather data");
            }
        }
    }
}
