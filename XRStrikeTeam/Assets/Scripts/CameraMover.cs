using Accenture.eviola;
using Accenture.eviola.Animation;
using Accenture.eviola.Async;
using Accenture.eviola.Math;
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
        [SerializeField]
        private float _easingTime = 0.3f;

        private Pose _startPose = new Pose();
        private Pose _endPose = new Pose();
        private Pose _targetPose = new Pose();
        private Pose _srcPose = new Pose();
        private float _fromTime = 0;
        private float _toTime = 0;
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
                _movement.MovementEasing = Easing.LINEAR;
                _movement.Waypoints.Add(_startPose);
                _movement.Waypoints.Add(_endPose);
            }
            else { 
                LoadTrajectory(trajectory);
            }
            
            _movement.Start();
        }

        private void LoadTrajectory(Trajectory traj) {
            _movement.MovementEasing = traj.MovementEasing;
            _movement.Duration = traj.Duration;
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

        public void SetCamera(Vector3 pos, Quaternion rot, bool instant = true)
        {
            StopCamera();
            _targetPose.position = pos;
            _targetPose.rotation = rot;
            _srcPose.position = _camera.transform.position;
            _srcPose.rotation = _camera.transform.rotation;
            _fromTime = Time.time;
            _toTime = _fromTime+_easingTime;
            if (instant)
            {
                _camera.transform.position = pos;
                _camera.transform.rotation = rot;
            }
        }

        public bool IsMoving() { return _movement.IsRunning(); }

        private void UpdateMovement() { 
            _movement.Update();
        }

        private void HandleMovementDone(Timer.TimerStopType tst) {
            if (tst == Timer.TimerStopType.MANUAL) return;
            _targetPose.position = _endPose.position;
            _targetPose.rotation = _endPose.rotation;
            OnDestinationReached.Invoke(_endPose);
        }

        private void UpdateTarget() {
            if (_movement.IsRunning()) return;
            if (Time.time > _toTime) return;
            _camera.transform.rotation = Quaternion.Slerp(_srcPose.rotation, _targetPose.rotation, Remap.Map(Time.time, _fromTime, _toTime, 0,1));
        }

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            Misc.CheckNotNull(_camera);

            _movement.LerpRotationForSingleWaypoints = false;
            _movement.StartEnforcementRules.Set(true, false);
            _movement.EndEnforcementRules.Set(true, false);
            _movement.OnStop.AddListener(HandleMovementDone);
        }

        private void Update()
        {
            UpdateMovement();
            UpdateTarget();
        }
        #endregion
    }
}