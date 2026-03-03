using System;
using Core;
using Gameplay;
using UnityEngine;

namespace RedEngine.Gameplay
{
    public class Puck : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material blueMaterial;
        [SerializeField] private Material pinkMaterial;

        private uint _id;
        private Vector3 _targetPosition;
        private bool _moving;
        private PuckColour _puckColour;
        private LineRendererManager _lineRendererManager;

        public uint ID => _id;
        public PuckColour PuckColour => _puckColour;

        private void Start()
        {
            ServiceLocator.Instance.TryGet(out _lineRendererManager);
        }

        private void Update()
        {
            if (_moving)
            {
                transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime);
            }
        }

        private void OnDisable()
        {
            _moving = false;
            _lineRendererManager.UnregisterPuck(this);
        }

        public void Init(uint id, PuckColour puckColour)
        {
            _id = id;
            _puckColour = puckColour;
            meshRenderer.material = puckColour == PuckColour.Blue ? blueMaterial : pinkMaterial;
            ServiceLocator.Instance.TryGet(out _lineRendererManager);
            _lineRendererManager.RegisterPuck(this);
        }
        
        public void ResetData()
        {
            _id = 0;
            _lineRendererManager.UnregisterPuck(this);
        }

        public void SetTargetPosition(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
            _moving = true;
        }
    }
}
