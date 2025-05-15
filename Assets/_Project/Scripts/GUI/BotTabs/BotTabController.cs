using Core;

namespace Assets._Project.Scripts.GUI.BotTabs
{
    public class BotTabController : BaseDisposable
    {
        public struct Ctx
        {
            public BotTabsView view;
        }

        private readonly Ctx _ctx;
        private readonly BotTabsModel _model;

        public BotTabController(Ctx ctx)
        {
            _ctx = ctx;
            _model = new();
        }
    }
}