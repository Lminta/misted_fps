using UnityEngine;

namespace Fog.Services.Input
{
    public class PCControls : IInputService
    {
        public Vector2 GetAxisMove() => new Vector2(
                UnityEngine.Input.GetAxis("Horizontal"),
                UnityEngine.Input.GetAxis("Vertical")
                );

        public Vector2 GetMouseMove() => new Vector2(
                UnityEngine.Input.GetAxis("Mouse X"),
                UnityEngine.Input.GetAxis("Mouse Y")
                );
        

        public bool GetJump() => UnityEngine.Input.GetButtonDown("Jump");

        public bool GetFire() => UnityEngine.Input.GetButtonDown("Fire1");

        public bool GetAction() => UnityEngine.Input.GetButtonDown("Fire2");
        public bool GetSprint() => UnityEngine.Input.GetButtonDown("Fire3");
    }
}