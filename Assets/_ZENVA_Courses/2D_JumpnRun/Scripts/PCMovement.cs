using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PCMovement : MonoBehaviour, IHorizontalMoveable
{
    [SerializeField]
    private JumpnRunInputReader inputReader = null;
    [SerializeField]
    private float moveSpeed = 5.0f;

    private Rigidbody2D _rigidbody = null;

    private void Awake()
    {
        if (!this.gameObject.TryGetComponent(out _rigidbody))
            Debug.LogError("Rigidbody2D component is missing on " + gameObject.name);
    }

    private void OnEnable()
    {
        inputReader.MovePerformed += OnMovePerformed;
        inputReader.MoveCanceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        inputReader.MovePerformed -= OnMovePerformed;
        inputReader.MoveCanceled -= OnMoveCanceled;
    }

    private void OnMoveCanceled(Vector2 vector)
    {
        MoveHorizontal(Vector2.zero);
    }

    private void OnMovePerformed(Vector2 vector)
    {
        MoveHorizontal(vector.normalized);
    }

    public void Move(Vector3 direction)
    {
        
    }

    public void MoveHorizontal(Vector2 horizontalDirection)
    {
        if (Mathf.Approximately(horizontalDirection.x, 0.0f))
        {
            _rigidbody.linearVelocityX = 0.0f;
            return;
        }

        if (Mathf.Abs(_rigidbody.linearVelocityX) > moveSpeed &&
            Mathf.Sign(horizontalDirection.x) == Mathf.Sign(_rigidbody.linearVelocityX))
        {
            return;
        }

        if (Mathf.Sign(horizontalDirection.x) != Mathf.Sign(_rigidbody.linearVelocityX))
            _rigidbody.linearVelocityX = 0.0f;

        _rigidbody.AddRelativeForce(horizontalDirection * moveSpeed, ForceMode2D.Impulse);
    }

}
