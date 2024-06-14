using Accenture.eviola;
using Accenture.eviola.Animation;
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
        private WaypointsTimedMovement _movement = new WaypointsTimedMovement();

        #region Animation

        public void Go(Vector3 pos, Quaternion rot, Trajectory trajectory = null) {
            if (IsMoving()) return;
            StopCamera();

            _startPose.position = _camera.transform.position;
            _startPose.rotation = _camera.transform.rotation;

            _endPose.position = pos;
            _endPose.rotation = rot;

            _movement.Stop();
            _movement.Duration = _travelTime;
            _movement.AnimatedTransform = _camera.transform;
            _movement.ClearWaypoints();

            _movement.AddWayPoint(_startPose);
            _movement.AddWayPoint(_endPose);

            _movement.Start();
        }

        public void StopCamera() { _movement.Stop(); }

        public void SetCamera(Vector3 pos, Quaternion rot, bool stopAnim=false) { 
            if(stopAnim)StopCamera();
            _camera.transform.position = pos;
            _camera.transform.rotation = rot;
        }

        public bool IsMoving() { return _movement.IsMoving(); }

        private void UpdateMovement() { 
            _movement.Update();
        }

        private void HandleMovementDone() {
            OnDestinationReached.Invoke(_endPose);
        }


        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            Misc.CheckNotNull(_camera);
            _movement.OnMovementCompleted.AddListener(HandleMovementDone);
        }

        private void Update()
        {
            UpdateMovement();
        }
        #endregion
    }
}