using UniRx;

namespace Assets._Project.Scripts.GUI.BotTabs
{
    public class BotTabsModel
    {
        public const int WeatherTab = 1;
        public const int DogTab = 2;

        public ReactiveProperty<int> CurrentTab { get; } = new ReactiveProperty<int>(WeatherTab);

        public void ChangeTab(int newTab) => CurrentTab.Value = newTab;
    }
}
