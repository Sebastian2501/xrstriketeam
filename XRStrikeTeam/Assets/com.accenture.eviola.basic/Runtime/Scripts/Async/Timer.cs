using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Accenture.eviola.Async
{
    public class Timer
    {
        public enum TimerStopType { 
            NATURAL,
            MANUAL
        }

        public UnityEvent<TimerStopType> OnStop = new UnityEvent<TimerStopType>();

        public float Duration = 1;
        
        private float _startTime = 0;
        private float _endTime = 0;
        private float _pct = 0;
        private  bool _bRunning = false;

        public bool IsRunning() { return _bRunning; }

        virtual public void Start() {
            if (IsRunning()) return;
            _bRunning = true;
            _startTime = Time.time;
            _endTime = _startTime + Duration;
        }

        public void Stop() {
            Stop(TimerStopType.MANUAL);
        }

        virtual protected void Stop(TimerStopType tst) {
            if (!IsRunning()) return;
            _bRunning = false;
            OnStop.Invoke(tst);
        }

        virtual public void Update() {
            if (!IsRunning()) return;
            float now = Time.time;
            if (now >= _endTime)
            {
                Stop(TimerStopType.NATURAL);
            }
            else {
                _pct = Math.Remap.Map(now, _startTime, _endTime, 0.0f, 1.0f);
            }
        }

        public float GetPct() {
            if (!IsRunning()) return 0;
            return _pct;
        }
    }
}