using System;
using System.Collections;
using UnityEngine;

namespace Accenture.eviola.Async
{
    /// <summary>
    /// Fires an action after a stipulated delay in sec.
    /// Works by attaching a coroutine to a MonoBehaviour
    /// </summary>
    public class DelayedAction
    {
        public MonoBehaviour HostMonoBehaviour = null;
        public float Delay = 1.0f;
        public Action TheAction = null;

        private IEnumerator _theDelayedAction;
        private bool _bRunning = false;

        public DelayedAction() {
            InitCoroutine();
        }
        public DelayedAction(MonoBehaviour hostMonoBehaviour, float delay, Action doAfterDelay)
        {
            InitCoroutine();
            Setup(hostMonoBehaviour, delay, doAfterDelay);
        }

        public void Setup(MonoBehaviour hostMonoBehaviour, float delay, Action doAfterDelay) { 
            HostMonoBehaviour = hostMonoBehaviour;
            Delay = delay;
            TheAction = doAfterDelay;
        }

        /// <summary>
        /// returns true if the timer is running
        /// </summary>
        public bool IsRunning() { return _bRunning; }

        /// <summary>
        /// returns true if the instance was initialized correctly
        /// </summary>
        public bool IsReady() { 
            return HostMonoBehaviour != null;
        }

        /// <summary>
        /// interrupt any running timer and abort action execution
        /// </summary>
        public void Abort() {
            if (!IsRunning()) return;
            HostMonoBehaviour.StopCoroutine(_theDelayedAction);
            _bRunning = false;
        }

        /// <summary>
        /// start the timer
        /// </summary>
        public void Fire() {
            if (IsRunning()) Abort();
            if (HostMonoBehaviour == null) {
                Debug.LogError("No host MonoBehaviour");
                return;
            }
            if (_theDelayedAction == null) {
                Debug.LogError("Coroutine not setup");
                return;
            }
            _bRunning = true;
            InitCoroutine();
            HostMonoBehaviour.StartCoroutine(_theDelayedAction);
        }

        /// <summary>
        /// start the timer
        /// </summary>
        public void Fire(float delay, Action doAfterDelay) {
            Delay = delay;
            TheAction = doAfterDelay;
            Fire();
        }

        private void InitCoroutine() {
            _theDelayedAction = PerformDelayedAction();
        }

        private IEnumerator PerformDelayedAction() {
            yield return new WaitForSeconds(Delay);
            if (TheAction != null) TheAction();
            _bRunning = false;
        }
    }
}