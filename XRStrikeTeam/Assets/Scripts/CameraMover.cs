using Accenture.eviola;
using Accenture.eviola.Animation;
using Accenture.eviola.Async;
using Accenture.eviola.Math;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.PlayerSettings;

namespace Accenture.XRStrikeTeam.Presentation
{
    public class CameraMover : MonoBehaviour
    {
        public enum CameraState { 
            ON_TARGET,
            IN_TRAJECTORY,
            ROTATING_TO_TARGET
        }

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
        private CameraState _curState = CameraState.ON_TARGET;

        #region Animation

        public void Go(Vector3 pos, Quaternion rot, Trajectory trajectory = null) { 
            StartTrajectory(pos, rot, trajectory);
        }

        public void SetCamera(Vector3 pos, Quaternion rot, bool instant = true) {
            _targetPose.position = pos;
            _targetPose.rotation = rot;
            _srcPose.position = _camera.transform.position;
            _srcPose.rotation = _camera.transform.rotation;
            _fromTime = Time.time;
            _toTime = _fromTime + _easingTime;
            if (instant)
            {
                SetOnTarget(pos, rot);
            }
            else {
                switch (State) {
                    case CameraState.IN_TRAJECTORY:
                        SetOnTarget(pos, _srcPose.rotation);
                        StartRotateToTarget();
                        break;
                    case CameraState.ROTATING_TO_TARGET:
                        
                        break;
                    case CameraState.ON_TARGET:
                    default:
                        SetOnTarget(pos, rot);
                        break;
                }
            }
        }

        private void LoadTrajectory(Trajectory traj)
        {
            _movement.MovementEasing = traj.MovementEasing;
            _movement.Duration = traj.Duration;
            _movement.Waypoints.Add(_startPose);
            if (traj.NumWaypoints > 2)
            {
                for (int i = 1; i < traj.NumWaypoints - 1; i++)
                {
                    Vector3 pos = traj.GetWayPoint(i);
                    _movement.Waypoints.Add(new Pose(pos, eviola.Math.Vector.LookAt(pos, traj.GetWayPoint(i + 1))));
                }
            }
            _movement.Waypoints.Add(_endPose);
        }

        private void HandleMovementDone(Timer.TimerStopType tst) {
            if (tst == Timer.TimerStopType.MANUAL) return;
            StartRotateToTarget();
            OnDestinationReached.Invoke(_endPose);
        }

        #endregion

        #region stateHandling
        
        public CameraState State { get { return _curState; } }

        public bool IsMoving() { return State == CameraState.IN_TRAJECTORY; }

        private void SetOnTarget(Vector3 pos, Quaternion rot) { 
            _curState = CameraState.ON_TARGET;
            _movement.Stop();
            _camera.transform.position = pos;
            _camera.transform.rotation = rot;
        }

        private void StartTrajectory(Vector3 posDest, Quaternion rotDest, Trajectory traj=null) {
            _movement.Stop();
            _curState = CameraState.IN_TRAJECTORY;

            _startPose.position = _camera.transform.position;
            _startPose.rotation = _camera.transform.rotation;

            _endPose.position = posDest;
            _endPose.rotation = rotDest;

            _movement.AnimatedTransform = _camera.transform;
            _movement.Waypoints.Clear();
            if (traj == null)
            {
                _movement.Duration = _travelTime;
                _movement.MovementEasing = Easing.LINEAR;
                _movement.Waypoints.Add(_startPose);
                _movement.Waypoints.Add(_endPose);
            }
            else { 
                LoadTrajectory(traj);
            }
            _movement.Start();
        }

        private void StartRotateToTarget() {
            _movement.Stop();
            _curState = CameraState.ROTATING_TO_TARGET;

            _fromTime = Time.time;
            _toTime = _fromTime + _easingTime;

            _targetPose.position = _endPose.position;
            _targetPose.rotation = _endPose.rotation;
        }

        private void UpdateRotateToTarget() {
            if (Time.time > _toTime) {
                SetOnTarget(_endPose.position, _endPose.rotation);
                return;
            }
            _camera.transform.rotation = Quaternion.Slerp(_srcPose.rotation, _targetPose.rotation, Remap.Map(Time.time, _fromTime, _toTime, 0, 1));
        }

        private void UpdateState() {
            switch (State) {
                case CameraState.IN_TRAJECTORY:
                    _movement.Update();
                    break;
                case CameraState.ROTATING_TO_TARGET:
                    UpdateRotateToTarget();
                    break;
                case CameraState.ON_TARGET:
                default:
                    break;
            }
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
            UpdateState();
        }
        #endregion
    }
}