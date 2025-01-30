using UnityEngine;
using UnityEngine.Serialization;

namespace Fog.Configs
{
    [CreateAssetMenu(menuName = "Content/Configs/Movement Config")]
    public class MovementConfig : ScriptableObject
    {
        [Header("Speeds")]
        public float WalkSpeed = 5f;
        public float RunMultiplier= 2f;
        [Range(0, 1)] public float AirControl = 0.5f;

        [Header("Jumping")]
        public float JumpHeight = 1.5f;
        public float Gravity = -9.81f;
        public float JumpCooldown = 0.2f;

        [Header("Look")]
        public float MouseSensitivity = 100f;
        public bool InvertY = false;
        public Vector2 VerticalLookLimits = new Vector2(-90f, 90f);

        [Header("Ground Check")]
        public float GroundCheckRadius = 0.4f;
        public float GroundCheckOffset = 0.1f;
    }
}