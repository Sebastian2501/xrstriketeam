using Accenture.eviola;
using Accenture.eviola.Animation;
using Accenture.eviola.Async;
using System;
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
            _movement.AnimatedTransform = _camera.transform;
            _movement.Waypoints.Clear();

            if (trajectory == null)
            {
                _movement.Duration = _travelTime;
                _movement.Waypoints.Add(_startPose);
                _movement.Waypoints.Add(_endPose);
            }
            else { 
                LoadTrajectory(trajectory);
            }
            
            _movement.Start();
        }

        private void LoadTrajectory(Trajectory traj) {
            _movement.Duration = traj.GetTravelTime();
            _movement.Waypoints.Add(_startPose);
            if (traj.NumWaypoints > 2) {
                for (int i = 1; i < traj.NumWaypoints - 1; i++) {
                    Vector3 pos = traj.GetWayPoint(i);
                    _movement.Waypoints.Add(new Pose(pos, eviola.Math.Vector.LookAt(pos, traj.GetWayPoint(i+1))));
                }
            }
            _movement.Waypoints.Add(_endPose);
        }

        public void StopCamera() { _movement.Stop(); }

        public void SetCamera(Vector3 pos, Quaternion rot, bool stopAnim=false) { 
            if(stopAnim)StopCamera();
            _camera.transform.position = pos;
            _camera.transform.rotation = rot;
        }

        public bool IsMoving() { return _movement.IsRunning(); }

        private void UpdateMovement() { 
            _movement.Update();
        }

        private void HandleMovementDone(Timer.TimerStopType tst) {
            if (tst == Timer.TimerStopType.MANUAL) return;
            OnDestinationReached.Invoke(_endPose);
        }


        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            Misc.CheckNotNull(_camera);
            _movement.OnStop.AddListener(HandleMovementDone);
        }

        private void Update()
        {
            UpdateMovement();
        }
        #endregion
    }
}