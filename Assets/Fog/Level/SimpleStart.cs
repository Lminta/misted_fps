using UnityEngine;

namespace Fog.Level
{
    public class SimpleStart : SimplePlate
    {
        private void Start()
        {
            _createNextCallback(this);
        }

        protected override void CheckIfInbound()
        {
            Debug.Log("Simple start");
            base.CheckIfInbound();
        }
    }
}