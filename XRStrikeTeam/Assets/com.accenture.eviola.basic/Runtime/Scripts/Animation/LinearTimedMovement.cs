using Accenture.eviola.Async;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}