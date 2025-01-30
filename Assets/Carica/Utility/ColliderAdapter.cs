using System;
using UnityEngine;

namespace Carica.Utility
{
    public class ColliderAdapter : MonoBehaviour
    {
        public event Action<Collision> OnColliderEnterEvent;
        public event Action<Collision> OnColliderExitEvent;
        public event Action<Collision> OnColliderStayEvent;
        public event Action<Collider> OnTriggerEnterEvent;
        public event Action<Collider> OnTriggerExitEvent;
        public event Action<Collider> OnTriggerStayEvent;

        private void OnTriggerStay(Collider other)
        {
            OnTriggerStayEvent?.Invoke(other);
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterEvent?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExitEvent?.Invoke(other);
        }

        private void OnCollisionEnter(Collision other)
        {
            OnColliderEnterEvent?.Invoke(other);
        }

        private void OnCollisionExit(Collision other)
        {
            OnColliderExitEvent?.Invoke(other);
        }

        private void OnCollisionStay(Collision other)
        {
            OnColliderStayEvent?.Invoke(other);
        }
    }
}