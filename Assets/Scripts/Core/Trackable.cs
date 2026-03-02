using System;
using UnityEngine;

namespace Core
{
    public class Trackable : MonoBehaviour
    {
        private TrackableLocator _trackableLocator;
        private bool _registered;

        private void Start()
        {
            ServiceLocator.Instance.TryGet(out _trackableLocator);
            _trackableLocator.RegisterTrackable(this);
            _registered = true;
        }

        private void OnEnable()
        {
            if (null != _trackableLocator && !_registered)
            {
                _trackableLocator.RegisterTrackable(this);
            }
        }

        private void OnDisable()
        {
            _trackableLocator.UnregisterTrackable(this);
            _registered = false;
        }
    }
}
