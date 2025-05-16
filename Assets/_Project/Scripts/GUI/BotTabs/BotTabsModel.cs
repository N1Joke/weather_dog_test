using Tools.Extensions;

namespace Assets._Project.Scripts.GUI.BotTabs
{
    public class BotTabsModel
    {
        public ReactiveEvent<string> OnScreenChange { get; } = new ();

        public void ChangeTab(string newTab) => OnScreenChange?.Notify(newTab);
    }
}
