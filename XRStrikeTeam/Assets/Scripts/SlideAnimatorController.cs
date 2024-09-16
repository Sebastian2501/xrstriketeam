using Accenture.eviola;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Accenture.XRStrikeTeam.Presentation
{
    public class SlideAnimatorController : MonoBehaviour
    {
        private Animator _animator = null;
        private const string _paramEnterName = "Enter";
        private const string _paramExitName = "Exit";
        private int _paramEnterIndex = -1;
        private int _paramExitIndex = -1;

        public void EaseIn() {
            InitAnimator();
            _animator.SetTrigger(_paramEnterIndex);
        }

        public void EaseOut() {
            InitAnimator();
            _animator.SetTrigger(_paramExitIndex);
        }

        protected void InitAnimator()
        {
            if (_animator != null) return;
            _animator = GetComponent<Animator>();

            Misc.CheckNotNull(_animator);

            _paramEnterIndex = Animator.StringToHash(_paramEnterName);
            _paramExitIndex = Animator.StringToHash(_paramExitName);
        }

        private void Awake()
        {
            InitAnimator();
        }
    }
}