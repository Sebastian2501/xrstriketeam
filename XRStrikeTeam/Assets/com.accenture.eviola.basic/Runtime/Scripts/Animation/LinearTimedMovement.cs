using Accenture.eviola.Async;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Accenture.eviola.Animation
{
    public enum Easing { 
        LINEAR,
        QUADRATIC,
        CUBIC
    }

    public class LinearTimedMovement : Timer
    {
        public Transform AnimatedTransform = null;
        public bool EnfoceStartAndStopPoses = true;
        
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

        public void EnforceStartPose() {
            _curPose.position = _startPose.position;
            _curPose.rotation = _startPose.rotation;
            UpdateAnimatedTransform();
        }

        public void EnforceEndPose()
        {
            _curPose.position = _endPose.position;
            _curPose.rotation = _endPose.rotation;
            UpdateAnimatedTransform();
        }

        protected void UpdateAnimatedTransform() {
            if (AnimatedTransform == null) return;
            AnimatedTransform.position = _curPose.position;
            AnimatedTransform.rotation = _curPose.rotation;
        }

        public override void Start()
        {
            if (EnfoceStartAndStopPoses) EnforceStartPose();
            
            base.Start();
        }

        protected override void Stop(TimerStopType tst)
        {
            if (EnfoceStartAndStopPoses)EnforceEndPose();
            
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

    public class PoseEnforcementRules {
        public bool EnforcePosition = true;
        public bool EnforceRotation = true;
        public PoseEnforcementRules() { }
        public PoseEnforcementRules(bool pos, bool rot) { Set(pos, rot); }
        public void Set(bool pos, bool rot) { EnforcePosition = pos; EnforceRotation = rot; }
    }

    public class WaypointsTimedMovement : Timer {
        public Transform AnimatedTransform = null;
        public List<Pose> Waypoints = new List<Pose>();
        public bool LerpRotationForSingleWaypoints = true;
        public PoseEnforcementRules StartEnforcementRules = new PoseEnforcementRules(true, true);
        public PoseEnforcementRules EndEnforcementRules = new PoseEnforcementRules(true, true);
        public Easing MovementEasing = Easing.LINEAR;

        private Pose _curPose = new Pose();
        private float _wptPct = 1;

        private Pose _firstWaypoint{ get { return Waypoints[0]; } }
        private Pose _lastWaypoint { 
            get { 
                if(Waypoints==null)return new Pose();
                if(Waypoints.Count<1)return new Pose();
                return Waypoints[Waypoints.Count - 1]; 
            } 
        }

        public void UpdateAnimatedTransform(bool bPos=true, bool bRot=true) {
            if (AnimatedTransform == null) return;
            if(bPos)AnimatedTransform.position = _curPose.position;
            if(bRot)AnimatedTransform .rotation = _curPose.rotation;
        }

        public Pose GetCurrentPose() {
            if (AnimatedTransform != null) {
                return new Pose(AnimatedTransform.position, AnimatedTransform.rotation);
            }
            else
            {
                if (IsRunning()) {
                    return _curPose;
                }
                else{
                    return new Pose();
                }
            }
        }

        public override void Start()
        {
            if (Waypoints == null) return;
            if (Waypoints.Count < 2) return;

            _curPose.position = _firstWaypoint.position;
            _curPose.rotation = _firstWaypoint.rotation;
            UpdateAnimatedTransform(StartEnforcementRules.EnforcePosition, StartEnforcementRules.EnforceRotation);

            _wptPct = 1.0f/(float)Waypoints.Count;

            base.Start();
        }

        protected override void Stop(TimerStopType tst)
        {
            if (tst == TimerStopType.NATURAL)
            {
                _curPose.position = _lastWaypoint.position;
                _curPose.rotation = _lastWaypoint.rotation;
                UpdateAnimatedTransform(EndEnforcementRules.EnforcePosition, EndEnforcementRules.EnforceRotation);
            }
            base.Stop(tst);
        }

        public override void Update()
        {
            base.Update();
            if (!IsRunning()) return;

            float totPct = GetPct();
            switch (MovementEasing) {
                case Easing.CUBIC:
                    totPct = totPct * totPct * totPct;
                    break;
                case Easing.QUADRATIC:
                    totPct = totPct * totPct;
                    break;
                default: break;
            }
            int _curSegment = (int)(totPct / _wptPct);
            if (_curSegment >= Waypoints.Count-1) return;
            Pose wpFrom = GetStartWaypointForSegment(_curSegment);
            Pose wpTo = GetEndWaypointForSegment(_curSegment);
            float segPct = Math.Remap.Map(totPct % _wptPct, 0, _wptPct, 0, 1);
            _curPose.position = Vector3.Slerp(wpFrom.position, wpTo.position, segPct);

            if (LerpRotationForSingleWaypoints)
            {
                _curPose.rotation = Quaternion.Slerp(wpFrom.rotation, wpTo.rotation, segPct);
            }
            else
            {
                _curPose.rotation = Quaternion.Slerp(_firstWaypoint.rotation, _lastWaypoint.rotation, totPct);
            }
            UpdateAnimatedTransform();
        }


        private Pose GetStartWaypointForSegment(int seg) { return Waypoints[seg]; }
        private Pose GetEndWaypointForSegment(int seg) { return Waypoints[seg + 1]; }
    }
}