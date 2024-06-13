using Accenture.eviola;
using Accenture.eviola.Async;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Accenture.XRStrikeTeam.Presentation
{
    public class CameraMover : MonoBehaviour
    {
        public UnityEvent<Pose> OnDestinationReached = new UnityEvent<Pose>();

        [Header("ExternalComponents")]
        [SerializeField]
        private Camera _camera;
        [Header("Options")]
        [SerializeField]
        private float _travelTime = 1;

        private Pose _startPose = new Pose();
        private Pose _endPose = new Pose();
        private Timer _timer = new Timer();

        #region Animation

        public void Go(Vector3 pos, Quaternion rot) {
            if (IsMoving()) return;
            StopCamera();

            _startPose.position = _camera.transform.position;
            _startPose.rotation = _camera.transform.rotation;

            _endPose.position = pos;
            _endPose.rotation = rot;

            _timer.Duration = _travelTime;
            _timer.Start();
        }

        public void StopCamera() { _timer.Stop(); }

        public void SetCamera(Vector3 pos, Quaternion rot, bool stopAnim=false) { 
            if(stopAnim)StopCamera();
            _camera.transform.position = pos;
            _camera.transform.rotation = rot;
        }

        public bool IsMoving() { return _timer.IsRunning(); }

        private void UpdateMovement() { 
            _timer.Update();
            if (!_timer.IsRunning()) return;
            float pct = _timer.GetPct();
            Vector3 pos = Vector3.zero;
            pos.x = eviola.Math.Remap.Map(pct, 0,1, _startPose.position.x, _endPose.position.x);
            pos.y = eviola.Math.Remap.Map(pct, 0, 1, _startPose.position.y, _endPose.position.y);
            pos.z = eviola.Math.Remap.Map(pct, 0, 1, _startPose.position.z, _endPose.position.z);
            Quaternion rot = Quaternion.Slerp(_startPose.rotation, _endPose.rotation, pct);
            SetCamera(pos, rot);
        }

        private void HandleTimer(Timer.TimerStopType tst) {
            SetCamera(_endPose.position, _endPose.rotation, false);
            OnDestinationReached.Invoke(_endPose);
        }

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            Misc.CheckNotNull(_camera);
            _timer.OnStop.AddListener(HandleTimer);
        }

        private void Update()
        {
            UpdateMovement();
        }
        #endregion
    }
}