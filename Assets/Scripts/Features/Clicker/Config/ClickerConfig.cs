using UnityEngine;

namespace ClickerTestTask.Features.Clicker.Config
{
    [CreateAssetMenu(
        fileName = "ClickerConfig",
        menuName = "Clicker/Clicker Config"
    )]
    public class ClickerConfig : ScriptableObject
    {
        [Header("Currency")]
        [SerializeField] private int _startingCurrency = 0;
        [SerializeField] private int _clickReward = 1;

        [Header("Energy")]
        [SerializeField] private int _startingEnergy = 1000;
        [SerializeField] private int _maxEnergy = 1000;
        [SerializeField] private int _energyPerClick = 1;

        [Header("Auto Click")]
        [SerializeField] private float _autoClickInterval = 3f;

        [Header("Energy Regeneration")]
        [SerializeField] private int _energyRegenAmount = 10;
        [SerializeField] private float _energyRegenInterval = 10f;

        public int StartingCurrency => _startingCurrency;
        public int ClickReward => _clickReward;

        public int StartingEnergy => _startingEnergy;
        public int MaxEnergy => _maxEnergy;
        public int EnergyPerClick => _energyPerClick;

        public float AutoClickInterval => _autoClickInterval;

        public int EnergyRegenAmount => _energyRegenAmount;
        public float EnergyRegenInterval => _energyRegenInterval;
    }
}