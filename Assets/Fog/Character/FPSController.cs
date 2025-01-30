using Fog.Configs;
using Fog.Services.Input;
using UnityEngine;
using Zenject;

namespace Fog.Character
{
    public class FPSPlayerController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Transform _playerCamera;
        [SerializeField] private AbstractGroundChecker _groundCheck;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private MovementConfig _movementConfig;

        private IInputService _inputService;
        
        private Vector3 _velocity;
        private bool _isGrounded;
        private float _xRotation;
        private float _jumpCooldownTimer;

        [Inject]
        public void Construct(IInputService inputService)
        {
            _inputService = inputService;
        }

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update()
        {
            HandleGroundCheck();
            HandleLook();
            HandleMovement();
            HandleJump();
            ApplyGravity();
        }

        private void HandleGroundCheck()
        {
            _isGrounded = _groundCheck.IsOnGround();
        }

        private void HandleLook()
        {
            var look = _inputService.GetMouseMove() * (_movementConfig.MouseSensitivity * Time.deltaTime);

            if (_movementConfig.InvertY)
            {
                look.y *= -1;
            }

            _xRotation -= look.y;
            _xRotation = Mathf.Clamp(_xRotation,
                _movementConfig.VerticalLookLimits.x,
                _movementConfig.VerticalLookLimits.y
            );

            _playerCamera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * look.x);
        }

        private void HandleMovement()
        {
            var input = _inputService.GetAxisMove();
            var move = transform.right * input.x + transform.forward * input.y;
            
            var multiplier = 1f;
            if (_inputService.GetSprint())
            {
                multiplier += _movementConfig.RunMultiplier;
            }

            var speed = _movementConfig.WalkSpeed * multiplier;

            var controlFactor = 1f;
            if (!_isGrounded)
            {
                controlFactor *= _movementConfig.AirControl;
            }

            _controller.Move(move * (speed * controlFactor * Time.deltaTime));
        }

        private void HandleJump()
        {
            _jumpCooldownTimer -= Time.deltaTime;

            if (_isGrounded && _inputService.GetJump() && _jumpCooldownTimer <= 0)
            {
                _velocity.y = Mathf.Sqrt(_movementConfig.JumpHeight * -2f * _movementConfig.Gravity);
                _jumpCooldownTimer = _movementConfig.JumpCooldown;
            }
        }

        private void ApplyGravity()
        {
            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            _velocity.y += _movementConfig.Gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }
    }
}