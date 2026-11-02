using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

namespace ClickerTestTask.Features.Dogs.Views
{
    public sealed class DogsView : MonoBehaviour
    {
        [Header("Breed List")]
        [SerializeField] private Transform _listRoot;
        [SerializeField] private GameObject _loader;

        [Header("Breed Popup")]
        [SerializeField] private GameObject _popup;
        [SerializeField] private TMP_Text _popupTitle;
        [SerializeField] private TMP_Text _popupDescription;
        [SerializeField] private Button _closeButton;
        [SerializeField] private GameObject _popupLoader;

        public event Action CloseClicked;

        private void Awake()
        {
            if (_closeButton != null)
                _closeButton.onClick.AddListener(OnCloseClicked);

            ClearPopup();
            HidePopup();
        }

        private void OnDestroy()
        {
            if (_closeButton != null)
                _closeButton.onClick.RemoveListener(OnCloseClicked);
        }

        private void OnCloseClicked() => CloseClicked?.Invoke();

        public void ShowLoader(bool isVisible)
        {
            if (_loader != null)
                _loader.SetActive(isVisible);
        }

        public void ClearList()
        {
            if (_listRoot == null)
                return;

            for (int i = _listRoot.childCount - 1; i >= 0; i--)
                Destroy(_listRoot.GetChild(i).gameObject);
        }

        public void AddBreedItem(DogBreedItemView item)
        {
            if (item == null || _listRoot == null)
                return;

            item.transform.SetParent(_listRoot, false);
        }

        public void ShowPopupLoader()
        {
            ClearPopup();

            if (_popup != null)
                _popup.SetActive(true);

            if (_popupLoader != null)
                _popupLoader.SetActive(true);
        }

        public void ShowPopup(string title, string description)
        {
            if (_popup != null)
                _popup.SetActive(true);

            if (_popupTitle != null)
                _popupTitle.text = title;

            if (_popupDescription != null)
                _popupDescription.text = description;

            if (_popupLoader != null)
                _popupLoader.SetActive(false);
        }

        public void ClearPopup()
        {
            if (_popupTitle != null)
                _popupTitle.text = string.Empty;

            if (_popupDescription != null)
                _popupDescription.text = string.Empty;
        }

        public void HidePopup()
        {
            if (_popupLoader != null)
                _popupLoader.SetActive(false);

            if (_popup != null)
                _popup.SetActive(false);

            ClearPopup();
        }

        public void ShowPopupError(string message)
        {
            ClearPopup();

            if (_popupTitle != null)
                _popupTitle.text = "Ошибка";

            if (_popupDescription != null)
                _popupDescription.text = message;

            if (_popupLoader != null)
                _popupLoader.SetActive(false);

            if (_popup != null)
                _popup.SetActive(true);
        }
    }
}