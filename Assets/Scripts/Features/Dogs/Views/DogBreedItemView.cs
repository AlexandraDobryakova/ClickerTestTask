using UnityEngine.UI;
using UnityEngine;
using Zenject;
using System;
using TMPro;

namespace ClickerTestTask.Features.Dogs.Views
{
    public sealed class DogBreedItemView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _nameText;

        private string _breedId;

        public event Action<string> BreedClicked;

        private void Awake()
        {
            if (_button != null)
                _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDestroy()
        {
            if (_button != null)
                _button.onClick.RemoveListener(OnButtonClicked);
        }

        public void Initialize(string breedId, string breedName)
        {
            _breedId = breedId;

            if (_nameText != null)
                _nameText.text = breedName;
        }

        private void OnButtonClicked()
        {
            if (!string.IsNullOrEmpty(_breedId))
                BreedClicked?.Invoke(_breedId);
        }

        public class Factory : PlaceholderFactory<DogBreedItemView>
        {
        }
    }
}