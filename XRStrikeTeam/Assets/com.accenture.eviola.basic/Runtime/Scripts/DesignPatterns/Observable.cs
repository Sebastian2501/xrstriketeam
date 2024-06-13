using Accenture.eviola.Async;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Accenture.eviola.DesignPatterns
{
    /// <summary>
    /// compact and snappy Observable pattern implementation
    /// </summary>
    public class Observable<T>
    {
        public UnityEvent<T> OnChange = new UnityEvent<T>();
        private T _value = default(T);

        public T Value { 
            get { 
                return _value; 
            } 
            
            set {  
                SetValue(value);
            } 
        }

        public void SetValue(T value) {
            T oldValue = _value;
            _value = value;
            if (!GenericEquals(oldValue, _value)) {
                OnChange.Invoke(_value);
            }
        }

        private bool GenericEquals(T a, T b) {
            if (a == null || b == null)
            {
                return a == null && b == null;
            }
            else { 
                return a.Equals(b);
            }
        }

        public static implicit operator T(Observable<T> o) => o.Value;
    }

    /// <summary>
    /// class allowing to observe a condition, until it happens or fails
    /// </summary>
    public class ObservableCondition { 
        public Func<bool> Condition { get; set; }
        public UnityEvent OnConditionMet = new UnityEvent();
        public UnityEvent OnFailed = new UnityEvent();

        private DelayedAction _delayedAction;
        private float _checkDelay = 0.5f;
        private int _maxTries = -1;
        private int _curTry = 0;

        public ObservableCondition(MonoBehaviour mb, Func<bool> condition) {
            _delayedAction = new DelayedAction(mb, _checkDelay, () => { HandleDelayedAction(); });
            Condition = condition;
        }

        /// <summary>
        /// set how often we check the condition (in sec)
        /// </summary>
        public void SetCheckFrequency(float sec) {
            _checkDelay = sec;
            if (_checkDelay < 0.05f) _checkDelay = 0.05f;
        }

        /// <summary>
        /// set the max number of tries before we throw the OnFailed event; -1 to never fail
        /// </summary>
        /// <param name="maxTries"></param>
        public void SetMaxTries(int maxTries) { 
            _maxTries = maxTries;
        }

        public bool IsObserving() {
            if (_delayedAction == null) return false;
            return _delayedAction.IsRunning();
        }

        public void StartObserving() {
            if (IsObserving()) return;
            _curTry = 0;
            _delayedAction.Delay = _checkDelay;
            _delayedAction.Fire();
        }

        private void HandleDelayedAction() {
            if (Condition != null) {
                if (Condition()) {
                    OnConditionMet.Invoke();
                    return;
                }
            }
            if (_maxTries > 0) {
                if (_curTry >= _maxTries) { 
                    OnFailed.Invoke();
                    return;
                }
                _curTry++;
            }
            _delayedAction.Delay = _checkDelay;
            _delayedAction.Fire();
        }
    }
}