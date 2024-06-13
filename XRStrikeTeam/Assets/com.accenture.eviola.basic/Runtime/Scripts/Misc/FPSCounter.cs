using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Accenture.eviola
{

    public class FPSCounter : MonoBehaviour
    {
        public UnityEvent OnCountingStarted;
        public UnityEvent OnCountingStopped;
        public Events.FloatEvent OnFpsUpdate;
        
        [Header("Options")]
        public bool StartOnEnable = true;
        public float SampleInterval = 0.1f;

        private float _fps = 0;
        private bool _bRunning = false;

        #region Controls
        public void StartCounting() {
            if (IsRunning()) return;
            _fps = 0;
            _bRunning = true;
            OnCountingStarted.Invoke();
            StartCoroutine(CountFps());
        }

        public void StopCounting() {
            if (!IsRunning()) return;
            _bRunning = false;
            OnCountingStopped.Invoke();
            StopCoroutine(CountFps());
        }

        public bool IsRunning() { return _bRunning; }

        public float GetFps() { return _fps; }
        #endregion

        #region Count

        private IEnumerator CountFps() {
            while (_bRunning) {
                _fps = 1.0f / Time.unscaledDeltaTime;
                OnFpsUpdate.Invoke(_fps);
                yield return new WaitForSeconds(SampleInterval);
            }            
        }
        
        #endregion

        #region MonoBehaviour

        private void OnEnable()
        {
            if (StartOnEnable) StartCounting();
        }

        private void OnDisable()
        {
            StopCounting();
        }

        #endregion
    }

}