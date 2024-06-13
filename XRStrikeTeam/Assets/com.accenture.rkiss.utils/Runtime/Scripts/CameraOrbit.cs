using DG.Tweening;
using UnityEngine;

namespace Accenture.rkiss.Utils
{
    public class CameraOrbit : MonoBehaviour
    {
        [SerializeField] private Transform followTarget; // The target the camera will orbit around
        [SerializeField] private float distance = 10.0f; // Distance from the target
        [SerializeField] private float xSpeed = 120.0f; // Speed of the orbit along the X axis
        [SerializeField] private float ySpeed = 120.0f; // Speed of the orbit along the Y axis
        [SerializeField] private float zoomSpeed = 5.0f; // Speed of zooming in and out
        [SerializeField] private float minDistance = 2.0f; // Minimum distance to the target
        [SerializeField] private float maxDistance = 20.0f; // Maximum distance from the target
        [SerializeField] private float yMinLimit = -20f; // Minimum Y angle limit
        [SerializeField] private float yMaxLimit = 80f; // Maximum Y angle limit

        private float x = 0.0f;
        private float y = 0.0f;

        [SerializeField] private float moveSpeed = 10f; // Speed of movement
        [SerializeField] private float moveDuration = 0.5f; // Duration of the eased movement

        void Start()
        {
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;
        }

        void LateUpdate()
        {
            if (followTarget)
            {
                HandleMouseInput();
                HandleZoomInput();
                Quaternion rotation = Quaternion.Euler(y, x, 0);
                Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + followTarget.position;

                transform.rotation = rotation;
                transform.position = position;
            }
        }

        void HandleMouseInput()
        {
            if (Input.GetMouseButton(0))
            {
                x += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }
        }

        void HandleZoomInput()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0.0f)
            {
                float targetDistance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);
                DOTween.To(() => distance, x => distance = x, targetDistance, moveDuration).SetEase(Ease.OutQuad);
            }
        }

        internal static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}