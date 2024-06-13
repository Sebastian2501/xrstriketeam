using Accenture.eviola.DesignPatterns.DI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Accenture.eviola.Interaction
{
    public abstract class MultiplatformInteractableComponentInjector<A,R> : MonoBehaviour where R:class
    {
        abstract protected ConditionalInjection<RuntimePlatform, A, R> _conditionalInjector { get; set; }

         abstract protected void ApplyPlatformCondition(R arg);

        virtual protected void InitForPlatform() {
            if (_conditionalInjector == null) return;
            ApplyPlatformCondition(_conditionalInjector.Inject(Application.platform));
        }

        private void Awake()
        {
            InitForPlatform();
        }
    }
}