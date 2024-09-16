using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accenture.eviola;
using UnityEngine.Events;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Accenture.XRStrikeTeam.Presentation
{
    public class StepController : MonoBehaviour
    {
        public UnityEvent<int> OnStepChange = new UnityEvent<int>();

        [Header("ExternalComponents")]
        [SerializeField]
        private CameraMover _cameraMover = null;
        [SerializeField]
        private Transform _stepsContainer = null;
        [SerializeField]
        private Transform _trajectoryContainer = null;
        [SerializeField]
        private GameObject _destinationPrefab = null;
        [SerializeField]
        private GameObject _trajectoryPrefab = null;
        [Header("Steps")]
        [SerializeField]
        private List<Destination> _steps = new List<Destination>();
        [SerializeField]
        private List<UrlVideoPlayer> _videos = new List<UrlVideoPlayer>();

        private int _curStep = -1;

        private Camera _camera;
        public Camera PovCamera { get { return _camera; } }
        public CameraMover CameraDriver { get { return _cameraMover; } }

        public int CurrentStep { get { return _curStep; } }

        #region Init
        public bool IsDestination(Transform tra) {
            return tra.GetComponent<Destination>() != null;
        }

#if UNITY_EDITOR
        public void MakeDestinationsFromStuffInSteps() {
            if (_stepsContainer.childCount < 1) return;

            List<Transform> tras = new List<Transform>();
            for (int i = 0; i < _stepsContainer.childCount; i++) {
                Transform tra = _stepsContainer.GetChild(i);
                if (!IsDestination(tra)) {
                    tras.Add(tra);
                }
            }

            foreach (Transform tra in tras) {
                Pose payloadPose = new Pose();
                payloadPose.position = tra.position;
                payloadPose.rotation = tra.rotation;
                GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(_destinationPrefab);
                go.transform.parent = _stepsContainer;
                go.name = _steps.Count + "_Step";
                Destination dst = go.GetComponent<Destination>();
                dst.PayloadContainer.position = payloadPose.position;
                dst.PayloadContainer.rotation = payloadPose.rotation;
                tra.parent = dst.PayloadContainer;
                _steps.Add(dst);
            }

            foreach (Destination step in _steps) {
                EditorUtility.SetDirty(step);
            }

            EditorUtility.SetDirty(this);
        }

        public void ExtractPayloads() {
            foreach (Destination step in _steps) {
                if (step.PayloadContainer.childCount > 0) {
                    step.PayloadContainer.GetChild(0).parent = _stepsContainer;
                }
            }
            _steps.Clear();
            EditorUtility.SetDirty(this);
        }

        public void MakeCamerasLookAtPayloads()
        {
            foreach (Destination step in _steps)
            {
                step.PointCameraAtPayload();
                EditorUtility.SetDirty(step);
            }
            EditorUtility.SetDirty(this);
        }

        public void MakeTrajectories() {
            if (_steps.Count < 1) return;
            for (int i = 0; i < _steps.Count - 1; i++) {
                int idxNxt = i + 1;
                GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(_trajectoryPrefab);
                go.transform.parent = _trajectoryContainer;
                go.name = i + "to" + idxNxt + "_trajectory";
                Trajectory trajectory = go.GetComponent<Trajectory>();
                if (trajectory != null)
                {
                    _steps[i].NextTrajectory = trajectory;
                    trajectory.SetTransformsAndMakeWaypoints(_steps[i].CameraSocket, _steps[idxNxt].CameraSocket);
                    EditorUtility.SetDirty(trajectory);
                    EditorUtility.SetDirty(_steps[i]);
                }
            }
        }

        public Trajectory MakeTrajectoryFromTo(int idxFrom, int idxTo) {
            if (!Misc.IsGoodIndex(idxFrom, _steps) || !Misc.IsGoodIndex(idxTo, _steps)) return null;

            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(_trajectoryPrefab);
            go.transform.parent = _trajectoryContainer;
            go.name = idxFrom + "to" + idxTo + "_trajectory";
            Trajectory trajectory = go.GetComponent<Trajectory>();
            if (trajectory != null)
            {
                trajectory.SetTransformsAndMakeWaypoints(_steps[idxFrom].CameraSocket, _steps[idxTo].CameraSocket);
                EditorUtility.SetDirty(trajectory);
            }
            return trajectory;
        }

        public void TryLinkStepAnimationControllers()
        {
            if (_steps.Count < 1) return;
            for (int i = 0; i < _steps.Count - 1; i++)
            {
                _steps[i].TryLinkAnimationController();
                EditorUtility.SetDirty(_steps[i]);
            }
            EditorUtility.SetDirty(this);
        }

        public void CollectVideos() {
            _videos.Clear();
            UrlVideoPlayer[] vids = _stepsContainer.GetComponentsInChildren<UrlVideoPlayer>();
            foreach (UrlVideoPlayer vid in vids) { _videos.Add(vid); }
            EditorUtility.SetDirty(this);
        }
#endif
        public void ToggleAllPayloadsVisibility() {
            foreach (Destination step in _steps)
            {
                step.TogglePayloadVisibility();
            }
        }

        public void SetAllPayloadsVisibility(bool b) {
            foreach (Destination step in _steps)
            {
                step.PayloadContainer.gameObject.SetActive(b);
            }
        }

        #endregion

        #region Steps

        public Destination GetStep(int idx) {
            if (!Misc.IsGoodIndex(idx, _steps)) return null;
            return _steps[idx];
        }

        private void InitSteps() {
            for (int i = 0; i < _steps.Count; i++) {
                _steps[i].Controller = this;
                _steps[i].Id = i;
            }
            foreach (var vid in _videos) { 
                vid.InitVideo();
            }
        }

        public void SetStep(int idx, bool instantaneous = false, StepJumpType jt=StepJumpType.JUMP, Trajectory traj = null) {
            if (!Misc.IsGoodIndex(idx, _steps)) return;
            if (idx == _curStep) return;
            if (instantaneous)
            {
                if (Misc.IsGoodIndex(_curStep, _steps)) _steps[_curStep].Leave();
            }
            _curStep = idx;
            OnStepChange.Invoke(_curStep);
            if (instantaneous)
            {
                _steps[_curStep].PayloadContainer.gameObject.SetActive(true);
                _steps[_curStep].Enter();
            }
            else
            {
                _steps[_curStep].Go(jt, traj);
            }
        }

        public void NextStep() { 
            int idx = _curStep+1;
            if (idx >= _steps.Count) return;
            SetStep(idx, false, StepJumpType.FORWARD);
        }

        public void PrevStep() { 
            int idx = _curStep-1;
            if (idx < 0) return;
            SetStep(idx, true, StepJumpType.JUMP);
        }

        public void FirstStep(bool instantaneous=false) {
            SetAllPayloadsVisibility(false);
            SetStep(0, instantaneous);
        }

        public void GoStep(int destIdx, Trajectory traj) {
            if (!Misc.IsGoodIndex(destIdx, _steps)) return;
            if (_curStep == destIdx) return;
            StepJumpType jt = StepJumpType.JUMP;
            if (traj!=null) jt = StepJumpType.CUSTOM;
            SetStep(destIdx, false, jt, traj);
        }

        public void HandleDestinationReached(int idx) {
            int prevIdx = idx - 1;
            if (Misc.IsGoodIndex(prevIdx, _steps)) {
                _steps[prevIdx].Leave();
            }
        }

        #endregion

        #region KeyboardInput
        private void UpdateKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                PrevStep();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                NextStep();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                FirstStep();
            }
        }
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            Misc.CheckNotNull(_cameraMover);
            Misc.CheckNotNull(_stepsContainer);
            Misc.CheckNotNull(_trajectoryContainer);
            Misc.CheckNotNull(_destinationPrefab);
            Misc.CheckNotNull(_trajectoryPrefab);
            _camera = _cameraMover.transform.GetComponent<Camera>();
            InitSteps();
        }

        private void OnEnable()
        {
            FirstStep(true);
        }

        private void Update()
        {
            UpdateKeyboard();
        }
        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(StepController))]
    public class StepControllerEditor : Editor {
        private StepController GetTarget() { return (StepController)target; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (Application.isPlaying) {
                EditorUtility.SetDirty(GetTarget());
                EditorUI.Header("Current Step: "+GetTarget().CurrentStep);
            }
            EditorUI.Button("Make Destinations From Stuff in Steps", GetTarget().MakeDestinationsFromStuffInSteps);
            EditorUI.Button("Extract Payloads", GetTarget().ExtractPayloads);
            EditorUI.Button("Make step camera sockets look at payloads", GetTarget().MakeCamerasLookAtPayloads);
            EditorUI.Button("Make trajectories", GetTarget().MakeTrajectories);
            EditorUI.Button("Toggle all payloads visibility", GetTarget().ToggleAllPayloadsVisibility);
            EditorUI.Button("Link Step Animation Controllers", GetTarget().TryLinkStepAnimationControllers);
            EditorUI.Button("Collect Videos", GetTarget().CollectVideos);
        }
    }
#endif
}