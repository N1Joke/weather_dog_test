using AppConstants;
using Assets._Project.Scripts.Network;
using Assets._Project.Scripts.Network.Weather;
using Core;
using Cysharp.Threading.Tasks;
using Presets;
using System;
using System.Text;
using System.Threading;
using Tools.Extensions;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets._Project.Scripts.GUI.WeatherScreen
{
    public class WeatherScreenController : BaseDisposable
    {
        public struct Ctx
        {
            public WeatherScreenView view;
            public QueryManager queryManager;
            public IWeatherService service;
            public ReactiveEvent<string> onScreenChange;
            public GameSettings gameSettings;
        }

        private readonly Ctx _ctx;
        private CancellationTokenSource _pollingCts;
        private CancellationTokenSource _iconCts;
        private StringBuilder _sb = new StringBuilder();
        private Sprite _lastSprite;
        private Texture2D _lastTexture;

        public WeatherScreenController(Ctx ctx)
        {
            _ctx = ctx;

            Subscribe();
        }

        private void Subscribe()
        {
            _ctx.onScreenChange.Subscribe(ChangeScreen);
        }

        private void ChangeScreen(string tag)
        {
            if (tag == Constants.WeatherTag)
                Activate();
            else
                Deactivate();
        }

        private void Activate()
        {
            _ctx.view.gameObject.SetActive(true);
            StartPolling();
        }

        private void Deactivate()
        {
            _ctx.view.gameObject.SetActive(false);
            StopPolling();
        }

        private void StartPolling()
        {
            _pollingCts = new CancellationTokenSource();
            _iconCts = new CancellationTokenSource();
            PollWeather(_pollingCts.Token).Forget();
        }

        private void StopPolling()
        {
            _pollingCts?.Cancel();
            _pollingCts?.Dispose();
            _pollingCts = null;

            _iconCts?.Cancel();
            _iconCts?.Dispose();
            _iconCts = null;

            _ctx.queryManager.RemoveByTag(Constants.WeatherTag);
        }

        private async UniTaskVoid PollWeather(CancellationToken outerToken)
        {
            while (!outerToken.IsCancellationRequested)
            {
                Debug.Log("Pull forecast");

                _ctx.queryManager.Enqueue(Constants.WeatherTag, async queryToken =>
                {
                    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(queryToken, outerToken);
                    var linkedToken = linkedCts.Token;

                    try
                    {
                        ShowLoading();
                        var forecast = await _ctx.service.GetForecastAsync(linkedToken);
                        ShowForecast(forecast);
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.Log("Weather request canceled");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Weather request error: {ex}");
                    }
                    finally
                    {
                        HideLoading();
                    }
                });

                try
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_ctx.gameSettings.weatherRefreshTimer), cancellationToken: outerToken);
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("Weather poll cycle canceled");
                    break; 
                }
            }
        }

        private void ShowLoading()
        {
            //to do
        }

        private void HideLoading()
        {
            //to do
        }

        private void ShowForecast(WeatherForecast forecast)
        {
            _sb.Clear();
            _sb.Append(forecast.Name);
            _sb.Append(" - ");
            _sb.Append(forecast.Temperature);
            _sb.Append(forecast.TemperatureUnit);
            _ctx.view.lable.text = _sb.ToString();

            LoadIcon(forecast.IconUrl, _iconCts.Token).Forget();
        }

        private async UniTaskVoid LoadIcon(string url, CancellationToken ct)
        {
            try
            {
                using var www = UnityWebRequestTexture.GetTexture(url);
                await www.SendWebRequest().WithCancellation(ct);

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"Icon load failed: {www.error}");
                    return;
                }

                if (_lastSprite != null)
                {
                    GameObject.Destroy(_lastSprite);
                    GameObject.Destroy(_lastTexture);
                }

                var texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                _lastTexture = texture;
                if (_ctx.view.icon != null)
                {
                    _lastSprite = Sprite.Create(texture,
                        new Rect(0, 0, texture.width, texture.height),
                        Vector2.one * 0.5f);
                    _ctx.view.icon.sprite = _lastSprite;
                }
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Request Icon load canceled");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Icon load error: {ex}");
            }
        }

        protected override void OnDispose()
        {
            _iconCts?.Cancel();
            _iconCts?.Dispose();
            _pollingCts?.Cancel();
            _pollingCts?.Dispose();
            _ctx.queryManager.RemoveByTag(Constants.WeatherTag);

            if (_lastSprite != null)
                GameObject.Destroy(_lastSprite);
            if (_lastTexture != null)
                GameObject.Destroy(_lastTexture);

            _ctx.service.ClearCache();

            base.OnDispose();
        }
    }
}