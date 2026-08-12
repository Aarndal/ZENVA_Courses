using UnityEngine;

namespace JumpnRun
{
    public class Enemy2DMovement : MonoBehaviour
    {
        [SerializeField]
        private float translationSpeed = 1.0f;
        [SerializeField]
        private Vector2 moveOffset = Vector2.zero;

        private Vector3 _startPosition = Vector3.zero;
        private Vector3 _targetPosition = Vector3.zero;
        private float _sqrTravelDistance = 0.0f;

        public Vector3 MoveDirection => (_targetPosition - _startPosition).normalized;

        private void Start()
        {
            SetStartValues();
        }

        private void Update()
        {
            Move();
        }
        
        public void Move()
        {
            this.transform.position = Vector3.MoveTowards(
                            this.transform.position,
                            _targetPosition,
                            translationSpeed * Time.deltaTime);

            if (Vector3.SqrMagnitude(this.transform.position - _startPosition) >= _sqrTravelDistance)
            {
                (_startPosition, _targetPosition) = (_targetPosition, _startPosition);
            }
        }
        
        private void SetStartValues()
        {
            _startPosition = this.transform.position;
            _targetPosition = _startPosition + (Vector3)moveOffset;
            _sqrTravelDistance = Vector3.SqrMagnitude(_targetPosition - _startPosition);
        }
    }
}