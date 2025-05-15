using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;

namespace Assets._Project.Scripts.Network.Weather
{
    public class WeatherService : IWeatherService
    {        
        private const string Url = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";

        public async UniTask<WeatherForecast> GetForecastAsync(CancellationToken ct)
        {
            using var request = UnityWebRequest.Get(Url);
            request.SetRequestHeader("User-Agent", "UnityWeatherApp");

            var operation = await request.SendWebRequest().WithCancellation(ct);

            if (request.result != UnityWebRequest.Result.Success)
                throw new System.Exception(request.error);

            var json = request.downloadHandler.text;
            var forecast = JsonConvert.DeserializeObject<WeatherForecastResponse>(json);

            var first = forecast.properties.periods[0];
            return new WeatherForecast() { Name = first.name, Temperature = first.temperature, TemperatureUnit = first.temperatureUnit, IconUrl = first.icon };
        }

        [System.Serializable]
        private class WeatherForecastResponse
        {
            public Properties properties;

            [System.Serializable]
            public class Properties
            {
                public Period[] periods;
            }

            [System.Serializable]
            public class Period
            {
                public string name;
                public int temperature;
                public string temperatureUnit;
                public string icon;
            }
        }
    }
}