using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;
using System;
using TMPro;

namespace ClickerTestTask.Features.Clicker.Views
{
    public sealed class ClickerView : MonoBehaviour
    {
        [Header("Button")]
        [SerializeField] private Button _clickButton;
        [SerializeField] private Transform _buttonTransform;

        [Header("Counters")]
        [SerializeField] private TMP_Text _currencyText;
        [SerializeField] private TMP_Text _energyText;

        [Header("Energy Bar")]
        [SerializeField] private Image _energyFill;

        [Header("Button Animation")]
        [SerializeField] private float _pressScale = 0.9f;
        [SerializeField] private float _animationDuration = 0.1f;

        [Header("Particles")]
        [SerializeField] private ParticleSystem _clickParticles;

        [Header("Floating Reward")]
        [SerializeField] private TMP_Text _floatingRewardPrefab;
        [SerializeField] private RectTransform _effectsRoot;
        [SerializeField] private float _rewardMoveDistance = 100f;
        [SerializeField] private float _rewardDuration = 0.7f;

        [Header("Sound")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _clickSound;

        [SerializeField, Range(0f, 1f)]
        private float _soundVolume = 0.5f;

        public event Action Clicked;

        private Tween _buttonTween;
        private Vector2 _lastClickPosition;

        private readonly List<TMP_Text> _activeRewards = new();

        private void Awake()
        {
            if (_clickButton == null)
            {
                Debug.LogError("ClickerView: Click Button не назначена!");
                return;
            }

            _clickButton.onClick.AddListener(OnClickButton);

            RectTransform buttonRect = _clickButton.GetComponent<RectTransform>();

            if (buttonRect != null)
            {
                Canvas canvas = _effectsRoot != null
                    ? _effectsRoot.GetComponentInParent<Canvas>()
                    : null;

                Camera camera = GetCanvasCamera(canvas);

                _lastClickPosition =
                    RectTransformUtility.WorldToScreenPoint(
                        camera,
                        buttonRect.position
                    );
            }
        }

        private void OnDestroy()
        {
            if (_clickButton != null)
                _clickButton.onClick.RemoveListener(OnClickButton);

            _buttonTween?.Kill();

            foreach (TMP_Text reward in _activeRewards)
            {
                if (reward == null)
                    continue;

                reward.DOKill();
                reward.rectTransform.DOKill();

                Destroy(reward.gameObject);
            }

            _activeRewards.Clear();
        }

        private void OnDisable()
        {
            _buttonTween?.Kill();

            if (_buttonTransform != null)
                _buttonTransform.localScale = Vector3.one;
        }

        public void SetClickPosition(Vector2 screenPosition) => _lastClickPosition = screenPosition;

        private void OnClickButton()
        {
            if (_clickButton == null || !_clickButton.interactable)
                return;

            Clicked?.Invoke();

            PlayButtonAnimation();
        }

        public void SetValues(int currency, int energy, int maxEnergy)
        {
            if (_currencyText != null)
                _currencyText.text = currency.ToString();

            if (_energyText != null)
                _energyText.text = $"{energy}/{maxEnergy}";

            if (_energyFill != null && maxEnergy > 0)
                _energyFill.fillAmount = (float)energy / maxEnergy;

            if (_clickButton != null)
                _clickButton.interactable = energy > 0;
        }

        private void PlayButtonAnimation()
        {
            if (_buttonTransform == null)
                return;

            _buttonTween?.Kill();

            _buttonTransform.localScale = Vector3.one;

            _buttonTween = _buttonTransform
                .DOScale(_pressScale, _animationDuration)
                .SetLoops(2, LoopType.Yoyo)
                .SetEase(Ease.OutQuad);
        }

        public void PlayRewardEffects(int amount)
        {
            PlayParticles();
            ShowFloatingReward(amount, _lastClickPosition);
            PlayClickSound();
        }

        private void PlayParticles()
        {
            if (_clickParticles == null)
                return;

            _clickParticles.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            _clickParticles.Play();
        }

        private void ShowFloatingReward(int amount, Vector2 screenPosition)
        {
            if (_floatingRewardPrefab == null || _effectsRoot == null)
                return;

            TMP_Text reward = Instantiate(
                _floatingRewardPrefab,
                _effectsRoot
            );

            reward.text = $"+{amount}";
            reward.gameObject.SetActive(true);

            RectTransform rewardRect = reward.rectTransform;

            RectTransform buttonRect =
                _clickButton.GetComponent<RectTransform>();

            float randomX = UnityEngine.Random.Range(
                -buttonRect.rect.width * 0.25f,
                buttonRect.rect.width * 0.25f
            );

            float randomY = UnityEngine.Random.Range(
                -buttonRect.rect.height * 0.2f,
                buttonRect.rect.height * 0.2f
            );

            Vector3 randomWorldPosition =
                buttonRect.TransformPoint(new Vector3(randomX, randomY, 0f));

            Canvas canvas = _effectsRoot.GetComponentInParent<Canvas>();
            Camera camera = GetCanvasCamera(canvas);

            Vector2 screenPoint =
                RectTransformUtility.WorldToScreenPoint(
                    camera,
                    randomWorldPosition
                );

            bool positionFound =
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _effectsRoot,
                    screenPoint,
                    camera,
                    out Vector2 localPosition
                );

            if (!positionFound)
            {
                Destroy(reward.gameObject);
                return;
            }

            rewardRect.anchoredPosition = localPosition;
            rewardRect.localScale = Vector3.one;

            _activeRewards.Add(reward);

            Sequence sequence = DOTween.Sequence();

            sequence.Join(
                rewardRect.DOAnchorPosY(
                    rewardRect.anchoredPosition.y + _rewardMoveDistance,
                    _rewardDuration
                ).SetEase(Ease.OutCubic)
            );

            sequence.Join(
                reward.DOFade(0f, _rewardDuration)
            );

            sequence.OnComplete(() =>
            {
                _activeRewards.Remove(reward);

                if (reward != null)
                    Destroy(reward.gameObject);
            });
        }

        private Camera GetCanvasCamera(Canvas canvas)
        {
            if (canvas == null)
                return null;

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return null;

            return canvas.worldCamera;
        }

        private void PlayClickSound()
        {
            if (_audioSource == null || _clickSound == null)
                return;

            _audioSource.PlayOneShot(
                _clickSound,
                _soundVolume
            );
        }
    }
}