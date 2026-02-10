using UnityEngine;
using UnityEngine.InputSystem;

public class PCJump : MonoBehaviour, IVerticalMoveable
{
    [SerializeField]
    private float moveSpeed = 5.0f;

    private Rigidbody2D _rigidbody = null;
    private PlayerInput _input = null;

    private void Awake()
    {
        if (!this.gameObject.TryGetComponent(out _rigidbody))
            Debug.LogError("Rigidbody2D component is missing on " + gameObject.name);

        if (!this.gameObject.TryGetComponent(out _input))
            Debug.LogError("PlayerInput component is missing on " + gameObject.name);

    }


    public void Move(Vector3 direction)
    {
        var isJumping = _input.actions["Jump"].ReadValue<float>();

        if (isJumping > 0)
            Move(Vector3.up);
    }

    public void MoveVertical(Vector2 verticalDirection)
    {
        _rigidbody.AddForce(verticalDirection * moveSpeed, ForceMode2D.Impulse);
    }

}
