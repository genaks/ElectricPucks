using System;
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

        public uint ID => _id;

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
        }

        public void SetData(uint id, PuckColour puckColour)
        {
            _id = id;
            meshRenderer.material = puckColour == PuckColour.Blue ? blueMaterial : pinkMaterial;
        }
        
        public void ResetData()
        {
            _id = 0;
        }

        public void SetTargetPosition(Vector3 targetPosition)
        {
            _targetPosition = targetPosition;
            _moving = true;
        }
    }
}
