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
        private Button _btnFullScreen = null;
        [SerializeField]
        private Button _btnMinimize = null;
        [SerializeField]
        private Button _btnMute = null;
        [SerializeField]
        private Button _btnUnmute = null;

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

        #endregion
    }
}