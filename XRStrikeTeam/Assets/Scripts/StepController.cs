using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accenture.eviola;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Accenture.XRStrikeTeam.Presentation
{
    public class StepController : MonoBehaviour
    {
        [Header("ExternalComponents")]
        [SerializeField]
        private CameraMover _cameraMover = null;
        [SerializeField]
        private Transform _stepsContainer = null;
        [SerializeField]
        private GameObject _destinationPrefab = null;
        [Header("Steps")]
        [SerializeField]
        private List<Destination> _steps = new List<Destination>();

        private int _curStep = -1;

        private Camera _camera;
        public Camera PovCamera { get { return _camera; } }
        public CameraMover CameraDriver { get { return _cameraMover; } }

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
                Vector3 payloadPos = tra.position;
                GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(_destinationPrefab);
                go.transform.parent = _stepsContainer;
                go.name = _steps.Count + "_Step";
                Destination dst = go.GetComponent<Destination>();
                dst.PayloadContainer.position = payloadPos;
                tra.parent = dst.PayloadContainer;
                _steps.Add(dst);
            }

            foreach (Destination step in _steps) { 
                EditorUtility.SetDirty(step);
            }

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
#endif
        #endregion

        #region Steps

        private void InitSteps() {
            for (int i = 0; i < _steps.Count; i++) {
                _steps[i].Controller = this;
                _steps[i].Id = i;
            }
        }

        private void SetStep(int idx, bool instantaneous = false) {
            if (!Misc.IsGoodIndex(idx, _steps)) return;
            if (idx == _curStep) return;
            if (instantaneous)
            {
                if (Misc.IsGoodIndex(_curStep, _steps)) _steps[_curStep].Leave();
            }
            _curStep = idx;
            if (instantaneous)
            {
                _steps[_curStep].Enter();
            }
            else
            {
                _steps[_curStep].Go();
            }
        }

        public void NextStep() { 
            int idx = _curStep+1;
            if (idx >= _steps.Count) return;
            SetStep(idx);
        }

        public void PrevStep() { 
            int idx = _curStep-1;
            if (idx < 0) return;
            SetStep(idx);
        }

        public void FirstStep(bool instantaneous=false) { 
            SetStep(0, instantaneous);
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
            Misc.CheckNotNull(_destinationPrefab);
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

            EditorUI.Button("Make Destinations From Stuff in Steps", GetTarget().MakeDestinationsFromStuffInSteps);
            EditorUI.Button("Make step camera sockets look at payloads", GetTarget().MakeCamerasLookAtPayloads);
        }
    }
#endif
}