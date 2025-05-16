using AppConstants;
using Core;
using Tools.Extensions;

namespace Assets._Project.Scripts.GUI.BotTabs
{
    public class BotTabController : BaseDisposable
    {
        public struct Ctx
        {
            public BotTabsView view;
            public BotTabsModel model;
        }

        private readonly Ctx _ctx;

        public BotTabController(Ctx ctx)
        {
            _ctx = ctx;

            _ctx.view.DogTabBtn.onClick.AddListener(() => _ctx.model.ChangeTab(Constants.DogTag));
            _ctx.view.WeatherTabBtn.onClick.AddListener(() => _ctx.model.ChangeTab(Constants.WeatherTag));

            _ctx.model.ChangeTab(Constants.WeatherTag);
        }
    }
}