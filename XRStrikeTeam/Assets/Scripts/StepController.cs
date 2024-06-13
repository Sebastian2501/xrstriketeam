using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accenture.eviola; 

namespace Accenture.XRStrikeTeam.Presentation
{
    public class StepController : MonoBehaviour
    {
        [Header("ExternalComponents")]
        [SerializeField]
        private CameraMover _cameraMover = null;
        [Header("Steps")]
        [SerializeField]
        private List<Destination> _steps = new List<Destination>();

        private int _curStep = -1;

        private Camera _camera;
        public Camera PovCamera { get { return _camera; } }
        public CameraMover CameraDriver { get { return _cameraMover; } }

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
}