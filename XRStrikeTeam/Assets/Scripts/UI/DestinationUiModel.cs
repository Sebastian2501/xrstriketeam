using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Accenture.XRStrikeTeam.Presentation.UI
{
    [System.Serializable]
    public class DestinationInfoWidget {
        [HideInInspector]
        public UnityEvent<int> OnDestinationIndex = new UnityEvent<int>();
        public Trajectory TheTrajectory = null;
        public int DestinationIndex = 0;

        virtual public void InitUi() { }
        protected void InvokeDestintationIndexEvent() { OnDestinationIndex.Invoke(DestinationIndex); }
    }

    [System.Serializable]
    public class DestinationInfoButton : DestinationInfoWidget{ 
        public Button TheButton  = null;
        override public void InitUi() {
            TheButton.onClick.AddListener(HandleButtonClicked);
        }

        private void HandleButtonClicked() {
            InvokeDestintationIndexEvent();
        }
    }
}