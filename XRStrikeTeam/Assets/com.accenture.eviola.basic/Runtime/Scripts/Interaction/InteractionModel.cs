using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Accenture.eviola.Interaction {
    public class InteractionArgs {
        public bool IsInteracting = false;
        public UnityEngine.Object InteractingObject = null;
    }

    public class InteractionEvent : UnityEvent<InteractionArgs>{}
}