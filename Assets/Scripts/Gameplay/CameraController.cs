using System.Collections.Generic;
using Core;
using UnityEngine;

namespace Gameplay
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] [Range(1f, 2f)] private float zoomPadding = 1.3f;
        [SerializeField] private float minDistance = 1f;
        [SerializeField] private float maxDistance = 50f;
        [SerializeField] private float positionSmoothTime = 0.3f;
        [SerializeField] private float zoomSmoothTime = 0.3f;
        [SerializeField] [Range(0f, 2f)] private float cameraAngle = 0.7f;

        private Camera  _camera;
        private TrackableLocator _trackableLocator;
        private float _currentDistance;
        private Vector3 _positionVelocity;
        private float _distanceVelocity;
        
        void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        private void Start()
        {
            ServiceLocator.Instance.TryGet(out _trackableLocator);
        }

        void LateUpdate()
        {
            var targets = _trackableLocator.GetAllTrackables();
            if (targets.Count == 0) return;

            Vector3 centroid = CalculateCentroid(targets);
            float targetDistance = CalculateRequiredDistance(targets, centroid);

            _currentDistance = Mathf.SmoothDamp(_currentDistance, targetDistance, ref _distanceVelocity, zoomSmoothTime);

            Vector3 targetPosition = centroid + new Vector3(0f, _currentDistance, -_currentDistance * cameraAngle);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _positionVelocity, positionSmoothTime);

            transform.LookAt(centroid);
        }

        // --- Helpers ---

        private static Vector3 CalculateCentroid(IReadOnlyList<Trackable> targets)
        {
            Vector3 sum = Vector3.zero;
            foreach (var target in targets)
                sum += target.transform.position;

            return sum / targets.Count;
        }

        private float CalculateRequiredDistance(IReadOnlyList<Trackable> targets, Vector3 centroid)
        {
            float boundingRadius = 0f;
            foreach (var target in targets)
            {
                float d = Vector3.Distance(target.transform.position, centroid);
                if (d > boundingRadius)
                {
                    boundingRadius = d;
                }
            }
            
            float verticalFovRad  = _camera.fieldOfView * Mathf.Deg2Rad;
            float horizontalFovRad  = 2f * Mathf.Atan(Mathf.Tan(verticalFovRad / 2f) * _camera.aspect);
            float minFov   = Mathf.Min(verticalFovRad, horizontalFovRad);

            float required = (boundingRadius * zoomPadding) / Mathf.Tan(minFov / 2f);
            return Mathf.Clamp(required, minDistance, maxDistance);
        }
    }
}