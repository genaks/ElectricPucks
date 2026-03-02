using System.Collections;
using System.Collections.Generic;
using Core;
using Data;
using RedEngine;
using RedEngine.Gameplay;
using Tools;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay
{
    public class PucksController : MonoBehaviour
    {
        [SerializeField] private float _playbackDuration = 10.0f;
        
        private TrackableLocator _trackableLocator;
        private Dictionary<uint, Puck> _registeredPucks = new ();

        private RedEngineInputActions _redEngineInputActions;

        private void Awake()
        {
            _redEngineInputActions = new RedEngineInputActions();
            _redEngineInputActions.RedEngineActionMap.PlayDataOne.performed += PlayDataOne;
            _redEngineInputActions.RedEngineActionMap.PlayDataTwo.performed += PlayDataTwo;
            _redEngineInputActions.RedEngineActionMap.PlayDataThree.performed += PlayDataThree;
        }
        
        private void Start()
        {
            ServiceLocator.Instance.TryGet(out _trackableLocator);
        }
        
        public void OnEnable()
        {
            _redEngineInputActions.Enable();
        }

        public void OnDisable()
        {
            _redEngineInputActions.Disable();
        }

        private void OnDestroy()
        {
            _redEngineInputActions.RedEngineActionMap.PlayDataOne.performed -= PlayDataOne;
            _redEngineInputActions.RedEngineActionMap.PlayDataTwo.performed -= PlayDataTwo;
            _redEngineInputActions.RedEngineActionMap.PlayDataThree.performed -= PlayDataThree;
        }

        private void PlayDataOne(InputAction.CallbackContext callbackContext)
        {
            ResetPucks();
            IReadOnlyList<FrameData> frameData = PuckTestDataLoader.LoadPuckData(1);
            StartCoroutine(StartPlayback(frameData));
        }

        private void PlayDataTwo(InputAction.CallbackContext callbackContext)
        {
            ResetPucks();
            IReadOnlyList<FrameData> frameData = PuckTestDataLoader.LoadPuckData(2);
            StartCoroutine(StartPlayback(frameData));
        }
        
        private void PlayDataThree(InputAction.CallbackContext callbackContext)
        {
            ResetPucks();
            IReadOnlyList<FrameData> frameData = PuckTestDataLoader.LoadPuckData(3);
            StartCoroutine(StartPlayback(frameData));
        }

        private IEnumerator StartPlayback(IReadOnlyList<FrameData> frameData)
        {
            float intervalDuration = _playbackDuration / frameData.Count;
            foreach (var frame in frameData)
            {
                UpdatePucks(frame.Pucks);
                yield return new WaitForSeconds(intervalDuration);
            }
        }
        
        private void UpdatePucks(IReadOnlyList<PuckData> puckData)
        {
            foreach (var puck in puckData)
            {
                if (_registeredPucks.ContainsKey(puck.PuckNumber))
                {
                    _registeredPucks[puck.PuckNumber].SetTargetPosition(new Vector3(puck.X, 0, puck.Y));
                }
                else
                {
                    RegisterPuck(puck);

                    _registeredPucks[puck.PuckNumber].SetTargetPosition(new Vector3(puck.X, 0, puck.Y));
                }
            }
        }

        private void RegisterPuck(PuckData puckData)
        {
            var trackables = _trackableLocator.GetAllTrackables();
            foreach (var trackable in trackables)
            {
                Puck puck = trackable.GetComponent<Puck>();
                if (puck.ID == 0)
                {
                    puck.SetData(puckData.PuckNumber, puckData.PuckColour);
                    _registeredPucks[puck.ID] = puck;
                    break;
                }
            }
        }

        private void ResetPucks()
        {
            foreach (var puck in _registeredPucks.Values)
            {
                puck.ResetData();
            }
            _registeredPucks.Clear();

        }
    }
}
