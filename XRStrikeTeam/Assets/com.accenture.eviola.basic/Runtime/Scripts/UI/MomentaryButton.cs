using Accenture.eviola.Events;
using Accenture.eviola.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Accenture.eviola.UI
{
    public class MomentaryButton : PushableMonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPushable
    {
        #region IPointerHandler

        public void OnPointerDown(PointerEventData eventData)
        {
            _bPushed = true;
            _onPushStateChanged.Invoke(new InteractionArgs()
            {
                IsInteracting = true,
                InteractingObject = null
            });
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _bPushed = false;
            _onPushStateChanged.Invoke(new InteractionArgs()
            {
                IsInteracting = false,
                InteractingObject = null
            });
        }

        #endregion



        #region MonoBehaviour

        private void OnEnable()
        {
            _bPushed = false;
        }

        private void OnDisable()
        {
            if (_bPushed) {
                _onPushStateChanged.Invoke(new InteractionArgs()
                {
                    IsInteracting = false,
                    InteractingObject = null
                });
            }
            _bPushed = false;
        }

        #endregion
    }
}