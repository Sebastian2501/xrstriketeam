using Accenture.eviola;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Accenture.XRStrikeTeam.Presentation.UI
{
    public class DestinationMenuUiView : MonoBehaviour
    {
        [Header("External Components")]
        [SerializeField]
        private StepController _stepController;
        [Header("UI")]
        [SerializeField]
        private List<DestinationInfoButton> _buttons = new List<DestinationInfoButton>();
        [Header("Preferences")]
        [SerializeField]
        private int _myStepIndex = 0;

        private Dictionary<int, DestinationInfoButton> _infoByDestination = new Dictionary<int, DestinationInfoButton>();

        #region UI

        private void InitUi() {
            foreach (var button in _buttons) {
                if (button != null) { 
                    button.InitUi();
                }
            }
            _infoByDestination.Clear();
            foreach (var button in _buttons) {
                _infoByDestination[button.DestinationIndex] = button;
            }
        }

        private void SetUiListeners(bool b) {
            foreach (var button in _buttons) {
                if (button != null) {
                    if (b)
                    {
                        button.OnDestinationIndex.AddListener(HandleDestinationClicked);
                    }
                    else {
                        button.OnDestinationIndex.RemoveListener(HandleDestinationClicked);
                    }
                }
            }
        }

        private void HandleDestinationClicked(int idx) {
            _stepController.GoStep(idx, _infoByDestination[idx].TheTrajectory);
        }

        #endregion

        #region Setup
#if UNITY_EDITOR
        /*public void MakeTrajectories() {
            foreach (var button in _buttons) {
                if (button != null) {
                    Trajectory t = _stepController.MakeTrajectoryFromTo(_myStepIndex, button.DestinationIndex);
                    if (t != null) { 
                        button.TheTrajectory = t;
                    }
                }
            }
        }*/
#endif

#endregion

        #region  MonoBehaviour

        private void Awake()
        {
            Misc.CheckNotNull(_stepController);
            InitUi();
        }

        private void OnEnable()
        {
            SetUiListeners(true);
        }

        private void OnDisable()
        {
            SetUiListeners(false);
        }

        #endregion

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DestinationMenuUiView))]
    public class DestinationMenuUiViewEditor : Editor { 
        private DestinationMenuUiView Target { get { return (DestinationMenuUiView)target; } }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorUI.Button("Make Transitions", () => {
                //Target.MakeTrajectories();
                EditorUtility.SetDirty(Target);
            });
        }
    }

#endif
}