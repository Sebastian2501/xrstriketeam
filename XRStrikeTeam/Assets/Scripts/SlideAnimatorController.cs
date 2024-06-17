using Accenture.eviola;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Accenture.XRStrikeTeam.Presentation
{
    public class SlideAnimatorController : MonoBehaviour
    {
        private Animator _animator = null;
        private const string _paramExitName = "Exit";
        private int _paramExitIndex = -1;

        public void EaseOut() {
            _animator.SetTrigger(_paramExitIndex);
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            Misc.CheckNotNull(_animator);

            _paramExitIndex = Animator.StringToHash(_paramExitName);
        }
    }
}