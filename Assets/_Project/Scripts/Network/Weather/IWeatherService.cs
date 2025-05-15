using Cysharp.Threading.Tasks;
using System.Threading;

namespace Assets._Project.Scripts.Network.Weather
{
    public interface IWeatherService
    {
        UniTask<WeatherForecast> GetForecastAsync(CancellationToken ct);
    }
}

