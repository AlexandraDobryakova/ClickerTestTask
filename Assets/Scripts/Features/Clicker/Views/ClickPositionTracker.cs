using UnityEngine.EventSystems;
using UnityEngine;

namespace ClickerTestTask.Features.Clicker.Views
{
    public sealed class ClickPositionTracker : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private ClickerView _clickerView;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (_clickerView == null)
                return;

            _clickerView.SetClickPosition(eventData.position);
        }
    }
}