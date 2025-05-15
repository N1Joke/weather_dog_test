using Assets._Project.Scripts.Network;
using Assets._Project.Scripts.Network.Weather;
using Core;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Text;
using System.Threading;
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
        }

        private const string WeatherTag = "Weather";
        private const int DelayRefresh = 5;
        private readonly Ctx _ctx;
        private readonly WeatherScreenModel _model;
        private CancellationTokenSource _pollingCts;
        private CancellationTokenSource _iconCts;
        private IDisposable _loadingDisposable;
        private Tween _loadingTween;
        private StringBuilder _sb = new StringBuilder();
        private Sprite _lastSprite;
        private Texture2D _lastTexture;

        public WeatherScreenController(Ctx ctx)
        {
            _ctx = ctx;
            _model = new();

            //StartPolling();
        }

        private void StartPolling()
        {
            _pollingCts = new CancellationTokenSource();
            PollWeather(_pollingCts.Token).Forget();
        }

        private async UniTaskVoid PollWeather(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                _ctx.queryManager.Enqueue(WeatherTag, async token =>
                {
                    ShowLoading();
                    var forecast = await _ctx.service.GetForecastAsync(token);
                    ShowForecast(forecast);
                    HideLoading();
                });

                await UniTask.Delay(TimeSpan.FromSeconds(DelayRefresh), cancellationToken: ct);
            }
        }

        private void ShowLoading()
        {
            //to do
            //_loadingTween = rectTransform
            //.DORotate(new Vector3(0, 0, -360), rotationSpeed / 360f)                          
            //.SetLoops(-1, LoopType.Incremental) 
            //.SetEase(Ease.Linear).AddTo(this);
        }

        private void HideLoading()
        {
            if (_loadingTween.IsActive())
                _loadingTween.Kill();
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
            if (_lastSprite != null)
            {
                GameObject.Destroy(_lastSprite);
                GameObject.Destroy(_lastTexture);
            }

            try
            {
                using var www = UnityWebRequestTexture.GetTexture(url);
                await www.SendWebRequest().WithCancellation(ct);

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"Icon load failed: {www.error}");
                    return;
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
                
            }
            catch (Exception ex)
            {
                Debug.LogError($"Icon load error: {ex}");
            }
        }

        private void StopPolling()
        {
            _pollingCts?.Cancel();
            _ctx.queryManager.CancelCurrent();
            _ctx.queryManager.RemoveByTag(WeatherTag);
        }

        protected override void OnDispose()
        {
            _iconCts?.Cancel();
            _iconCts?.Dispose();
            _pollingCts?.Cancel();
            _pollingCts?.Dispose();
            _ctx.queryManager.RemoveByTag(WeatherTag);

            if (_lastSprite != null)
                GameObject.Destroy(_lastSprite);
            if (_lastTexture != null)
                GameObject.Destroy(_lastTexture);

            base.OnDispose();
        }
    }
}