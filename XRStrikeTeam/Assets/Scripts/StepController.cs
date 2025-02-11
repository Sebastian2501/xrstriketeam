using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accenture.eviola;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Accenture.XRStrikeTeam.Presentation
{
    public class StepController : MonoBehaviour
    {
        public UnityEvent<int> OnStepChange = new UnityEvent<int>();
        public UnityEvent<bool> OnMuteStateChange = new UnityEvent<bool>();

        [Header("ExternalComponents")]
        [SerializeField]
        private CameraMover _cameraMover = null;
        [SerializeField]
        private ScreenCurtainController _screenCurtain = null;
        [Header("Steps")]
        [SerializeField]
        private List<StepCollection> _stepCollections = new List<StepCollection>();
        [Header("Options")]
        [SerializeField]
        private bool _instantFirstStep = true;
        [SerializeField]
        private bool _fadeToFirstStep = true;
        [SerializeField]
        private float _screenFadeTime = 1;
        [SerializeField]
        private float _minStepChangeTime = 2;
        [SerializeField]
        private bool _circularSteps = true;

        public bool ShouldGoHomeInstantly { get { return _instantFirstStep; } }

        private float _lastTimeStepChanged = 0;
        private int _curStepCollection = -1;
        private int _curStep = -1;
        private bool _bMuted = false;

        private Camera _camera;
        public Camera PovCamera { get { return _camera; } }
        public CameraMover CameraDriver { get { return _cameraMover; } }

        public int CurrentStep { get { return _curStep; } }

        private List<Destination> _steps { get { return _stepCollections[_curStepCollection].Steps; } }

        #region Init
        public bool IsDestination(Transform tra) {
            return tra.GetComponent<Destination>() != null;
        }


        public void ToggleAllPayloadsVisibility() {
            if (!Misc.IsGoodIndex(_curStepCollection, _stepCollections)) return;
            foreach (Destination step in _steps)
            {
                step.TogglePayloadVisibility();
            }
        }

        public void SetAllPayloadsVisibility(bool b) {
            if (!Misc.IsGoodIndex(_curStepCollection, _stepCollections)) return;
            foreach (Destination step in _steps)
            {
                step.PayloadContainer.gameObject.SetActive(b);
            }
        }

        #endregion

        #region Steps

        public Destination GetStep(int idx) {
            if (!Misc.IsGoodIndex(_curStepCollection, _stepCollections)) return null;
            return _stepCollections[_curStepCollection].GetStep(idx);
            
        }

        private void InitSteps() {
            
            foreach (var collection in _stepCollections) { 
                collection.InitSteps(this);
            }
        }

        public void SetStep(int idx, bool instantaneous = false, StepJumpType jt = StepJumpType.JUMP, Trajectory traj = null) {
            if (IsCameraMoving()) return;
            if (!Misc.IsGoodIndex(_curStepCollection, _stepCollections)) return;
            if (!Misc.IsGoodIndex(idx, _steps)) return;
            if (idx == _curStep) return;
            PrintSetStepDebugInfo(idx, instantaneous, jt, traj);
            if (instantaneous)
            {
                if (Misc.IsGoodIndex(_curStep, _steps)) _steps[_curStep].Leave();
            }
            _curStep = idx;
            OnStepChange.Invoke(_curStep);
            if (instantaneous)
            {
                _steps[_curStep].PayloadContainer.gameObject.SetActive(true);
                _steps[_curStep].Enter(instantaneous);
            }
            else
            {
                _steps[_curStep].Go(jt, traj);
            }
            _lastTimeStepChanged = Time.time;
        }

        private void PrintSetStepDebugInfo(int idx, bool instantaneous, StepJumpType jt, Trajectory traj) {
            Debug.Log("SetStep: "+idx+": "+(instantaneous?"instant":"NOTinstant")+", "+jt+", "+(traj!=null?"trajectory":"NOtrajectory"));
        }

        public void NextStep() {
            if (!Misc.IsGoodIndex(_curStepCollection, _stepCollections)) return;
            int idx = _curStep + 1;
            if (idx >= _steps.Count)
            {
                if(!_circularSteps) return;
                FirstStep();
                return;
            }
            SetStep(idx, false, StepJumpType.FORWARD);
        }

        public void PrevStep() {
            if (!Misc.IsGoodIndex(_curStepCollection, _stepCollections)) return;
            int idx = _curStep - 1;
            if (idx < 0)
            {
                if(!_circularSteps) return;
                idx = _steps.Count - 1;
            }
            if (_fadeToFirstStep) FadeInAndOut();
            SetStep(idx, true, StepJumpType.JUMP);
        }

        public void FirstStep(bool instantaneous = false) {
            if (!Misc.IsGoodIndex(_curStepCollection, _stepCollections)) return;
            if (_curStep == 0) return;
            if (_fadeToFirstStep) FadeInAndOut();
            SetAllPayloadsVisibility(false);
            SetStep(0, instantaneous);
        }

        public void GoStep(int destIdx, Trajectory traj) {
            if (!Misc.IsGoodIndex(_curStepCollection, _stepCollections)) return;
            if (!Misc.IsGoodIndex(destIdx, _steps)) return;
            if (_curStep == destIdx) return;
            StepJumpType jt = StepJumpType.JUMP;
            if (traj != null) jt = StepJumpType.CUSTOM;
            SetStep(destIdx, false, jt, traj);
        }

        public void HandleDestinationReached(int idx) {
            int prevIdx = idx - 1;
            if (Misc.IsGoodIndex(prevIdx, _steps)) {
                _steps[prevIdx].Leave();
            }
        }

        public bool IsCameraMoving() { return _cameraMover.IsMoving(); }

        #endregion

        #region Audio

        public bool IsMuted() { return _bMuted; }

        public void SetMuted(bool b) {
            if (b == IsMuted()) return;
            foreach (var collection in _stepCollections) { 
                collection.SetMuted(b);
            }
            
            _bMuted = b;
            OnMuteStateChange.Invoke(_bMuted);
        }

        public void ToggleMuted() { SetMuted(!_bMuted); }

        #endregion

        #region ScreenFade

        private void FadeInAndOut() {
            _screenCurtain.SetOpaque(true);
            StartCoroutine(DelayFadeOut());
        }

        private IEnumerator DelayFadeOut() {
            yield return new WaitForSeconds(_screenFadeTime);
            _screenCurtain.SetOpaque(false);
        }

        #endregion

        #region AntiThrottle

        public bool DidEnoughTimePassFromLastStepChange() {
            return Time.time > (_lastTimeStepChanged + _minStepChangeTime);
        }
        
        #endregion

        #region KeyboardInput
        private void UpdateKeyboard()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (DidEnoughTimePassFromLastStepChange()) PrevStep();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                NextStep();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if(DidEnoughTimePassFromLastStepChange()) FirstStep(_instantFirstStep);
            }
        }
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            Misc.CheckNotNull(_cameraMover);
            Misc.CheckNotNull(_screenCurtain);
            _camera = _cameraMover.transform.GetComponent<Camera>();
            InitSteps();
        }

        private void OnEnable()
        {
            _curStepCollection = 0;
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
            
        }
    }
#endif
}