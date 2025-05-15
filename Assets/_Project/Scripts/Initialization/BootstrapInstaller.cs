using Assets._Project.Scripts.GUI.BotTabs;
using Assets._Project.Scripts.GUI.DogsScreen;
using Assets._Project.Scripts.GUI.WeatherScreen;
using Assets._Project.Scripts.Network;
using Assets._Project.Scripts.Network.Dogs;
using Assets._Project.Scripts.Network.Weather;
using GUI;
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
        //[SerializeField] private UnitPresetCollection _unitPresetCollection;
        
        private List<IDisposable> _disposables = new List<IDisposable>();

        public override void InstallBindings()
        {
            DontDestroyOnLoad(this);

            #region Scriptable objects
            //Container.Bind<UnitPresetCollection>().FromScriptableObject(_unitPresetCollection).AsSingle();            
            #endregion

            var guiInstance = Instantiate(_guiViewPrefab, transform);

            _disposables.Add(new BotTabController(new BotTabController.Ctx
            {
                view = guiInstance.botTabsView
            }));

            var requestQueue = new QueryManager();
            _disposables.Add(requestQueue);

            var weatherScreenController = new WeatherScreenController(new WeatherScreenController.Ctx
            {
                view = guiInstance.weatherScreenView,
                queryManager = requestQueue,
                service = new WeatherService()
            });
            _disposables.Add(weatherScreenController);

            var dogsScreenController = new DogsScreenController(new DogsScreenController.Ctx
            {
                view = guiInstance.dogsScreenView,
                queryManager = requestQueue,
                service = new DogService()
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