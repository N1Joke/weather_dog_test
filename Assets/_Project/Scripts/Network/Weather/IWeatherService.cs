using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Assets._Project.Scripts.Network.Weather
{
    public interface IWeatherService
    {
        UniTask<WeatherForecast> GetForecastAsync(CancellationToken ct);
        UniTask<Sprite> LoadIconAsync(string url, CancellationToken ct);
        void ClearCache();
    }
}

