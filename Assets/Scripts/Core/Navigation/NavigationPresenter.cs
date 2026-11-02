using Zenject;
using System;

namespace ClickerTestTask.Core.Navigation
{
    public class NavigationPresenter : IInitializable, IDisposable
    {
        private readonly NavigationView _view;
        private readonly NavigationController _controller;

        public NavigationPresenter(
            NavigationView view,
            NavigationController controller)
        {
            _view = view;
            _controller = controller;
        }

        public void Initialize()
        {
            _view.TabSelected += OnTabSelected;
            _controller.TabChanged += OnTabChanged;

            _controller.Open(NavigationTab.Clicker);
        }

        public void Dispose()
        {
            _view.TabSelected -= OnTabSelected;
            _controller.TabChanged -= OnTabChanged;
        }

        private void OnTabSelected(NavigationTab tab) => _controller.Open(tab);

        private void OnTabChanged(NavigationTab tab) => _view.ShowTab(tab);
    }
}