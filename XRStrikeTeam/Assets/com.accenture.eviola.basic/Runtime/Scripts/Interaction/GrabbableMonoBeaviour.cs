using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Accenture.eviola.Interaction
{
    public class GrabbableMonoBeaviour : MonoBehaviour, IGrabbable
    {
        protected InteractionEvent _onGrabStateChanged = new InteractionEvent();
        protected bool _bGrabbed = false;

        public InteractionEvent OnGrabStateChanged => _onGrabStateChanged;

        public bool IsBeingGrabbed()
        {
            return _bGrabbed;
        }
    }
}