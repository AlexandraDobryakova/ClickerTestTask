using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace ClickerTestTask.Features.Weather.Views
{
    public sealed class WeatherView : MonoBehaviour
    {
        [Header("Weather Info")]
        [SerializeField] private TMP_Text _temperatureText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Image _weatherIcon;

        [Header("Loading")]
        [SerializeField] private GameObject _loader;

        [Header("Error")]
        [SerializeField] private TMP_Text _errorText;

        private void Awake()
        {
            if (_weatherIcon != null)
                _weatherIcon.gameObject.SetActive(false);

            if (_errorText != null)
                _errorText.gameObject.SetActive(false);
        }

        public void SetInitialLoading(bool isLoading)
        {
            if (_loader != null)
                _loader.SetActive(isLoading);
        }

        public void ShowWeather(
            int temperature,
            string description,
            Sprite icon)
        {
            if (_temperatureText != null)
                _temperatureText.text = $"{temperature}°F";

            if (_descriptionText != null)
                _descriptionText.text = description;

            if (_weatherIcon != null)
            {
                if (icon != null)
                {
                    _weatherIcon.sprite = icon;
                    _weatherIcon.gameObject.SetActive(true);
                }
                else
                {
                    _weatherIcon.gameObject.SetActive(false);
                }
            }

            if (_errorText != null)
                _errorText.gameObject.SetActive(false);
        }

        public void ShowError(string message)
        {
            if (_errorText == null)
                return;

            _errorText.text = message;
            _errorText.gameObject.SetActive(true);
        }
    }
}