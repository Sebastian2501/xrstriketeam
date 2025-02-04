using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accenture.eviola;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Accenture.XRStrikeTeam.Presentation
{
    public class StepCollection : MonoBehaviour
    {
        public List<Destination> Steps = new List<Destination>();
        public List<UrlVideoPlayer> Videos = new List<UrlVideoPlayer>();

#if UNITY_EDITOR
        public void PopulateFromChildren() {
            Steps.Clear();
            Videos.Clear();
            if (transform.childCount < 1) return;
            for (int i = 0; i < transform.childCount; i++) {
                Transform child = transform.GetChild(i);
                Destination dest = child.GetComponent<Destination>();
                if (dest != null) {
                    Steps.Add(dest);
                    FetchDestinationVideo(child);
                }
            }
            EditorUtility.SetDirty(this);
        }

        public void FetchDestinationVideo(Transform dest) {
            UrlVideoPlayer[] uvps = dest.GetComponentsInChildren<UrlVideoPlayer>();
            for (int i = 0; i < uvps.Length; i++) { 
                Videos.Add(uvps[i]);
            }
        }
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(StepCollection))]
    public class StepCollectionEditor : Editor {
        private StepCollection GetTarget() { return (StepCollection)target; }

        public override void OnInspectorGUI() { 
            base.OnInspectorGUI();

            EditorUI.Button("Populate from children", () => { GetTarget().PopulateFromChildren(); });
        }
    }
#endif
}