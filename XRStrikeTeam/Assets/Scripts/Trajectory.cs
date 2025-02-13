using Accenture.eviola;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accenture.rkiss.PathGeneration;
using System;
using UnityEngine.UIElements;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Accenture.XRStrikeTeam
{
    public class Trajectory : MonoBehaviour
    {
        [Header("Path")]
        [SerializeField]
        private Transform _from = null;
        [SerializeField]
        private Transform _to = null;
        [SerializeField]
        private Vector3 _bezierHandle1 = Vector3.zero;
        [SerializeField]
        private Vector3 _bezierHandle2 = Vector3.zero;
        [SerializeField]
        private List<Vector3> _wayPoints = new List<Vector3>();
        [SerializeField]
        public int DesiredNumWayPoints = 10;
        [SerializeField]
        public eviola.Animation.Easing MovementEasing = eviola.Animation.Easing.LINEAR;
        [Header("Time")]
        [SerializeField]
        private float _duration = 3;
        [SerializeField]
        private float _delayEaseIn = 0;
        [SerializeField]
        private float _delayEaseOut = 0;
        [Header("options")]
        [SerializeField]
        private bool _keepTrackOfStepIndices = false;
        [HideInInspector]
        [SerializeField]
        private int _idxFrom = -1;
        [HideInInspector]
        [SerializeField]
        private int _idxTo = -1;

        #region namingConventions

        /// <summary>
        /// given a trajectory name, return the from and to indices
        /// expected name format: FROMtoTO_trajectory
        /// </summary>
        /// <returns>Tuple<from, to>; Tuple<-1,-1> if bad naming</returns>
        static public Tuple<int, int> NameToIndices(string name) { 
            Tuple<int,int> res = new Tuple<int,int>(-1,-1);
            string[] indicesAndName = name.Split("_");
            if (indicesAndName.Length < 2) return Tuple.Create(-1, -1);
            string[] indices = indicesAndName[0].Split("to");
            if(indices.Length<2) return Tuple.Create(-1, -1);
            return Tuple.Create(Int32.Parse(indices[0]), Int32.Parse(indices[1]));
        }

        public void SetStepIndices(int from, int to) {
            _keepTrackOfStepIndices = true;
            _idxFrom = from;
            _idxTo = to;
        }

        public void SetStepIndicesFromName() {
            Tuple<int, int> indices = NameToIndices(gameObject.name);
            if (indices.Item1 < 0 || indices.Item2 < 0) return;
            SetStepIndices(indices.Item1, indices.Item2);
        }

        public void SetStepIndicesFromNameIfNotConsecutive() {
            Tuple<int, int> indices = NameToIndices(gameObject.name);
            if (indices.Item1 < 0 || indices.Item2 < 0) return;
            if (indices.Item1 == indices.Item2 - 1) return;
            SetStepIndices(indices.Item1, indices.Item2);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }

        #endregion

        #region setup

        public int NumWaypoints { get { return _wayPoints.Count; } }

        public float Duration { get { return _duration; } }
        public float DelayEaseIn { get { return _delayEaseIn; } }
        public float DelayEaseOut { get { return _delayEaseOut; } }

        public bool KeepTrackOfStepIndices
        {
            get { 
                return _keepTrackOfStepIndices;
            }
        }

        public int IdxFrom
        {
            get { return _idxFrom; }
            set { _idxFrom = value; }
        }

        public int IdxTo
        {
            get { return _idxTo; }
            set { _idxTo = value; }
        }

        public bool IsTrackingStepIndices() {
            if (!_keepTrackOfStepIndices) return false;
            return (_idxFrom>=0 && _idxTo>=0);
        }

        public Vector3 GetWayPoint(int idx) {
            if (!Misc.IsGoodIndex(idx, _wayPoints)) return Vector3.zero;
            return _wayPoints[idx];
        }

        public void MakeWayPoints() { 
            _wayPoints.Clear();
            Vector3[] pts = PathGenerationFunctions.GetWaypointsAlongSpline(_from.position, _to.position, _bezierHandle1, _bezierHandle2, 1, false, 1, DesiredNumWayPoints);
            foreach (Vector3 point in pts)
            {
                _wayPoints.Add(point);
            }
        }

        public void SetTransformsAndMakeWaypoints(Transform startPoint, Transform endPoint) {
            _from = startPoint;
            _to = endPoint;
            MakeWayPoints();
        }

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            Misc.CheckNotNull(_from);
            Misc.CheckNotNull(_to);
        }

        private void OnDrawGizmosSelected()
        {
            if (_from == null || _to == null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_from.position+_bezierHandle1, 0.3f);
            Gizmos.DrawWireCube(_from.position, new Vector3(0.3f, 0.3f, 0.3f));
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_to.position+_bezierHandle2, 0.3f);
            Gizmos.DrawWireCube(_to.position, new Vector3(0.3f, 0.3f, 0.3f));
            Gizmos.color = Color.magenta;
            foreach (Vector3 point in _wayPoints)
            {
                Gizmos.DrawWireSphere(point, 0.1f);
            }
        }
        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(Trajectory))]
    public class TrajectoryEditor : Editor
    {
        private Trajectory GetTarget() { return (Trajectory)target; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GetTarget().KeepTrackOfStepIndices) {
                EditorUI.IntField("Step index From", GetTarget().IdxFrom, (int i) => { GetTarget().IdxFrom = i; EditorUtility.SetDirty(GetTarget()); });
                EditorUI.IntField("Step index To", GetTarget().IdxTo, (int i) => { GetTarget().IdxTo = i; EditorUtility.SetDirty(GetTarget()); });
            }

            EditorUI.Header("Editor");
            EditorUI.Button("Make Trajectory", () => { 
                GetTarget().MakeWayPoints();
                EditorUtility.SetDirty(GetTarget());   
            });
            EditorUI.Button("TrackIndicesFromName", () => {
                GetTarget().SetStepIndicesFromName();
                EditorUtility.SetDirty(GetTarget());
            });
        }
    }

#endif
}