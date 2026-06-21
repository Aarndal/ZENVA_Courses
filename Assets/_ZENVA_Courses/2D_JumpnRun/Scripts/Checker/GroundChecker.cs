using System;
using UnityEngine;

public class GroundChecker : MonoBehaviour, IChecker
{
    public enum CastShape : byte
    {
        Ray,
        Circle,
        Capsule,
        Box
    }

    public enum CastDirection : sbyte
    {
        Left = -2,
        Down = -1,
        Up = 1,
        Right = 2
    }

    [Header("Cast Settings")]
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private CastShape castShape = CastShape.Box;
    [SerializeField,
        Tooltip("Cast origin offset from this object's position, in world units.")]
    private Vector2 originOffset = Vector2.zero;
    [SerializeField]
    private CastDirection castDirection = CastDirection.Down;
    [SerializeField,
        Range(-90.0f, 90.0f),
        Tooltip("Rotates the cast direction between -90 and +90 degrees. " +
        "Negative values rotate clockwise, positive ones rotate counter-clockwise.")]
    private float castAngle = 0.0f;
    [SerializeField, Min(0.0f)]
    private float castDistance = 0.1f;

    [Header("Shape Size")]
    [SerializeField,
        Min(0.0f),
        Tooltip("Used by the Circle shape.")]
    private float castRadius = 0.25f;
    [SerializeField,
        Tooltip("Used by the Box and Capsule shapes.")]
    private Vector2 castSize = new(0.5f, 0.1f);
    [SerializeField,
        Range(-90.0f, 90.0f),
        Tooltip("Rotates a Box or Capsule shape around its center.")]
    private float castTilt = 0.0f;
    [SerializeField,
        Tooltip("Used by the Capsule shape.")]
    private CapsuleDirection2D capsuleDirection = CapsuleDirection2D.Horizontal;


    private bool _isGrounded = false;

    private RaycastHit2D[] _hitBuffer;
    private ContactFilter2D _contactFilter;


    /// <summary>
    /// The collider the most recent cast landed on; null when not grounded.
    /// </summary>
    public Collider2D Ground { get; private set; }

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

    /// <summary>
    /// Normal of the surface the most recent cast hit; Vector2.zero when not grounded.
    /// </summary>
    public Vector2 SurfaceNormal { get; private set; }


    public event Action<bool> GroundedStateChanged;


    private void Awake()
    {
        _hitBuffer = new RaycastHit2D[8];
        _contactFilter = new ContactFilter2D
        {
            layerMask = groundLayer,
            useTriggers = false,
            useLayerMask = true
        };
    }

    // Establish the correct state before the first physics step so subscribers that hooked up in OnEnable see an accurate initial value.
    private void Start() => Check();

    private void FixedUpdate() => Check();

    private void OnDrawGizmos()
    {
        Vector2 origin = (Vector2)transform.position + originOffset;
        Vector2 direction = ResolveCastDirection();
        Vector2 end = origin + direction * castDistance;

        Gizmos.color = _isGrounded ? Color.red : Color.green;

        switch (castShape)
        {
            case CastShape.Ray:
                {
                    Gizmos.DrawLine(origin, end);
                    break;
                }
            case CastShape.Circle:
                {
                    // Draw spheres at start and end point.
                    Gizmos.DrawWireSphere(origin, castRadius);
                    Gizmos.DrawWireSphere(end, castRadius);

                    // Draw lines connecting the spheres.
                    Vector2 perpendicular = Vector2.Perpendicular(direction) * castRadius;
                    Gizmos.DrawLine(origin + perpendicular, end + perpendicular);
                    Gizmos.DrawLine(origin - perpendicular, end - perpendicular);
                    break;
                }
            case CastShape.Box:
                {
                    Matrix4x4 previousMatrix = Gizmos.matrix;
                    Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, castTilt);

                    Gizmos.matrix = Matrix4x4.TRS(origin, rotation, Vector3.one);
                    Gizmos.DrawWireCube(Vector3.zero, castSize);

                    Gizmos.matrix = Matrix4x4.TRS(end, rotation, Vector3.one);
                    Gizmos.DrawWireCube(Vector3.zero, castSize);

                    Gizmos.matrix = previousMatrix;
                    break;
                }
            case CastShape.Capsule:
                {
                    Matrix4x4 previousMatrix = Gizmos.matrix;
                    Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, castTilt);

                    Gizmos.matrix = Matrix4x4.TRS(origin, rotation, Vector3.one);
                    DrawWireCapsule2D(castSize, capsuleDirection);

                    Gizmos.matrix = Matrix4x4.TRS(end, rotation, Vector3.one);
                    DrawWireCapsule2D(castSize, capsuleDirection);

                    Gizmos.matrix = previousMatrix;
                    break;
                }
        }
    }

    public void Check()
    {
        int hitCount = PerformCast();

        if (hitCount > 0)
        {
            Ground = _hitBuffer[0].collider;
            SurfaceNormal = _hitBuffer[0].normal;
        }
        else
        {
            Ground = null;
            SurfaceNormal = Vector2.zero;
        }

        IsGrounded = hitCount > 0;
    }

    private int PerformCast()
    {
        Vector2 origin = (Vector2)transform.position + originOffset;
        Vector2 direction = ResolveCastDirection();

        return castShape switch
        {
            CastShape.Ray => Physics2D.Raycast(
                origin, direction, _contactFilter, _hitBuffer, castDistance),

            CastShape.Circle => Physics2D.CircleCast(
                origin, castRadius, direction, _contactFilter, _hitBuffer, castDistance),

            CastShape.Capsule => Physics2D.CapsuleCast(
                origin, castSize, capsuleDirection, castTilt, direction, _contactFilter, _hitBuffer, castDistance),

            CastShape.Box => Physics2D.BoxCast(
                origin, castSize, castTilt, direction, _contactFilter, _hitBuffer, castDistance),

            _ => 0
        };
    }

    private Vector2 ResolveCastDirection()
    {
        Vector2 baseDirection = castDirection switch
        {
            CastDirection.Up => Vector2.up,
            CastDirection.Down => Vector2.down,
            CastDirection.Left => Vector2.left,
            CastDirection.Right => Vector2.right,
            _ => Vector2.down
        };

        return (Vector2)(Quaternion.Euler(0.0f, 0.0f, castAngle) * baseDirection);
    }

    private static void DrawWireCapsule2D(Vector2 size, CapsuleDirection2D direction)
    {
        if (direction == CapsuleDirection2D.Vertical)
        {
            float radius = size.x * 0.5f;
            float capOffset = Mathf.Max(0.0f, size.y * 0.5f - radius);

            Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, size.y - 2.0f * radius, 0.0f));
            Gizmos.DrawWireSphere(Vector3.up * capOffset, radius);
            Gizmos.DrawWireSphere(Vector3.down * capOffset, radius);
        }
        else // Horizontal
        {
            float radius = size.y * 0.5f;
            float capOffset = Mathf.Max(0.0f, size.x * 0.5f - radius);

            Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x - 2.0f * radius, size.y, 0.0f));
            Gizmos.DrawWireSphere(Vector3.right * capOffset, radius);
            Gizmos.DrawWireSphere(Vector3.left * capOffset, radius);
        }
    }
}