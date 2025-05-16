using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;
using System;
using System.Collections.Generic;
using Core;

namespace Assets._Project.Scripts.Network.Weather
{
    public class WeatherService : BaseDisposable, IWeatherService
    {        
        private const string Url = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";

        private readonly Dictionary<string, Sprite> _iconCache = new();
        private readonly Dictionary<string, Texture2D> _textureCache = new();

        public void ClearCache() => Dispose();

        protected override void OnDispose()
        {
            foreach (var sprite in _iconCache.Values)
            {
                if (sprite != null)
                    UnityEngine.Object.Destroy(sprite);
            }

            foreach (var texture in _textureCache.Values)
            {
                if (texture != null)
                    UnityEngine.Object.Destroy(texture);
            }

            _iconCache.Clear();
            _textureCache.Clear();

            base.OnDispose();
        }

        public async UniTask<WeatherForecast> GetForecastAsync(CancellationToken ct)
        {
            using var request = UnityWebRequest.Get(Url);
            request.SetRequestHeader("User-Agent", "UnityWeatherApp");

            var operation = await request.SendWebRequest().WithCancellation(ct);

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception(request.error);

            var json = request.downloadHandler.text;
            var forecast = JsonConvert.DeserializeObject<WeatherForecastResponse>(json);

            var first = forecast.properties.periods[0];
            return new WeatherForecast() { Name = first.name, Temperature = first.temperature, TemperatureUnit = first.temperatureUnit, IconUrl = first.icon };
        }

        public async UniTask<Sprite> LoadIconAsync(string url, CancellationToken ct)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("Icon URL is null or empty");

            if (_iconCache.TryGetValue(url, out var cachedSprite) && cachedSprite != null)
                return cachedSprite;

            using var www = UnityWebRequestTexture.GetTexture(url);
            await www.SendWebRequest().WithCancellation(ct);

            if (www.result != UnityWebRequest.Result.Success)
                throw new Exception($"Icon load failed: {www.error}");

            var texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            _textureCache[url] = texture;

            var sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));

            _iconCache[url] = sprite;

            return sprite;
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