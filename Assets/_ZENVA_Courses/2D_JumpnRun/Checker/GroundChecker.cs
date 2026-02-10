using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GroundChecker : MonoBehaviour, ICheckable
{
    [SerializeField]
    private LayerMask groundLayerMask;
    [SerializeField, Range(0.01f, 0.1f)]
    private float circleCastDistance = 0.01f;

    private bool _isGrounded = false;
    private Collider2D _collider = null;

    private RaycastHit2D[] _hitBuffer;
    private ContactFilter2D _contactFilter;

    private readonly HashSet<Collider2D> _groundContactColliders = new();

    public IReadOnlyCollection<Collider2D> GroundContactColliders => _groundContactColliders;

    public bool IsGrounded
    {
        get => _isGrounded;
        private set
        {
            if (value != _isGrounded)
            {
                _isGrounded = value;
                GroundedStateChanged?.Invoke(_isGrounded);
            }
        }
    }

    public event Action<bool> GroundedStateChanged;


    private void Awake()
    {
        if (_collider == null && !this.TryGetComponent(out _collider))
        {
            Debug.LogError("Collider2D component is missing on " + this.gameObject.name);
            this.enabled = false;
        }

        _hitBuffer = new RaycastHit2D[8];
        _contactFilter = new ContactFilter2D
        {
            layerMask = groundLayerMask,
            useTriggers = false,
            useLayerMask = true
        };
    }

    private void Start()
    {
        _collider.isTrigger = true;
        CheckForStateChange();
    }

    private void FixedUpdate()
    {
        // Periodically clean up destroyed or inactive colliders
        int removedCount = _groundContactColliders.RemoveWhere(
            collider => collider == null || !collider.enabled || !collider.gameObject.activeInHierarchy
        );

        if (removedCount > 0 && _groundContactColliders.Count == 0)
            CheckForStateChange();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((groundLayerMask & (1 << other.gameObject.layer)) == 0 || other.isTrigger)
            return;

        _groundContactColliders.Add(other);
        IsGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((groundLayerMask & (1 << other.gameObject.layer)) == 0 || other.isTrigger)
            return;

        _groundContactColliders.Remove(other);

        if (_groundContactColliders.Count == 0)
            CheckForStateChange();
    }

    private void OnDrawGizmos()
    {
        if (_collider == null)
            return;

        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(
            _collider.bounds.center + Vector3.down * circleCastDistance,
            _collider.bounds.extents.x);
    }

    public void CheckForStateChange()
    {
        int hitCount = Physics2D.CircleCast(_collider.bounds.center, _collider.bounds.extents.x, Vector2.down, _contactFilter, _hitBuffer, circleCastDistance);

        IsGrounded = hitCount > 0;
    }
}
