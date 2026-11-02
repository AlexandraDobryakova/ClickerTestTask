using ClickerTestTask.Features.Clicker.Config;
using ClickerTestTask.Features.Clicker.Models;
using ClickerTestTask.Features.Clicker.Views;
using Cysharp.Threading.Tasks;
using System.Threading;
using Zenject;
using System;

namespace ClickerTestTask.Features.Clicker.Presenters
{
    public sealed class ClickerPresenter : IInitializable, IDisposable
    {
        private readonly ClickerModel _model;
        private readonly ClickerConfig _config;
        private readonly ClickerView _view;

        private CancellationTokenSource _cancellationTokenSource;

        public ClickerPresenter(
            ClickerModel model,
            ClickerConfig config,
            ClickerView view)
        {
            _model = model;
            _config = config;
            _view = view;
        }

        public void Initialize()
        {
            _view.Clicked += OnClicked;
            _model.ValuesChanged += OnValuesChanged;

            UpdateView();

            _cancellationTokenSource =
                new CancellationTokenSource();

            AutoClickLoop(_cancellationTokenSource.Token).Forget();
            EnergyRegenerationLoop(_cancellationTokenSource.Token).Forget();
        }

        public void Dispose()
        {
            _view.Clicked -= OnClicked;
            _model.ValuesChanged -= OnValuesChanged;

            if (_cancellationTokenSource == null)
                return;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        private void OnClicked()
        {
            if (_model.TryClick())
                _view.PlayRewardEffects(_config.ClickReward);
        }

        private void OnValuesChanged(int currency, int energy) => _view.SetValues(currency, energy, _model.MaxEnergy);

        private void UpdateView()
        {
            _view.SetValues(
                _model.Currency,
                _model.Energy,
                _model.MaxEnergy
            );
        }

        private async UniTask AutoClickLoop(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await UniTask.Delay(
                        TimeSpan.FromSeconds(_config.AutoClickInterval),
                        cancellationToken: cancellationToken
                    );

                    if (_model.TryClick())
                        _view.PlayRewardEffects(_config.ClickReward);
                }
            }
            catch (OperationCanceledException)
            {
                // Цикл остановлен.
            }
        }

        private async UniTask EnergyRegenerationLoop(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await UniTask.Delay(
                        TimeSpan.FromSeconds(_config.EnergyRegenInterval),
                        cancellationToken: cancellationToken
                    );

                    _model.RegenerateEnergy();
                }
            }
            catch (OperationCanceledException)
            {
                // Цикл остановлен.
            }
        }
    }
}