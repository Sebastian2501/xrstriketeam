using Accenture.eviola;
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
        
        [HideInInspector]
        public StepController Controller = null;
        [HideInInspector]
        public int Id = -1;

        private bool _bCameraMoverListener = false;

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

        #endregion

        #region Transitioning

        public Trajectory GetTrajectoryToGetHere() {
            if (Id < 1) return null;
            Destination prevDestination = Controller.GetStep(Id - 1);
            if (prevDestination == null) return null;
            return prevDestination.NextTrajectory;
        }

        public void Leave() { 
            _activatedPayload.SetActive(false);
        }

        public void Enter() {
            Controller.CameraDriver.SetCamera(_cameraSocket.position, _cameraSocket.rotation, true);

            _activatedPayload.SetActive(true);
        }

        public void Go() {
            AddCameraMoverListener();
            _activatedPayload.SetActive(true);
            Trajectory trajectory = GetTrajectoryToGetHere();
            Controller.CameraDriver.Go(_cameraSocket.position, _cameraSocket.rotation, trajectory);
            
        }

        public bool IsTransitioning() { return Controller.CameraDriver.IsMoving(); }

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
            
        }

        #endregion
    }
}