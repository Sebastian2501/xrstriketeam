using Accenture.eviola.Async;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Accenture.eviola.Animation
{
    public class LinearTimedMovement : Timer
    {
        public Transform AnimatedTransform = null;
        
        private Pose _startPose = new Pose();
        private Pose _endPose = new Pose();
        private Pose _curPose = new Pose();

        public void SetStartPose(Vector3 pos, Quaternion rot) {
            _startPose.position = pos; 
            _startPose.rotation = rot;
        }

        public void SetEndPose(Vector3 pos, Quaternion rot) { 
            _endPose.position = pos;
            _endPose.rotation = rot;
        }

        public Pose GetCurrentPose() {
            if (AnimatedTransform == null)
            {
                if (IsRunning())
                {
                    return _curPose;
                }
                else
                {
                    return new Pose();
                }
            }
            else
            {
                return new Pose(AnimatedTransform.position, AnimatedTransform.rotation);
            }
        }

        protected void UpdateAnimatedTransform() {
            if (AnimatedTransform == null) return;
            AnimatedTransform.position = _curPose.position;
            AnimatedTransform.rotation = _curPose.rotation;
        }

        public override void Start()
        {
            _curPose.position = _startPose.position;
            _curPose.rotation = _startPose.rotation;
            UpdateAnimatedTransform();
            base.Start();
        }

        protected override void Stop(TimerStopType tst)
        {
            _curPose.position = _endPose.position;
            _curPose.rotation= _endPose.rotation;
            UpdateAnimatedTransform();
            base.Stop(tst);
        }

        public override void Update()
        {
            base.Update();
            if (!IsRunning()) return;
            float pct = GetPct();
            _curPose.position.x = Math.Remap.Map(pct, 0, 1, _startPose.position.x, _endPose.position.x);
            _curPose.position.y = Math.Remap.Map(pct, 0, 1, _startPose.position.y, _endPose.position.y);
            _curPose.position.z = Math.Remap.Map(pct, 0, 1, _startPose.position.z, _endPose.position.z);
            _curPose.rotation = Quaternion.Slerp(_startPose.rotation, _endPose.rotation, pct);
            UpdateAnimatedTransform();
        }
    }

    public class WaypointsTimedMovement {
        public UnityEvent OnMovementCompleted = new UnityEvent();
        public float Duration = 1;
        public Transform AnimatedTransform = null;

        private LinearTimedMovement _movement = new LinearTimedMovement();
        private List<Pose> _waypoints = new List<Pose>();
        private int _curWaypoint = -1;

        private int _lastSegment { get { return _waypoints.Count-2; } }

        private int _numSegments { get { return _waypoints.Count - 1; } }

        public WaypointsTimedMovement() {
            _movement.OnStop.AddListener(HandleTimer);
        }

        public void ClearWaypoints() { _waypoints.Clear(); }

        public void AddWayPoint(Pose wp) { _waypoints.Add(new Pose(wp.position, wp.rotation)); }

        public bool IsMoving() { return _movement.IsRunning(); }

        public void Stop() {
            if (!IsMoving()) return;
            _movement.Stop();
        }

        public void Start() {
            if (IsMoving()) return;
            if (_waypoints.Count < 2) return;
            StartSegment(0);
        }

        private void StartSegment(int idx) {
            if (idx < 0 || idx > _lastSegment)
            {
                Stop();
                return;
            }
            _curWaypoint = idx;
            _movement.Stop();
            _movement.Duration = Duration /(float)_numSegments;
            _movement.AnimatedTransform = AnimatedTransform;
            _movement.SetStartPose(_waypoints[idx].position, _waypoints[idx].rotation);
            _movement.SetEndPose(_waypoints[idx+1].position, _waypoints[idx+1].rotation);
            _movement.Start();
        }

        public void Update()
        {
            _movement.Update();
            //if (UnityEngine.Input.GetKeyDown("q")) {
            //    StartSegment(_curWaypoint + 1);
            //}
        }

        private void HandleTimer(Timer.TimerStopType tst) {
            if (tst == Timer.TimerStopType.NATURAL) {
                if (_curWaypoint < _lastSegment)
                {
                    StartSegment(_curWaypoint + 1);
                }
                else {
                    OnMovementCompleted.Invoke();
                }
            }
        }
    }
}