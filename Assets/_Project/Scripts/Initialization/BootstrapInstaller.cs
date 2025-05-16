using Assets._Project.Scripts.DogsScreen.Factory;
using Assets._Project.Scripts.GUI.BotTabs;
using Assets._Project.Scripts.GUI.DogsScreen;
using Assets._Project.Scripts.GUI.WeatherScreen;
using Assets._Project.Scripts.Network;
using Assets._Project.Scripts.Network.Dogs;
using Assets._Project.Scripts.Network.Weather;
using GUI;
using Presets;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace MonoInstallers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [Header("Prefabs")]
        [SerializeField] private GUIView _guiViewPrefab;
        [SerializeField] private DogBreedItemView _breedItemViewPrefab;
        [Header("Presets")]
        [SerializeField] private GameSettings _gameSettings;

        private List<IDisposable> _disposables = new List<IDisposable>();

        public override void InstallBindings()
        {
            DontDestroyOnLoad(this);

            var guiInstance = Instantiate(_guiViewPrefab, transform);

            var botTabModel = new BotTabsModel();
            _disposables.Add(new BotTabController(new BotTabController.Ctx
            {
                view = guiInstance.botTabsView,
                model = botTabModel
            }));

            var requestQueue = new QueryManager();
            _disposables.Add(requestQueue);

            var weatherScreenController = new WeatherScreenController(new WeatherScreenController.Ctx
            {
                view = guiInstance.weatherScreenView,
                queryManager = requestQueue,
                service = new WeatherService(),
                onScreenChange = botTabModel.OnScreenChange,
                gameSettings = _gameSettings
            });
            _disposables.Add(weatherScreenController);

            Container.BindMemoryPool<DogBreedItemView, DogBreedItemPool>()
             .FromComponentInNewPrefab(_breedItemViewPrefab)
             .UnderTransform(guiInstance.dogsScreenView.contentParent);

            var dogsScreenController = new DogsScreenController(new DogsScreenController.Ctx
            {
                view = guiInstance.dogsScreenView,
                queryManager = requestQueue,
                service = new DogService(),
                onScreenChange = botTabModel.OnScreenChange,
                itemPool = Container.Resolve<DogBreedItemPool>(),
                gameSettings = _gameSettings
            });
            _disposables.Add(dogsScreenController);
        }

        private void OnDestroy()
        {
            for (int i = _disposables.Count - 1; i >= 0; i--)
                _disposables[i]?.Dispose();
            _disposables.Clear();
        }
    }
}