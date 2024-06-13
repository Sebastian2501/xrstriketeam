using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Accenture.eviola.Interaction
{
    public class PushableMonoBehaviour : MonoBehaviour, IPushable
    {
        protected InteractionEvent _onPushStateChanged =  new InteractionEvent();
        protected bool _bPushed = false;

        public InteractionEvent OnPushStateChanged => _onPushStateChanged;

        public bool IsBeingPushed()
        {
            return _bPushed;
        }
    }
}