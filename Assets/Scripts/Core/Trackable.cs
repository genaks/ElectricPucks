using RedEngine.Core;
using UnityEngine;

namespace Core
{
    public class Trackable : MonoBehaviour
    {
        private TrackableLocator _trackableLocator;

        private void Start()
        {
            if (ServiceLocator.Instance.TryGet(out TrackableLocator trackableLocator))
            {
                _trackableLocator = trackableLocator;
                _trackableLocator.RegisterTrackable(this);
            }
        }

        private void OnDisable()
        {
            _trackableLocator.UnregisterTrackable(this);
        }
    }
}
