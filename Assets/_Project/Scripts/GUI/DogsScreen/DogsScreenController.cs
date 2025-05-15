using Assets._Project.Scripts.Network;
using Assets._Project.Scripts.Network.Dogs;
using Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

namespace Assets._Project.Scripts.GUI.DogsScreen
{
    public class DogsScreenController : BaseDisposable
    {
        public struct Ctx
        {
            public DogsScreenView view;
            public QueryManager queryManager;
            public IDogService service;
        }

        private const string DogTag = "DogList";
        private readonly Ctx _ctx;
        private CancellationTokenSource _requestCts;

        public DogsScreenController(Ctx ctx)
        {
            _ctx = ctx;

            Pull();
        }

        private void Pull()
        {
            _requestCts = new CancellationTokenSource();

            _ctx.queryManager.Enqueue(DogTag, async token =>
            {
                ShowLoading();
                var breeds = await _ctx.service.GetBreedsAsync(token);
                ShowBreeds(breeds);
                HideLoading();
            },
            onCancel: () => HideLoading());
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
                var dogBreedItemViewInstance = GameObject.Instantiate(_ctx.view.breedItemViewPrefab, _ctx.view.contentParent);
                dogBreedItemViewInstance.lable.text = breeds[i].Name;
            }
        }
    }
}