using System;
using Carica.Utility;
using UnityEngine;

namespace Fog.Level
{
    public abstract class AbstractPlate : MonoBehaviour
    {
        protected const string DESTROY_TAG = "DestractionBorder";
        protected const string CREATE_TAG = "CreationBorder";

        [SerializeField] private ColliderAdapter _border;
        
        protected Action<AbstractPlate> _createNextCallback;
        protected Action<AbstractPlate> _destroyCallback;
        protected Vector2Int _coordinates;
        protected bool _isInbound = true;

        public Vector2Int Coordinates => _coordinates;

        public void Setup(Vector2Int coordinates, Action<AbstractPlate> createCallback, Action<AbstractPlate> destroyCallback)
        {
            _createNextCallback = createCallback;
            _destroyCallback = destroyCallback;
            _coordinates = coordinates;
        }

        private void Start()
        {
            _border.OnTriggerStayEvent += OnBorderStay;
            _border.OnTriggerExitEvent += OnBorderExit;
            _border.OnTriggerEnterEvent += OnBorderEnter;
        }

        private void Update()
        {
            CheckIfInbound();
        }

        private void OnDestroy()
        {
            _border.OnTriggerStayEvent -= OnBorderStay;
            _border.OnTriggerExitEvent -= OnBorderExit;
            _border.OnTriggerEnterEvent -= OnBorderEnter;
        }

        private void OnBorderEnter(Collider other)
        {
            if (other.CompareTag(DESTROY_TAG))
                _isInbound = true;
            else if (other.CompareTag(CREATE_TAG))
                _createNextCallback?.Invoke(this);
        }
        
        private void OnBorderStay(Collider other)
        {    
            if (other.CompareTag(DESTROY_TAG))
                _isInbound = true;
        }

        private void OnBorderExit(Collider other)
        {
            if (other.CompareTag(DESTROY_TAG))
                _isInbound = false;
        }

        protected virtual void CheckIfInbound()
        {
            if (!_isInbound)
            {
                _destroyCallback?.Invoke(this);
            }
        }
    }
}