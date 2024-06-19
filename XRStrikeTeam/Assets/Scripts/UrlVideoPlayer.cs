using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Video;

namespace Accenture.XRStrikeTeam.Presentation
{
    [RequireComponent(typeof(VideoPlayer))]
    public class UrlVideoPlayer : MonoBehaviour
    {
        [Tooltip("relative to StreamingAssets")]
        [SerializeField]
        private string _videoURL = string.Empty;
        [SerializeField]
        private bool _playOnAwake = true;

        private VideoPlayer _videoPlayer = null;

        private string _fullVideoUrl { get { return Path.Combine(Application.streamingAssetsPath, _videoURL); } }

        public void PlayVideo() {
            _videoPlayer.Play();
        }

        private void Awake()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
            _videoPlayer.playOnAwake = false;
            _videoPlayer.url = _fullVideoUrl;
        }

        private void OnEnable()
        {
            if (_playOnAwake) PlayVideo();
        }
    }
}