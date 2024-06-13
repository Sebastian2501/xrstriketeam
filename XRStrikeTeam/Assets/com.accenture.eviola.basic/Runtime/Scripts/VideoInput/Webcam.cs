using Accenture.eviola.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Accenture.eviola.VideoInput
{
    [System.Serializable]
    public class Webcam
    {
        public UnityEvent OnCameraStopped = new UnityEvent();
        public UnityEvent OnCameraStarted = new UnityEvent();
        public TextureEvent OnTextureBecameAvailable = new TextureEvent();

        public int DesiredCameraId = -1;
        public int CamWidth = 1024;
        public int CamHeight = 768;
        public int Fps = 30;

        public MonoBehaviour ParentMonoBehaviour = null;

        private int _curCamId = -1;
        private string _curCamName = "";
        private WebCamTexture _webCamTexture = null;
        private bool _bRunning = false;
        private bool _bCheckingTexture = false;

        static public int GetNumAvailableCameras() { return WebCamTexture.devices.Length; }

        static public bool IsGoodCameraDeviceId(int i) { return Misc.IsGoodIndex(i, WebCamTexture.devices); }

        /// <summary>
        /// returns the device id, given its name; if the provided name is not valid, it will return -1
        /// </summary>
        static public int GetCameraDeviceIdFromDeviceName(string n) {
            for (int i = 0; i < WebCamTexture.devices.Length; i++) {
                if (n == WebCamTexture.devices[i].name) return i;
            }
            return -1;
        }

        static public List<string> GetAvailableCameraDeviceNames() { 
            List<string> n = new List<string>();
            foreach (WebCamDevice d in WebCamTexture.devices) {
                n.Add(d.name);
            }
            return n;
        }

        public bool IsRunning() { return _bRunning; }

        public void Start() {
            Start(DesiredCameraId);
        }

        public void Start(string deviceName)
        {
            Start(GetCameraDeviceIdFromDeviceName(deviceName));
        }

        public void Start(int deviceId) {
            if (_bRunning) {
                if (_curCamId == deviceId) return;
                Stop();
            }
            if (!IsGoodCameraDeviceId(deviceId)) {
                Debug.LogError("Bad device id: "+deviceId);
                return;
            }
            _curCamId = deviceId;
            _curCamName = WebCamTexture.devices[deviceId].name;
            _webCamTexture = new WebCamTexture(_curCamName, CamWidth, CamHeight, Fps);
            _webCamTexture.Play();
            _bRunning = true;
            OnCameraStarted.Invoke();
            StartCheckingTextureAvailability();
        }

        public void Stop() {
            if (!_bRunning) return;
            _bRunning = false;
            if (_webCamTexture == null) return;
            _webCamTexture.Stop();
            OnCameraStopped.Invoke();
        }

        public Texture GetTexture() { return _webCamTexture; }

        public Color32[] GetPixels32() { return _webCamTexture.GetPixels32(); }

        public bool IsTextureAvailable() {
            if (_webCamTexture == null) return false;
            return _webCamTexture.width > 20;
        }

        private void StartCheckingTextureAvailability() {
            if (ParentMonoBehaviour == null) return;
            if (_bCheckingTexture) return;
            _bCheckingTexture = true;
            ParentMonoBehaviour.StartCoroutine(CheckTextureAvailability());
        }

        private void StopCheckingTextureAvailability() {
            if (!_bCheckingTexture) return;
            _bCheckingTexture = false;
            ParentMonoBehaviour.StopCoroutine(CheckTextureAvailability());
        }

        private IEnumerator CheckTextureAvailability() {
            while (!IsTextureAvailable()) { 
                yield return new WaitForSeconds(0.5f);
            }
            OnTextureBecameAvailable.Invoke(GetTexture());
            StopCheckingTextureAvailability();
        }
    }
}