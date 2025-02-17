using Accenture.eviola;
using Accenture.eviola.Async;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

        public Trajectory GetTrajectoryToGetHere(StepJumpType jt=StepJumpType.FORWARD) {
            switch (jt) {
                case StepJumpType.FORWARD:
                    if (Id < 1) return null;
                    Destination prevDestination = Controller.GetStep(Id - 1);
                    if (prevDestination == null) return null;
                    return prevDestination.NextTrajectory;
                case StepJumpType.BACKWARDS:
                    return null;
                    break;
                case StepJumpType.JUMP:
                default:
                    return null;
            }
        }

        public void Leave() {
            if (_slideAnimatorController == null)
            {
                _activatedPayload.SetActive(false);
            }
            else {
                _slideAnimatorController.EaseOut();
                _delayedPayloadDeactivation.Abort();
                _delayedPayloadDeactivation.Fire();
            }
        }

        public void Enter(bool instantaneous=false) {
            if (_bLeaveOtherDestinationOnEnter) HandleOtherDestinationDelayedLeave();
            Controller.CameraDriver.SetCamera(_cameraSocket.position, _cameraSocket.rotation, instantaneous);
            if (_autoAdvance) { 
                Controller.NextStep();
            }
        }

        public void Go(StepJumpType jt=StepJumpType.FORWARD, Trajectory traj=null) {
            Trajectory trajectory = null;
            switch (jt) {
                case StepJumpType.CUSTOM:
                    trajectory = traj;
                    break;
                default:
                    trajectory = GetTrajectoryToGetHere(jt);
                    break;
            }
            
            HandleOtherDestinationLeave(trajectory, jt);
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

        private void HandleOtherDestinationLeave(Trajectory traj, StepJumpType jt) {
            switch (jt) {
                case StepJumpType.FORWARD:
                    HandlePreviousDestinationLeave(traj);
                    break;
                case StepJumpType.CUSTOM:
                    if (traj.IsTrackingStepIndices())
                    {
                        HandleOtherDestinationLeave(traj.IdxFrom, traj);
                    }
                    else {
                        HandlePreviousDestinationLeave(traj);
                    }
                    break;
                default:
                    break;
            }
        }

        private void HandlePreviousDestinationLeave(Trajectory traj) { HandleOtherDestinationLeave(Id -1, traj); }

        private void HandleOtherDestinationLeave(int otherDestId, Trajectory traj) {
            _bLeaveOtherDestinationOnEnter = false;
            _otherDestination = Controller.GetStep(otherDestId);
            if (_otherDestination == null) return;

            if (traj == null)
            {
                _bLeaveOtherDestinationOnEnter = true;
            }
            else if (traj.DelayEaseOut <= 0)
            {
                HandleOtherDestinationDelayedLeave();
            }
            else {
                _delayedLeave.Abort();
                _delayedLeave.Delay = traj.DelayEaseOut;
                _delayedLeave.Fire();
            }
        }

        /*private void HandlePreviourDestinationLeave(Trajectory traj) {
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
        }*/

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

#if UNITY_EDITOR
    [CustomEditor(typeof(Destination))]
    public class DestinationEditor : Editor { 
        private Destination _target { get { return (Destination)target; } }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorUI.Button("Enter", () => { _target.Enter(); });
            EditorUI.Button("Leave", () => { _target.Leave(); });
        }
    }
#endif
}