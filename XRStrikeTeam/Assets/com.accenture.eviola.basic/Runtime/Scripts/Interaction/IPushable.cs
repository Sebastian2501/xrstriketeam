using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Accenture.eviola.Interaction
{
    public interface IPushable
    {
        bool IsBeingPushed();
        InteractionEvent OnPushStateChanged { get; }
    }
}
