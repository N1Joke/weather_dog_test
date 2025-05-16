using Assets._Project.Scripts.DogsScreen.Factory;
using Core;
using DG.Tweening;
using Tools.Extensions;
using UnityEngine;

namespace Assets._Project.Scripts.GUI.DogsScreen.DigBreedItemController
{
    public class DogBreedItemController : BaseDisposable, IDogBreedItem
    {
        public struct Ctx
        {
            public ReactiveEvent<IDogBreedItem> onClickAction;
            public string id;
            public DogBreedItemPool itemPool;
            public RectTransform parent;
            public string name;
        }

        private DogBreedItemView _view;
        private float _speed = 10;
        private readonly Ctx _ctx;
        private Tween _tween;        
        public string Id => _ctx.id;

        public DogBreedItemController(Ctx ctx)
        {
            _ctx = ctx;

            _view = _ctx.itemPool.Spawn(_ctx.parent);
            _view.lable.text = _ctx.name;

            Subscribe();

            ToggleLoading(false);
        }

        private void Subscribe()
        {
            _view.button.onClick.AddListener(() => _ctx.onClickAction?.Notify(this));
        }

        public void ToggleLoading(bool enable)
        {
            if (isDisposed)
                return;

            if (_tween.IsActive())
                _tween.Kill();

            if (enable)
            {
                _view.loadingRect.rotation = Quaternion.Euler(Vector3.zero);
                _view.loadingRect.gameObject.SetActive(true);
                _tween = _view.loadingRect.DORotate(new Vector3(0, 0, 359), _speed / 360)
                .SetLoops(-1, LoopType.Incremental)
                .SetEase(Ease.Linear)
                .SetLink(_view.loadingRect.gameObject);
            }
            else
                _view.loadingRect.gameObject.SetActive(false);
        }

        protected override void OnDispose()
        {
            if (_tween.IsActive())
                _tween.Kill();

            _ctx.itemPool.Despawn(_view);

            base.OnDispose();
        }
    }
}
