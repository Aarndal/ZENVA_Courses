using System;
using UnityEngine;

[RequireComponent(typeof(GroundChecker))]
public class SlopeChecker : MonoBehaviour, IChecker
{
    [SerializeField]
    private GroundChecker groundChecker = null;
    [SerializeField, Range(0.0f, 90.0f), Tooltip("Angles at or below this count as walkable ground, not a slope.")]
    private float maxWalkableAngle = 45.0f;

    private float _slopeAngle = 0.0f;
    private bool _isOnSlope = false;

    public float SlopeAngle => _slopeAngle;

    public bool IsOnSlope
    {
        get => _isOnSlope;
        private set
        {
            if (value != _isOnSlope)
            {
                _isOnSlope = value;
                SlopeStateChanged?.Invoke(_isOnSlope);
            }
        }
    }

    public event Action<bool> SlopeStateChanged;

    private void Awake()
    {
        if (groundChecker == null && !this.TryGetComponent(out groundChecker))
        {
            Debug.LogError("GroundChecker component is missing on " + this.gameObject.name);
            this.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (!groundChecker.IsGrounded)
        {
            _slopeAngle = 0.0f;
            IsOnSlope = false;
            return;
        }

        // Angle between the surface normal and straight up: 0 = flat, 90 = vertical wall.
        _slopeAngle = Vector2.Angle(groundChecker.SurfaceNormal, Vector2.up);
        IsOnSlope = _slopeAngle > maxWalkableAngle;
    }
}

