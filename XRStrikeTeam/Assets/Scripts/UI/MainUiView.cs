using Accenture.eviola;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Accenture.XRStrikeTeam.Presentation.UI
{
    public class MainUiView : MonoBehaviour
    {
        [Header("External Components")]
        [SerializeField]
        private StepController _stepController = null;
        [Header("UI")]
        [SerializeField]
        private Button _btnPrev = null;
        [SerializeField]
        private Button _btnNext = null;
        [SerializeField]
        private Button _btnHome = null;
        [SerializeField]
        private PointerButton _btnFullScreen = null;
        [SerializeField]
        private PointerButton _btnMinimize = null;
        [SerializeField]
        private Button _btnMute = null;
        [SerializeField]
        private Button _btnUnmute = null;

        #region fullscreen
        
        private bool _isFullscreen {
            get {
                return Screen.fullScreen;
            }
            set { 
                Screen.fullScreen = value;
                DisplayFullscreenState(value);
            }
        }
        
        #endregion

        #region Controller
        private void AddControllerListeners() {
            DisplayeMuteState(_stepController.IsMuted());

            _stepController.OnMuteStateChange.AddListener(HandleMutedChanged);
        }

        private void RemoveControllerListeners() {
            _stepController.OnMuteStateChange.RemoveListener(HandleMutedChanged);
        }

        private void HandleMutedChanged(bool b) { 
            DisplayeMuteState(b);
        }

        #endregion

        #region UI

        private void DisplayeMuteState(bool b) {
            _btnMute.gameObject.SetActive(!b);
            _btnUnmute.gameObject.SetActive(b);
        }

        private void DisplayFullscreenState(bool b) { 
            _btnFullScreen.gameObject.SetActive(!b);
            _btnMinimize.gameObject.SetActive(b);
        }

        private void HandlePrevClick() {
            if (!_stepController.DidEnoughTimePassFromLastStepChange()) return;
            _stepController.PrevStep();
        }

        private void HandleNextClick() { 
            _stepController.NextStep();
        }

        private void HandleHomeClicked() {
            if (!_stepController.DidEnoughTimePassFromLastStepChange()) return;
            _stepController.FirstStep(_stepController.ShouldGoHomeInstantly);
        }

        private void HandleFullscreenClick() { _isFullscreen = true; }

        private void HandleMinimizeClick() { _isFullscreen = false; }

        private void HandleMuteClick() { 
            _stepController.SetMuted(true);
        }

        private void HandleUnmuteClick() {
            _stepController.SetMuted(false);
        }

        private void AddUiListeners()
        {
            _btnPrev.onClick.AddListener(HandlePrevClick);
            _btnNext.onClick.AddListener(HandleNextClick);
            _btnFullScreen.OnPointerClick.AddListener(HandleFullscreenClick);
            _btnMinimize.OnPointerClick.AddListener(HandleMinimizeClick);
            _btnMute.onClick.AddListener(HandleMuteClick);
            _btnUnmute.onClick.AddListener(HandleUnmuteClick);
            _btnHome.onClick.AddListener(HandleHomeClicked);
        }

        private void RemoveUiListeners() {
            _btnPrev.onClick.RemoveListener(HandlePrevClick);
            _btnNext.onClick.RemoveListener(HandleNextClick);
            _btnFullScreen.OnPointerClick.RemoveListener(HandleFullscreenClick);
            _btnMinimize.OnPointerClick.RemoveListener(HandleMinimizeClick);
            _btnMute.onClick.RemoveListener(HandleMuteClick);
            _btnUnmute.onClick.RemoveListener(HandleUnmuteClick);
            _btnHome.onClick.RemoveListener(HandleHomeClicked);
        }

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            Misc.CheckNotNull(_stepController);
            Misc.CheckNotNull(_btnPrev);
            Misc.CheckNotNull(_btnNext);
            Misc.CheckNotNull(_btnHome);
            Misc.CheckNotNull(_btnFullScreen);
            Misc.CheckNotNull(_btnMinimize);
            Misc.CheckNotNull(_btnMute);
            Misc.CheckNotNull(_btnUnmute);
        }

        private void OnEnable()
        {
            DisplayFullscreenState(_isFullscreen);
            AddControllerListeners();
            AddUiListeners();
        }


        private void OnDisable()
        {
            RemoveControllerListeners();
            RemoveUiListeners();
        }
        #endregion
    }
}