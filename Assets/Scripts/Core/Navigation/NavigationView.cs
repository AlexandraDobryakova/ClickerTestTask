using UnityEngine.UI;
using UnityEngine;
using System;

namespace ClickerTestTask.Core.Navigation
{
    public class NavigationView : MonoBehaviour
    {
        [Header("Navigation Buttons")]
        [SerializeField] private Button _clickerButton;
        [SerializeField] private Button _weatherButton;
        [SerializeField] private Button _dogsButton;

        [Header("Panels")]
        [SerializeField] private GameObject _clickerPanel;
        [SerializeField] private GameObject _weatherPanel;
        [SerializeField] private GameObject _dogsPanel;

        public event Action<NavigationTab> TabSelected;

        private void OnEnable()
        {
            _clickerButton.onClick.AddListener(OnClickerClicked);
            _weatherButton.onClick.AddListener(OnWeatherClicked);
            _dogsButton.onClick.AddListener(OnDogsClicked);
        }

        private void OnDisable()
        {
            _clickerButton.onClick.RemoveListener(OnClickerClicked);
            _weatherButton.onClick.RemoveListener(OnWeatherClicked);
            _dogsButton.onClick.RemoveListener(OnDogsClicked);
        }

        private void OnClickerClicked() => TabSelected?.Invoke(NavigationTab.Clicker);

        private void OnWeatherClicked() => TabSelected?.Invoke(NavigationTab.Weather);

        private void OnDogsClicked() => TabSelected?.Invoke(NavigationTab.Dogs);

        public void ShowTab(NavigationTab tab)
        {
            _clickerPanel.SetActive(tab == NavigationTab.Clicker);
            _weatherPanel.SetActive(tab == NavigationTab.Weather);
            _dogsPanel.SetActive(tab == NavigationTab.Dogs);
        }
    }
}