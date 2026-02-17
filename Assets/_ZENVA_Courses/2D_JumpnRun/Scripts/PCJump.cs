using System;
using UnityEngine;

namespace JumpnRun
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PCJump : MonoBehaviour, IVerticalMoveable
    {
        private const float STANDARD_GRAVITY_SCALE = 1.0f;

        [SerializeField]
        private JumpnRunInputReader inputReader = null;
        [SerializeField, Min(0.1f)]
        private float jumpImpulse = 5.0f;
        [SerializeField, Min(STANDARD_GRAVITY_SCALE)]
        private float fallGravityScale = 2.0f;

        private Rigidbody2D _rigidbody = null;
        private GroundChecker _groundChecker = null;
        private bool _canJump = false;

        private void Awake()
        {
            if (!this.gameObject.TryGetComponent(out _rigidbody))
                Debug.LogError("Rigidbody2D component is missing on " + gameObject.name);

            if (!this.gameObject.TryGetComponent(out _groundChecker))
            {
                _groundChecker = this.gameObject.GetComponentInChildren<GroundChecker>();

                if (_groundChecker == null)
                    Debug.LogError("GroundChecker component is missing on " + gameObject.name);
            }
        }

        private void OnEnable()
        {
            _groundChecker.GroundedStateChanged += OnGroundedStateChanged;
            inputReader.JumpPressed += OnJumpPressed;
        }

        private void OnGroundedStateChanged(bool isGrounded)
        {
            _canJump = isGrounded;
        }

        private void FixedUpdate()
        {
            if (_rigidbody.linearVelocityY < 0.0f && _rigidbody.gravityScale < fallGravityScale)
            {
                _rigidbody.gravityScale = fallGravityScale;
            }

            if (_rigidbody.linearVelocityY >= 0.0f && _rigidbody.gravityScale > STANDARD_GRAVITY_SCALE)
            {
                _rigidbody.gravityScale = STANDARD_GRAVITY_SCALE;
            }
        }

        private void OnDisable()
        {
            inputReader.JumpPressed -= OnJumpPressed;
            _groundChecker.GroundedStateChanged -= OnGroundedStateChanged;
        }

        private void OnJumpPressed(bool isPressed)
        {
            if (!isPressed) return;
            if (!_canJump) return;

            MoveVertical(Vector2.up);
        }

        public void Move(Vector3 direction)
        {
            return;
        }

        public void MoveVertical(Vector2 verticalDirection)
        {
            _rigidbody.AddForce(verticalDirection * jumpImpulse, ForceMode2D.Impulse);
        }

    }
}