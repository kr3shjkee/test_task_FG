using UnityEngine;
using Zenject;

namespace Game
{
    public class Line : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<Line>
        {
        }

        private LineRenderer _lineRenderer;
        
        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public void Initialize(Vector3 pos1, Vector3 pos2)
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, pos1);
            _lineRenderer.SetPosition(1, pos2);
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}