using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Accenture.eviola.Animation
{
    public class Billboarder : MonoBehaviour
    {
        [Header("External Components")]
        [Tooltip("This is the camera to billboard to; leave empty to use the default camera")]
        [SerializeField]
        private Camera _theCamera = null;
        [Header("Options")]
        [SerializeField]
        private bool _lockX = false;
        [SerializeField]
        private bool _lockY = false;
        [SerializeField]
        private bool _lockZ = false;
        [Space]
        [SerializeField]
        private bool _startOnEnable = true;

        private Camera _camera = null;
        private bool _bBillboarding = false;

        public bool IsBillboarding { get { return _bBillboarding; } }

        public void SetBillboarding(bool b) {
            if (IsBillboarding == b) return;
            _bBillboarding = b;
        }

        private void Awake()
        {
            if (_theCamera == null)
            {
                _camera = Camera.main;
            }
            else { 
                _camera = _theCamera;
            }
        }

        private void OnEnable()
        {
            if(_startOnEnable)SetBillboarding(true);
        }

        private void Update()
        {
            Vector3 pos = _camera.transform.position;
            if(_lockX)pos.x = transform.position.x;
            if(_lockY)pos.y = transform.position.y;
            if(_lockZ)pos.z = transform.position.z;
            transform.LookAt(pos);
        }
    }
}