using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GroundChecker : MonoBehaviour, IChecker
{
    [SerializeField]
    private Collider2D checkerCollider = null;

    [Space(10)]

    [Header("Cast Settings")]
    [SerializeField]
    private LayerMask groundLayer;
    //[SerializeField, Range(0.1f, 1.0f)]
    //private float castRadius = 0.1f;
    //[SerializeField, Range(0.1f, 1.0f)]
    //private float castHeight = 0.1f;
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

        Gizmos.color = _isGrounded ? Color.red : Color.green;

        switch (checkerCollider)
        {
            case BoxCollider2D boxCollider:
                {
                    Gizmos.DrawWireCube(
                        boxCollider.bounds.center + Vector3.down * castDistance,
                        new Vector3(boxCollider.bounds.size.x, boxCollider.bounds.size.y, 0.1f));
                    break;
                }
                case CircleCollider2D circleCollider:
                {
                    Gizmos.DrawWireSphere(
                        circleCollider.bounds.center + Vector3.down * castDistance,
                        circleCollider.bounds.extents.x);
                    break;
                }
                case CapsuleCollider2D capsuleCollider:
                {
                    Gizmos.DrawWireCube(
                        capsuleCollider.bounds.center + Vector3.down * castDistance,
                        new Vector3(capsuleCollider.bounds.size.x, capsuleCollider.bounds.size.y, 0.1f));
                    break;
                }
                default:
                {
                    Debug.LogErrorFormat("Unsupported Collider2D type on Checker: {0} | ID: {1}",
                        this.gameObject.name,
                        this.GetEntityId());
                    return;
                }
        }
    }

    public void CheckForStateChange()
    {
        if (checkerCollider == null)
            return;

        int hitCount;

        switch (checkerCollider)
        {
            case BoxCollider2D boxCollider:
                {
                    hitCount = Physics2D.BoxCast(
                        origin: boxCollider.bounds.center,
                        size: boxCollider.bounds.size,
                        angle: 0.0f,
                        direction: Vector2.down,
                        contactFilter: _contactFilter,
                        results: _hitBuffer,
                        distance: castDistance);
                    break;
                }
            case CircleCollider2D circleCollider:
                {
                    hitCount = Physics2D.CircleCast(
                        origin: circleCollider.bounds.center,
                        radius: circleCollider.bounds.extents.x,
                        direction: Vector2.down,
                        contactFilter: _contactFilter,
                        results: _hitBuffer,
                        distance: castDistance);
                    break;
                }
            case CapsuleCollider2D capsuleCollider:
                {
                    hitCount = Physics2D.CapsuleCast(
                        origin: capsuleCollider.bounds.center,
                        size: capsuleCollider.bounds.size,
                        capsuleDirection: capsuleCollider.direction,
                        angle: 0.0f,
                        direction: Vector2.down,
                        contactFilter: _contactFilter,
                        results: _hitBuffer,
                        distance: castDistance);
                    break;
                }
            default:
                {
                    Debug.LogErrorFormat("Unsupported Collider2D type on Checker: {0} | ID: {1}",
                        this.gameObject.name,
                        this.GetEntityId());
                    return;
                }
        }

        IsGrounded = hitCount > 0;
    }
}
