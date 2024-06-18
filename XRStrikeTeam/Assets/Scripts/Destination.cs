using Accenture.eviola;
using Accenture.eviola.Async;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Accenture.XRStrikeTeam.Presentation
{
    public class Destination : MonoBehaviour
    {
        [Header("ExternalComponents")]
        [SerializeField]
        private Transform _cameraSocket = null;
        [SerializeField]
        private GameObject _activatedPayload = null;
        [SerializeField]
        public Trajectory NextTrajectory = null;
        [SerializeField]
        public SlideAnimatorController _slideAnimatorController = null;
        [Header("Options")]
        [SerializeField]
        private bool _autoAdvance = false;
        
        [HideInInspector]
        public StepController Controller = null;
        [HideInInspector]
        public int Id = -1;

        private DelayedAction _delayedPayloadActivation = null;
        private DelayedAction _delayedPayloadDeactivation = null;
        private Destination _otherDestination = null;
        private DelayedAction _delayedLeave = null;
        private bool _bCameraMoverListener = false;
        private bool _bLeaveOtherDestinationOnEnter = false;

        public Transform PayloadContainer { get { return _activatedPayload.transform; } }
        public Transform CameraSocket { get { return _cameraSocket; } }

        #region Init

        public void PointCameraAtPayload() {
            _cameraSocket.position = PayloadContainer.position - (PayloadContainer.forward * 2);
            _cameraSocket.LookAt(PayloadContainer);
        }

        public void TogglePayloadVisibility() { 
            bool b = PayloadContainer.gameObject.activeSelf;
            PayloadContainer.gameObject.SetActive(!b);
        }

        public void TryLinkAnimationController() {
            if (_slideAnimatorController != null) return;
            if (PayloadContainer.childCount < 1) return;

            _slideAnimatorController = PayloadContainer.gameObject.GetComponentInChildren<SlideAnimatorController>();
        }

        #endregion

        #region Transitioning

        public Trajectory GetTrajectoryToGetHere() {
            if (Id < 1) return null;
            Destination prevDestination = Controller.GetStep(Id - 1);
            if (prevDestination == null) return null;
            return prevDestination.NextTrajectory;
        }

        public void Leave() {
            if (_slideAnimatorController == null)
            {
                _activatedPayload.SetActive(false);
            }
            else {
                _slideAnimatorController.EaseOut();
                _delayedPayloadDeactivation.Abort();
                _delayedPayloadActivation.Fire();
            }
        }

        public void Enter() {
            if (_bLeaveOtherDestinationOnEnter) HandleOtherDestinationDelayedLeave();
            Controller.CameraDriver.SetCamera(_cameraSocket.position, _cameraSocket.rotation, true);
            if (_autoAdvance) { 
                Controller.NextStep();
            }
        }

        public void Go() {
            Trajectory trajectory = GetTrajectoryToGetHere();

            HandlePreviourDestinationLeave(trajectory);
            AddCameraMoverListener();
            
            Controller.CameraDriver.Go(_cameraSocket.position, _cameraSocket.rotation, trajectory);
            StartDelayedPayloadActivation(trajectory);
            
        }

        public bool IsTransitioning() { return Controller.CameraDriver.IsMoving(); }

        private void StartDelayedPayloadActivation(Trajectory traj) {
            if (traj == null) {
                HandlePayloadActivation();
                return;
            }
            if (traj.DelayEaseIn <= 0) {
                HandlePayloadActivation();
                return;
            }
            _delayedPayloadActivation.Abort();
            _delayedPayloadActivation.Delay = traj.DelayEaseIn;
            _delayedPayloadActivation.Fire();
        }

        private void HandlePreviourDestinationLeave(Trajectory traj) {
            _bLeaveOtherDestinationOnEnter = false;
            int prevIdx = Id - 1;
            _otherDestination = Controller.GetStep(prevIdx);
            if (_otherDestination == null) return;
            if (traj == null)
            {
                _bLeaveOtherDestinationOnEnter = true;
            }
            else if (traj.DelayEaseOut <= 0) {
                HandleOtherDestinationDelayedLeave();
            }
            else
            {
                _delayedLeave.Abort();
                _delayedLeave.Delay = traj.DelayEaseOut;
                _delayedLeave.Fire();
            }
        }

        private void HandleOtherDestinationDelayedLeave() {
            if (_otherDestination == null) return;
            _otherDestination.Leave();
        }

        private void HandlePayloadActivation() {
            _activatedPayload.SetActive(true);
        }

        private void AddCameraMoverListener() {
            if (_bCameraMoverListener) return;
            if (Controller == null) return;
            Controller.CameraDriver.OnDestinationReached.AddListener(HandleDestinationReached);
            _bCameraMoverListener = true;
        }

        private void RemoveCameraMoverListener()
        {
            if (!_bCameraMoverListener) return;
            if (Controller == null) return;
            Controller.CameraDriver.OnDestinationReached.RemoveListener(HandleDestinationReached);
            _bCameraMoverListener = false;
        }

        private void HandleDestinationReached(Pose p) {
            RemoveCameraMoverListener();
            Enter();
            Controller.HandleDestinationReached(Id);
        }
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            Misc.CheckNotNull(_cameraSocket);

            TryLinkAnimationController();

            _delayedPayloadActivation = new DelayedAction(this, 0, HandlePayloadActivation);
            _delayedPayloadDeactivation = new DelayedAction(this, 1, () => { _activatedPayload.SetActive(false); });
            _delayedLeave = new DelayedAction(this, 1, HandleOtherDestinationDelayedLeave);
        }

        #endregion
    }
}