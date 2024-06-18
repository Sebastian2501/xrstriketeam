using Accenture.eviola;
using Accenture.eviola.Async;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Accenture.XRStrikeTeam.Presentation
{

    public class MultiStepAsset : MonoBehaviour
    {
        [Header("External Components")]
        [SerializeField]
        private StepController _stepController = null;
        [SerializeField]
        private Transform _payloadContainer = null;
        [SerializeField]
        private Animator _animator = null;
        [Header("Options")]
        [SerializeField]
        private List<DestinationInterval> _activeStepIntervals = new List<DestinationInterval>();
        [SerializeField]
        private float _delayIn = 0;
        [SerializeField]
        private float _delayOut = 0;

        private DelayedAction _delayedActivation = null;
        private DelayedAction _delayedDeactivation = null;
        private DelayedAction _delayedExitAnimation = null;
        private const string _paramEnterName = "Enter";
        private const string _paramExitName = "Exit";
        private int _paramEnterIdx = -1;
        private int _paramExitIdx = -1;

        #region Animation

        private void InitAnimator() {
            if (_animator == null) return;
            _paramEnterIdx = Animator.StringToHash(_paramEnterName);
            _paramExitIdx = Animator.StringToHash(_paramExitName);
        }

        private bool HasAnimator() { return _animator != null; }
        
        #endregion

        #region Activation

        private bool IsActive() {
            if (_delayedDeactivation.IsRunning()) return false;
            if (_delayedExitAnimation.IsRunning()) return false;
            return _payloadContainer.gameObject.activeSelf; 
        }

        public void SetActive(bool b) {
            if (IsActive() == b) return;
            if (b)
            {
                if (_delayIn <= 0)
                {
                    EnforceActive();
                }
                else {
                    _delayedActivation.Abort();
                    _delayedActivation.Delay = _delayIn;
                    _delayedActivation.Fire();
                }
            }
            else {
                if (_delayOut <= 0)
                {
                    EnforceInactive();
                }
                else { 
                    _delayedDeactivation.Abort();
                    _delayedDeactivation.Delay = _delayOut;
                    _delayedDeactivation.Fire();
                }
            }
        }

        public void EnforceActive() { 
            _payloadContainer.gameObject.SetActive(true);
        }

        public void EnforceInactive() {
            if (HasAnimator())
            {
                _animator.SetTrigger(_paramExitIdx);
                _delayedExitAnimation.Abort();
                _delayedExitAnimation.Fire();
            }
            else {
                _payloadContainer.gameObject.SetActive(false);
            }
            
        }
        
        #endregion

        #region Intervals

        private bool IsInInterval(int idx) {
            foreach (var interval in _activeStepIntervals) {
                if (interval.IsInInterval(idx)) return true;
            }
            return false;
        }

        #endregion

        #region Controller

        private void HandleStepChanged(int s) {
            if (IsActive())
            {
                if (!IsInInterval(s)) {
                    SetActive(false);
                }
            }
            else {
                if (IsInInterval(s)) {
                    SetActive(true);                    
                }
            }
        }

        private void AddControllerListeners() {
            _stepController.OnStepChange.AddListener(HandleStepChanged);
        }

        private void RemoveControllerListeners() {
            _stepController.OnStepChange.AddListener(HandleStepChanged);
        }
        
        #endregion

        #region MonoBehaviour
        private void Awake()
        {
            Misc.CheckNotNull(_stepController);
            Misc.CheckNotNull(_payloadContainer);

            InitAnimator();

            _delayedActivation = new DelayedAction(this, 1, EnforceActive);
            _delayedDeactivation = new DelayedAction(this, 1, EnforceInactive);
            _delayedExitAnimation = new DelayedAction(this, 1, () => { _payloadContainer.gameObject.SetActive(false); });
        }

        private void OnEnable()
        {
            AddControllerListeners();
        }

        private void OnDisable()
        {
            RemoveControllerListeners();
        }
        #endregion
    }
}