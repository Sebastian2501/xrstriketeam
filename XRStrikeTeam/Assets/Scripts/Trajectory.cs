using Accenture.eviola;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Accenture.rkiss.PathGeneration;
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
        [Header("Time")]
        [SerializeField]
        private float _duration = 3;
        [SerializeField]
        private float _delayEaseIn = 0;
        [SerializeField]
        private float _delayEaseOut = 0;

        public int NumWaypoints { get { return _wayPoints.Count; } }

        public float Duration { get { return _duration; } }
        public float DelayEaseIn { get { return _delayEaseIn; } }
        public float DelayEaseOut { get { return _delayEaseOut; } }


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
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(Trajectory))]
    public class TrajectoryEditor : Editor
    {
        private Trajectory GetTarget() { return (Trajectory)target; }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorUI.Button("Make Trajectory", () => { 
                GetTarget().MakeWayPoints();
                EditorUtility.SetDirty(GetTarget());   
            });
        }
    }

#endif
}