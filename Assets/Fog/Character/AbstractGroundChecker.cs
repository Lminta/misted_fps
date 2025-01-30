using UnityEngine;

namespace Fog.Character
{
    public abstract class AbstractGroundChecker : MonoBehaviour
    {
        public abstract bool IsOnGround();
    }
}