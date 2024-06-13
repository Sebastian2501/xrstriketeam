using Accenture.eviola.DesignPatterns.DI;
using System;
using UnityEngine;

namespace Accenture.eviola.Interaction
{
    public class MultiplatformPushable : MultiplatformInteractableComponentInjector<Type, Component>, IPushable
    {
        protected InteractionEvent _onPushStateChanged = new InteractionEvent();
        protected IPushable _pushable = null;
        protected PlatformDependentComponentInjection _injector = new PlatformDependentComponentInjection();

        #region Pushable

        public InteractionEvent OnPushStateChanged => _onPushStateChanged;

        public bool IsBeingPushed()
        {
            if (_pushable == null) return false;
            return _pushable.IsBeingPushed();
        }

        #endregion

        #region inheritance

        protected override ConditionalInjection<RuntimePlatform, Type, Component> _conditionalInjector { 
            get => _injector; 
            set => value = _injector; 
        }


        protected override void ApplyPlatformCondition(Component arg)
        {
            if (arg == null) return;
            _pushable = (PushableMonoBehaviour)arg;
            if (_pushable == null) return;
            _pushable.OnPushStateChanged.AddListener((InteractionArgs args) => { _onPushStateChanged.Invoke(args); });
        }

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            _injector.InjectionTarget = transform;
            InitForPlatform();
        }

        #endregion
    }
}