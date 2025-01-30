using UnityEngine;

namespace Fog.Services.Input
{
    public interface IInputService
    {
        Vector2 GetAxisMove();
        Vector2 GetMouseMove();
        bool GetJump();
        bool GetFire();
        bool GetAction();
        bool GetSprint();
    }
}