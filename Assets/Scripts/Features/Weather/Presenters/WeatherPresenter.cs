using ClickerTestTask.Features.Weather.Services;
using ClickerTestTask.Features.Weather.Views;
using ClickerTestTask.Infrastructure.Network;
using ClickerTestTask.Features.Weather.Data;
using ClickerTestTask.Core.Navigation;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System;

namespace ClickerTestTask.Features.Weather.Presenters
{
    public sealed class WeatherPresenter : IInitializable, IDisposable
    {
        private readonly WeatherView _view;
        private readonly WeatherApiService _weatherApi;
        private readonly NavigationController _navigation;

        private RequestHandle<WeatherForecastResponse> _forecastHandle;
        private RequestHandle<Texture2D> _iconHandle;

        private bool _isActive;
        private bool _hasAttemptedInitialLoad;

        private int _loopVersion;

        public WeatherPresenter(
            WeatherView view,
            WeatherApiService weatherApi,
            NavigationController navigation)
        {
            _view = view;
            _weatherApi = weatherApi;
            _navigation = navigation;
        }

        public void Initialize()
        {
            _navigation.TabChanged += OnTabChanged;

            if (_navigation.CurrentTab == NavigationTab.Weather)
                EnterTab();
        }

        public void Dispose()
        {
            _navigation.TabChanged -= OnTabChanged;

            ExitTab();
        }

        private void OnTabChanged(NavigationTab tab)
        {
            if (tab == NavigationTab.Weather)
                EnterTab();
            else
                ExitTab();
        }

        private void EnterTab()
        {
            if (_isActive)
                return;

            _isActive = true;

            _loopVersion++;

            UpdateWeatherLoop(_loopVersion).Forget();
        }

        private void ExitTab()
        {
            if (!_isActive)
                return;

            _isActive = false;

            _loopVersion++;

            _forecastHandle?.Cancel();
            _iconHandle?.Cancel();

            _forecastHandle = null;
            _iconHandle = null;

            _view.SetInitialLoading(false);
        }

        private async UniTask UpdateWeatherLoop(int version)
        {
            while (_isActive && version == _loopVersion)
            {
                await LoadWeather();

                if (!_isActive || version != _loopVersion)
                    break;

                await UniTask.Delay(TimeSpan.FromSeconds(5));

                if (!_isActive || version != _loopVersion)
                    break;
            }
        }

        private async UniTask LoadWeather()
        {
            bool showLoader = !_hasAttemptedInitialLoad;

            if (showLoader)
                _view.SetInitialLoading(true);

            var handle = _weatherApi.GetForecast();

            _forecastHandle = handle;

            try
            {
                WeatherForecastResponse response = await handle.Result;

                if (!_isActive || _forecastHandle != handle)
                    return;

                var periods = response?.properties?.periods;

                if (periods == null || periods.Length == 0)
                {
                    _view.ShowError("Прогноз погоды не найден.");
                    return;
                }

                WeatherPeriod period = Array.Find(
                    periods,
                    item => item != null && item.isDaytime
                );

                if (period == null)
                {
                    period = Array.Find(
                        periods,
                        item => item != null
                    );
                }

                if (period == null)
                {
                    _view.ShowError("Не удалось определить прогноз.");
                    return;
                }

                int temperature = period.temperature;

                if (period.temperatureUnit == "C")
                {
                    temperature = Mathf.RoundToInt(
                        temperature * 9f / 5f + 32f
                    );
                }

                Sprite iconSprite = null;

                if (!string.IsNullOrEmpty(period.icon))
                {
                    var iconHandle = _weatherApi.GetIcon(period.icon);

                    _iconHandle = iconHandle;

                    try
                    {
                        Texture2D texture = await iconHandle.Result;

                        if (!_isActive ||
                            _forecastHandle != handle ||
                            _iconHandle != iconHandle)
                        {
                            return;
                        }

                        iconSprite = Sprite.Create(
                            texture,
                            new Rect(
                                0,
                                0,
                                texture.width,
                                texture.height
                            ),
                            new Vector2(0.5f, 0.5f)
                        );
                    }
                    finally
                    {
                        if (_iconHandle == iconHandle)
                            _iconHandle = null;
                    }
                }

                if (!_isActive || _forecastHandle != handle)
                    return;

                _view.ShowWeather(
                    temperature,
                    period.shortForecast,
                    iconSprite
                );
            }
            catch (OperationCanceledException)
            {
                // Запрос отменён при выходе с вкладки.
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);

                if (_isActive && _forecastHandle == handle)
                {
                    _view.ShowError(
                        "Не удалось загрузить погоду."
                    );
                }
            }
            finally
            {
                if (_forecastHandle == handle)
                {
                    _forecastHandle = null;
                    _hasAttemptedInitialLoad = true;

                    _view.SetInitialLoading(false);
                }
            }
        }
    }
}