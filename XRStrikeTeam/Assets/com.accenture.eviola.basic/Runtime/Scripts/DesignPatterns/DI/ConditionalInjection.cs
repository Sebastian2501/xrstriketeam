using System;
using System.Collections.Generic;
using UnityEngine;

namespace Accenture.eviola.DesignPatterns.DI
{
    public abstract class ConditionalInjection<C,A, R> where R:class
    {
        public Dictionary<C, A> ConditionArgPairs = new Dictionary<C, A>();

        protected abstract R ApplyCondition(A arg);

        public R Inject(C condition) {
            if (!ConditionArgPairs.ContainsKey(condition)) return null;
            return ApplyCondition(ConditionArgPairs[condition]);
        }
    }

    public class ConditionalActivationInjection<C> : ConditionalInjection<C, GameObject, GameObject> {
        protected override GameObject ApplyCondition(GameObject arg)
        {
            foreach (var pair in ConditionArgPairs)
            {
                if (pair.Value != arg) {
                    arg.SetActive(false);
                }
            }
            if (arg == null) return null;
            arg.SetActive(true);
            return arg;
        }
    }

    public class ConditionalPrefabInjection<C> : ConditionalInjection<C, GameObject, GameObject> {
        public Transform InjectionTarget = null;

        protected override GameObject ApplyCondition(GameObject arg)
        {
            if (arg == null) return null;
            return MonoBehaviour.Instantiate(arg, InjectionTarget);
        }
    }

    public class ConditionalComponentInjection<C> : ConditionalInjection<C, Type, Component> {
        public Transform InjectionTarget = null;

        protected override Component ApplyCondition(Type arg)
        {
            if (InjectionTarget == null) return null;
            if (arg == null) return null;
            return InjectionTarget.gameObject.AddComponent(arg);
        }
    }

    public class PlatformDependentActivationInjection : ConditionalActivationInjection<RuntimePlatform> { }
    public class PlatformDependentPrefabInjection : ConditionalPrefabInjection<RuntimePlatform> { }
    public class PlatformDependentComponentInjection : ConditionalComponentInjection<RuntimePlatform> { }
}