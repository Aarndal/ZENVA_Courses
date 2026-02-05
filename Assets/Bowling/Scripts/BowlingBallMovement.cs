using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class BowlingBallMovement : MonoBehaviour, IMoveable
{
    [SerializeField]
    private float impulse = 10.0f;

    private bool _isRolling = false;

    private Rigidbody _rigidbody = null;
    private InputAction _shoot = null;

    private void Awake()
    {
        if (!TryGetComponent(out _rigidbody))
            Debug.LogError("Rigidbody component is missing on " + gameObject.name);


        _shoot = new InputAction(binding: "<Mouse>/leftButton");

    }

    private void OnEnable()
    {
        _isRolling = false;

        _shoot.performed += OnShoot;
        _shoot.Enable();
    }

    private void OnDisable()
    {
        _isRolling = false;

        _shoot.performed -= OnShoot;
        _shoot.Disable();
    }

    private void OnDestroy()
    {
        _shoot.Dispose();
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        Move();
    }

    public void Move()
    {
        if (!this.gameObject.activeInHierarchy)
            return;

        if (_isRolling)
            return;

        _rigidbody.AddForce(Vector3.forward * impulse, ForceMode.Impulse);

        _isRolling = true;
    }
}
