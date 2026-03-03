using System;
using System.Collections.Generic;
using Core;
using RedEngine;
using RedEngine.Gameplay;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay
{
    public class LineRendererManager : MonoBehaviour, IGameService
    {
        [SerializeField] private Material lineRendererMaterial;
        [SerializeField] private float lineWidth = 0.1f;
        [SerializeField] private Color blueColor;
        [SerializeField] private Color pinkColor;

        private readonly List<Puck> _activePucks = new ();
        private List<LineRendererData> _lineRenderers = new ();
        private static readonly int Color1 = Shader.PropertyToID("_Color");

        private class LineRendererData
        {
            public Puck PuckOne;
            public Puck PuckTwo;
            public LineRenderer Line;
        }

        private void Awake()
        {
            ServiceLocator.Instance.Register(this);
        }

        public void RegisterPuck(Puck puck)
        {
            if (!_activePucks.Contains(puck))
            {
                _activePucks.Add(puck);
                RebuildConnections();
            }
        }

        public void UnregisterPuck(Puck puck)
        {
            if (_activePucks.Remove(puck))
            {
                RebuildConnections();
            }
        }

        private void RebuildConnections()
        {
            // Destroy old lines
            foreach (var lineRenderer in _lineRenderers)
            {
                if (lineRenderer.Line != null)
                    Destroy(lineRenderer.Line.gameObject);
            }

            _lineRenderers.Clear();

            // Create new connections
            for (int i = 0; i < _activePucks.Count; i++)
            {
                for (int j = i + 1; j < _activePucks.Count; j++)
                {
                    if (_activePucks[i].PuckColour == _activePucks[j].PuckColour)
                    {
                        CreateConnection(_activePucks[i], _activePucks[j]);
                    }
                }
            }
        }

        private void CreateConnection(Puck puckOne, Puck puckTwo)
        {
            GameObject lineObj = new GameObject($"Line_{puckOne.name}_{puckTwo.name}");
            lineObj.transform.parent = transform;

            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
            Material mat = new Material(lineRendererMaterial);
            lineRenderer.material = lineRendererMaterial;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.positionCount = 2;
            if (puckOne.PuckColour == PuckColour.Blue)
            {
                mat.SetColor(Color1, blueColor);
            }
            else
            {
                mat.SetColor(Color1, pinkColor);
            }

            _lineRenderers.Add(new LineRendererData
            {
                PuckOne = puckOne,
                PuckTwo = puckTwo,
                Line = lineRenderer
            });
        }

        private void Update()
        {
            // Update line positions every frame
            foreach (var lineRenderer in _lineRenderers)
            {
                if (lineRenderer.PuckOne != null && lineRenderer.PuckTwo != null)
                {
                    lineRenderer.Line.SetPosition(0, lineRenderer.PuckOne.transform.position);
                    lineRenderer.Line.SetPosition(1, lineRenderer.PuckTwo.transform.position);
                }
            }
        }

        private void OnDestroy()
        {
            WrapUp(false);
        }

        public void WrapUp(bool isAppExit)
        {
            ServiceLocator.Instance.Unregister<TrackableLocator>();
        }
    }
}