
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace Accenture.XRStrikeTeam.Presentation
{
    [RequireComponent(typeof(VideoPlayer))]
    public class UrlVideoPlayer : MonoBehaviour
    {
        public enum VideoState { 
            NOT_INITED,
            PREPARING,
            READY
        }

        [Tooltip("relative to StreamingAssets")]
        [SerializeField]
        private string _videoURL = string.Empty;
        [SerializeField]
        private bool _playOnAwake = true;

        private VideoPlayer _videoPlayer = null;
        private VideoState _curState = VideoState.NOT_INITED;
        private bool _bReservedPlay = false;

        public VideoState CurrentState { get { return _curState; } }

        private string _fullVideoUrl { get { return Path.Combine(Application.streamingAssetsPath, _videoURL); } }

        public void PlayVideo() {
            switch (_curState) { 
                case VideoState.NOT_INITED:
                    _bReservedPlay = true;
                    InitVideo();
                    break;
                case VideoState.PREPARING:
                    _bReservedPlay = true;
                    _videoPlayer.Prepare();
                    break;
                case VideoState.READY:
                    ExecutePlay();
                    break;
                default: 
                    break;
            }
        }

        public void InitVideo() {
            switch (_curState) {
                case VideoState.NOT_INITED:
                    Debug.Log("Initializing "+_videoURL);
                    _videoPlayer = GetComponent<VideoPlayer>();
                    _videoPlayer.playOnAwake = false;
                    _videoPlayer.url = _fullVideoUrl;
                    _videoPlayer.prepareCompleted += HandleVideoPrepared;
                    _videoPlayer.Prepare();
                    _curState = VideoState.PREPARING;
                    break;
                case VideoState.PREPARING:
                case VideoState.READY:
                default:
                    break;
            }
        }

        private void ExecutePlay() {
            Debug.Log("Playing " + _videoURL);
            _videoPlayer.Play();
            _bReservedPlay = false;
        }

        private void HandleVideoPrepared(VideoPlayer vp) {
            Debug.Log("Prepared "+_videoURL);
            _curState = VideoState.READY;
            if (_bReservedPlay) ExecutePlay();
        }

        private void Awake()
        {
            InitVideo();
        }

        private void OnEnable()
        {
            if (_playOnAwake) PlayVideo();
        }
    }
}