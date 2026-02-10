using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GroundChecker : MonoBehaviour, ICheckable<LayerMask>
{
    public event Action<bool> GroundedStateChanged;

    [SerializeField]
    private LayerMask groundLayerMask;
    [SerializeField]
    private Collider2D _collider = null;
    [SerializeField, Range(0.01f, 0.1f)]
    private float circleCastDistance = 0.01f;

    private bool _isGrounded = false;

    public bool IsGrounded
    {
        get => _isGrounded;
        private set
        {
            if (value != _isGrounded)
            {
                _isGrounded = value;
                Debug.LogFormat("Grounded State Changed: " + _isGrounded);
                GroundedStateChanged?.Invoke(_isGrounded);
            }
        }
    }


    private void Awake()
    {
        if (!this.gameObject.TryGetComponent(out _collider))
            Debug.LogError("Collider2D component is missing on " + gameObject.name);
    }

    private void Start()
    {
        _collider.isTrigger = true;
        Check(0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Check(1 << other.gameObject.layer);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Check(1 << other.gameObject.layer);
    }

    private void OnDrawGizmos()
    {
        if (_collider == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_collider.bounds.center + Vector3.down * circleCastDistance, _collider.bounds.extents.x);
    }

    public bool Check(LayerMask layerMask)
    {

        var hit = Physics2D.CircleCast(_collider.bounds.center, _collider.bounds.extents.x, Vector2.down, circleCastDistance, groundLayerMask);

        return IsGrounded = hit.collider != null && !hit.collider.isTrigger;
    }
}
