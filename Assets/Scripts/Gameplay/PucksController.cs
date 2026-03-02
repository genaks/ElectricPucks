using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private void EnableInput()
        {
            
        }

        private void PlayDataOne(InputAction.CallbackContext callbackContext)
        {
            ResetPucks();
            IReadOnlyList<FrameData> frameData = PuckTestDataLoader.LoadPuckData(1);
            _redEngineInputActions.Disable();
            StartCoroutine(StartPlayback(frameData));
        }

        private void PlayDataTwo(InputAction.CallbackContext callbackContext)
        {
            ResetPucks();
            IReadOnlyList<FrameData> frameData = PuckTestDataLoader.LoadPuckData(2);
            _redEngineInputActions.Disable();
            StartCoroutine(StartPlayback(frameData));
        }
        
        private void PlayDataThree(InputAction.CallbackContext callbackContext)
        {
            ResetPucks();
            IReadOnlyList<FrameData> frameData = PuckTestDataLoader.LoadPuckData(3);
            _redEngineInputActions.Disable();
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
            
            _redEngineInputActions.Enable();
        }
        
        private void UpdatePucks(IReadOnlyList<PuckData> puckData)
        {
            List<uint> puckIDs = _registeredPucks.Keys.ToList();
            foreach (var puck in puckData)
            {
                puckIDs.Remove(puck.PuckNumber);
                if (_registeredPucks.ContainsKey(puck.PuckNumber))
                {
                    _registeredPucks[puck.PuckNumber].SetTargetPosition(new Vector3(puck.X, 0, puck.Y));
                }
                else
                {
                    RegisterPuck(puck);
                    _registeredPucks[puck.PuckNumber].SetTargetPosition(new Vector3(puck.X, 0, puck.Y));
                }
                _registeredPucks[puck.PuckNumber].gameObject.SetActive(true);
            }

            foreach (var puckID in puckIDs) //all the pucks that aren't in the current frame data need to be set inactive
            {
                _registeredPucks[puckID].gameObject.SetActive(false);
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
                puck.gameObject.SetActive(true);
            }
            _registeredPucks.Clear();

        }
    }
}
