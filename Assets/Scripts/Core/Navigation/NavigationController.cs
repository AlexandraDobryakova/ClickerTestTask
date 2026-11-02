using System;

namespace ClickerTestTask.Core.Navigation
{
    public class NavigationController
    {
        public event Action<NavigationTab> TabChanged;

        public NavigationTab CurrentTab { get; private set; }

        private bool _hasCurrentTab;

        public void Open(NavigationTab tab)
        {
            if (_hasCurrentTab && CurrentTab == tab)
                return;

            CurrentTab = tab;
            _hasCurrentTab = true;

            TabChanged?.Invoke(tab);
        }
    }
}