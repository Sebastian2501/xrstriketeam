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
        [Header("Collection Assets")]
        public List<Destination> Steps = new List<Destination>();
        public List<UrlVideoPlayer> Videos = new List<UrlVideoPlayer>();
        [SerializeField]
        private Transform _trajectoryContainer = null;
        [Header("Prefabs")]
        [SerializeField]
        private GameObject _destinationPrefab = null;
        [SerializeField]
        private GameObject _trajectoryPrefab = null;

        #region Steps

        public void InitSteps(StepController sc) {
            for (int i = 0; i < Steps.Count; i++)
            {
                Steps[i].Controller = sc;
                Steps[i].Id = i;
            }
            foreach (var vid in Videos) {
                vid.InitVideo();
            }
        }

        public Destination GetStep(int idx) {
            if(!Misc.IsGoodIndex(idx, Steps))return null;
            return Steps[idx];
        }

        #endregion

        #region Mute

        public void SetMuted(bool b) {
            foreach (var vid in Videos) { 
                vid.IsMuted = b;
            }
        }
        
        #endregion

        #region Editor
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

        public void MakeCameraLookAtPayloads() {
            foreach (Destination step in Steps)
            {
                step.PointCameraAtPayload();
                EditorUtility.SetDirty(step);
            }
            EditorUtility.SetDirty(this);
        }

        public void TryLinkStepAnimationControllers()
        {
            if (Steps.Count < 1) return;
            for (int i = 0; i < Steps.Count - 1; i++)
            {
                Steps[i].TryLinkAnimationController();
                EditorUtility.SetDirty(Steps[i]);
            }
            EditorUtility.SetDirty(this);
        }

        public void MakeTrajectories() {
            if (Steps.Count < 1) return;
            for (int i = 0; i < Steps.Count - 1; i++)
            {
                int idxNxt = i + 1;
                GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(_trajectoryPrefab);
                go.transform.parent = _trajectoryContainer;
                go.name = i + "to" + idxNxt + "_trajectory";
                Trajectory trajectory = go.GetComponent<Trajectory>();
                if (trajectory != null)
                {
                    Steps[i].NextTrajectory = trajectory;
                    trajectory.SetTransformsAndMakeWaypoints(Steps[i].CameraSocket, Steps[idxNxt].CameraSocket);
                    EditorUtility.SetDirty(trajectory);
                    EditorUtility.SetDirty(Steps[i]);
                }
            }
        }

        public Trajectory MakeTrajectoryFromTo(int idxFrom, int idxTo)
        {
            if (_trajectoryContainer == null) return null;
            if(_trajectoryPrefab==null)return null;
            if (!Misc.IsGoodIndex(idxFrom, Steps) || !Misc.IsGoodIndex(idxTo, Steps)) return null;

            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(_trajectoryPrefab);
            go.transform.parent = _trajectoryContainer;
            go.name = idxFrom + "to" + idxTo + "_trajectory";
            Trajectory trajectory = go.GetComponent<Trajectory>();
            if (trajectory != null)
            {
                trajectory.SetTransformsAndMakeWaypoints(Steps[idxFrom].CameraSocket, Steps[idxTo].CameraSocket);
                EditorUtility.SetDirty(trajectory);
            }
            return trajectory;
        }
#endif

        #endregion
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(StepCollection))]
    public class StepCollectionEditor : Editor {
        private StepCollection GetTarget() { return (StepCollection)target; }

        public override void OnInspectorGUI() { 
            base.OnInspectorGUI();

            EditorUI.Button("Populate from children", () => { GetTarget().PopulateFromChildren(); });
            EditorUI.Button("Make Camera Look at Payload", () => { GetTarget().MakeCameraLookAtPayloads(); });
            EditorUI.Button("Try Link Animation Controllers", () => { GetTarget().TryLinkStepAnimationControllers(); });
            EditorUI.Button("Make Trajectories", () => { GetTarget().MakeTrajectories(); });
        }
    }
#endif
}