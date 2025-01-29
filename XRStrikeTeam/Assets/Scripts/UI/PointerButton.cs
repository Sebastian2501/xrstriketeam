using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Accenture.XRStrikeTeam.Presentation.UI
{
    public class PointerButton : Button
    {
        public UnityEvent OnPointerClick = new UnityEvent();

        public override void OnPointerDown(PointerEventData eventData) { 
            base.OnPointerDown(eventData);
            OnPointerClick.Invoke();
        }
    }
}