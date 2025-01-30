using System;
using System.Collections.Generic;
using Carica.Utility;
using UnityEngine;

namespace Fog.Character
{
    public class ColliderGroundCheck : AbstractGroundChecker
    {
        [SerializeField] private string _filterTag;
        [SerializeField] private ColliderAdapter _collider;
        private readonly HashSet<Collider> _collidingObjects = new ();
        
        public override bool IsOnGround()
        {
            return _collidingObjects.Count > 0;
        }

        private void Start()
        {
            _collider.OnTriggerStayEvent += OnTriggerStayEvent;
            _collider.OnTriggerExitEvent += OnTriggerExitEvent;
        }

        private void OnTriggerStayEvent(Collider other)
        {
            if (other.CompareTag(_filterTag))
            {
                _collidingObjects.Add(other);
            }
        }

        private void OnTriggerExitEvent(Collider other)
        {
            _collidingObjects.Remove(other);
        }

        private void OnDestroy()
        {
            _collider.OnTriggerStayEvent -= OnTriggerStayEvent;
            _collider.OnTriggerExitEvent -= OnTriggerExitEvent;
        }
    }
}