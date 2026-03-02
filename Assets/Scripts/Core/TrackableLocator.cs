using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class TrackableLocator : MonoBehaviour, IGameService
    {

        private readonly List<Trackable> _trackables = new ();

        private void Awake()
        {
            ServiceLocator.Instance.Register(this);
        }

        public void RegisterTrackable(Trackable trackable)
        {
            if (_trackables.Contains(trackable))
            {
                Debug.Log($"Trackable {trackable.gameObject.name} is already registered with the trackable locator");
                return;
            }
            
            _trackables.Add(trackable);
        }

        public void UnregisterTrackable(Trackable trackable)
        {
            if (!_trackables.Contains(trackable))
            {
                Debug.Log($"Attempting to unregister a trackable which was not registered with the TrackableLocator - {trackable.gameObject.name}");
                return;
            }
            
            _trackables.Remove(trackable);
        }

        public List<Trackable> GetAllTrackables()
        {
            return _trackables;
        }

        public void WrapUp(bool isAppExit)
        {
            _trackables.Clear();
            ServiceLocator.Instance.Unregister<TrackableLocator>();
        }
    }
}
