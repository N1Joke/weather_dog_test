using AppConstants;
using Assets._Project.Scripts.DogsScreen.Factory;
using Assets._Project.Scripts.GUI.DogsScreen.DigBreedItemController;
using Assets._Project.Scripts.Network;
using Assets._Project.Scripts.Network.Dogs;
using Core;
using System;
using System.Collections.Generic;
using System.Threading;
using Tools.Extensions;
using UnityEditor.Search;
using UnityEngine;
using Zenject;

namespace Assets._Project.Scripts.GUI.DogsScreen
{
    public class DogsScreenController : BaseDisposable
    {
        public struct Ctx
        {
            public DogsScreenView view;
            public QueryManager queryManager;
            public IDogService service;
            public ReactiveEvent<string> onScreenChange;
            public DogBreedItemPool itemPool;
        }
                
        private readonly Ctx _ctx;
        private CancellationTokenSource _requestBreedsCts;
        private CancellationTokenSource _loadInfoCts;
        private readonly DogScreenModel _screenModel;

        public DogsScreenController(Ctx ctx)
        {
            _ctx = ctx;
            _screenModel = new();

            _ctx.view.popupView.gameObject.SetActive(false);

            Subscribe();
        }

        private void Subscribe()
        {
            _ctx.onScreenChange.Subscribe(ChangeScreen);
            _ctx.view.popupView.closeBtn.onClick.AddListener(() => TogglePopup(false));
        }

        public void TogglePopup(bool enable) => _ctx.view.popupView.gameObject.SetActive(enable);

        private void ChangeScreen(string tag)
        {
            if (tag == Constants.DogTag)
                Activate();
            else
                Deactivate();
        }

        private void Activate()
        {            
            _ctx.view.gameObject.SetActive(true);
            Pull();
        }
        
        private void Deactivate()
        {
            _ctx.view.gameObject.SetActive(false);
            _screenModel.DisposeItems();
            TogglePopup(false);
            StopPull();
        }

        private void Pull()
        {
            _requestBreedsCts = new CancellationTokenSource();

            _ctx.queryManager.Enqueue(Constants.DogTag, async token =>
            {
                ShowLoading();

                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, _requestBreedsCts.Token);
                var linkedToken = linkedCts.Token;

                try
                {
                    ShowLoading();
                    var breeds = await _ctx.service.GetBreedsAsync(linkedToken);
                    ShowBreeds(breeds);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error loading: {ex}");
                }
                finally
                {
                    HideLoading();
                }
            },
            onCancel: () => HideLoading());
        }

        private void StopPull()
        {
            _requestBreedsCts?.Cancel();
            _requestBreedsCts?.Dispose();
            _requestBreedsCts = null;

            _loadInfoCts?.Cancel();
            _loadInfoCts?.Dispose();
            _loadInfoCts = null;

            _ctx.queryManager.RemoveByTag(Constants.DogTag);
        }

        private void ShowLoading()
        {
            //to do
        }

        private void HideLoading()
        {
            //to do
        }

        private void ShowBreeds(List<DogBreed> breeds)
        {
            for (int i = 0; i < breeds.Count; i++)
            {
                ReactiveEvent<IDogBreedItem> onClickEvent = new();
                AddDispose(onClickEvent.SubscribeWithSkip(LoadInfo));
                _screenModel.AddItem(new DogBreedItemController(new DogBreedItemController.Ctx
                {
                    onClickAction = onClickEvent,
                    id = breeds[i].Id,
                    itemPool = _ctx.itemPool,
                    name = breeds[i].Name,
                    parent = _ctx.view.contentParent
                }));

                if (i == 9)
                    return;
            }
        }

        private void LoadInfo(IDogBreedItem item)
        {
            TogglePopup(false);

            _loadInfoCts?.Cancel();
            _loadInfoCts?.Dispose();
            _loadInfoCts = new CancellationTokenSource();

            item.ToggleLoading(true);

            _ctx.queryManager.Enqueue(Constants.DogTag, async token =>
            {
                ShowLoading();

                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, _loadInfoCts.Token);
                var linkedToken = linkedCts.Token;

                try
                {
                    var details = await _ctx.service.GetBreedInfoAsync(item.Id, linkedToken);

                    if (linkedToken.IsCancellationRequested)
                        return;

                    _ctx.view.popupView.gameObject.SetActive(true);
                    _ctx.view.popupView.header.text = details.Name;
                    _ctx.view.popupView.description.text = details.Description;

                    item.ToggleLoading(false);
                }
                catch (OperationCanceledException) { }
                finally
                {
                    item.ToggleLoading(false);
                }
            },
            onCancel: () => { item.ToggleLoading(false); });
        }

        protected override void OnDispose()
        {
            StopPull();
            _screenModel.DisposeItems();

            base.OnDispose();
        }
    }
}