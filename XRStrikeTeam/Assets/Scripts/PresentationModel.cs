using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Accenture.XRStrikeTeam.Presentation {
    [System.Serializable]
    public class DestinationInterval {
        [SerializeField]
        public Destination FromDestination = null;
        [SerializeField]
        public Destination ToDestination = null;

        public bool IsInInterval(int idx) {
            if (FromDestination == null || ToDestination == null) return false;
            int fromIdx = FromDestination.Id;
            int toIdx = ToDestination.Id;
            if (toIdx < fromIdx) { 
                int temp = fromIdx;
                fromIdx = toIdx;
                toIdx = temp;
            }
            return idx>=fromIdx && idx<=toIdx;
        }
    }

    public enum StepJumpType { 
        FORWARD,
        BACKWARDS,
        JUMP,
        CUSTOM
    }
}
