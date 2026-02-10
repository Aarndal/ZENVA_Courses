using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GroundChecker : MonoBehaviour, IChecker
{
    [SerializeField]
    private Collider2D checkerCollider = null;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField, Range(0.01f, 0.1f)]
    private float castDistance = 0.01f;

    private bool _isGrounded = false;

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
        if (checkerCollider == null && !this.TryGetComponent(out checkerCollider))
        {
            Debug.LogError("Collider2D component is missing on " + this.gameObject.name);
            this.enabled = false;
        }

        _hitBuffer = new RaycastHit2D[8];
        _contactFilter = new ContactFilter2D
        {
            layerMask = groundLayer,
            useTriggers = false,
            useLayerMask = true
        };
    }

    private void Start()
    {
        if (checkerCollider != null)
            checkerCollider.isTrigger = true;

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
        if ((groundLayer & (1 << other.gameObject.layer)) == 0 || other.isTrigger)
            return;

        _groundContactColliders.Add(other);
        IsGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ((groundLayer & (1 << other.gameObject.layer)) == 0 || other.isTrigger)
            return;

        _groundContactColliders.Remove(other);

        if (_groundContactColliders.Count == 0)
            CheckForStateChange();
    }

    private void OnDrawGizmos()
    {
        if (checkerCollider == null)
            return;

        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(
            checkerCollider.bounds.center + Vector3.down * castDistance,
            checkerCollider.bounds.extents.x);
    }

    public void CheckForStateChange()
    {
        int hitCount = Physics2D.CircleCast(checkerCollider.bounds.center, checkerCollider.bounds.extents.x, Vector2.down, _contactFilter, _hitBuffer, castDistance);

        IsGrounded = hitCount > 0;
    }
}
