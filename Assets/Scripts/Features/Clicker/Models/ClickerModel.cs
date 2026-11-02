using ClickerTestTask.Features.Clicker.Config;
using System;

namespace ClickerTestTask.Features.Clicker.Models
{
    public sealed class ClickerModel
    {
        private readonly ClickerConfig _config;

        public int Currency { get; private set; }
        public int Energy { get; private set; }

        public int MaxEnergy => _config.MaxEnergy;

        public event Action<int, int> ValuesChanged;

        public ClickerModel(ClickerConfig config)
        {
            _config = config;

            Currency = _config.StartingCurrency;

            Energy = Math.Clamp(
                _config.StartingEnergy,
                0,
                _config.MaxEnergy
            );
        }

        public bool TryClick()
        {
            if (Energy < _config.EnergyPerClick)
                return false;

            Energy -= _config.EnergyPerClick;
            Currency += _config.ClickReward;

            NotifyValuesChanged();

            return true;
        }

        public void RegenerateEnergy()
        {
            if (Energy >= _config.MaxEnergy)
                return;

            Energy += _config.EnergyRegenAmount;

            if (Energy > _config.MaxEnergy)
                Energy = _config.MaxEnergy;

            NotifyValuesChanged();
        }

        private void NotifyValuesChanged() => ValuesChanged?.Invoke(Currency, Energy);
    }
}