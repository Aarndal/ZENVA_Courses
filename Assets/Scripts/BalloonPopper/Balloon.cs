using System;
using System.Collections;
using UnityEngine;

namespace BalloonPopper
{
    public class Balloon : MonoBehaviour
    {
        [SerializeField]
        private float scaleFactor = 0.2f;
        [SerializeField]
        private int clicksToPop = 5;

        private bool _isPopping = false;

        public static event Action<Balloon> BalloonPopped;

        private void OnMouseDown()
        {
            if (_isPopping) return;

            clicksToPop--;

            Vector3 scalteIncrease = new(scaleFactor, scaleFactor, scaleFactor);
            this.transform.localScale += scalteIncrease;

            if (clicksToPop <= 0)
            {
                Pop();
            }
        }

        private void Pop()
        {
            if (this.gameObject.activeInHierarchy)
            {
                _isPopping = true;

                BalloonPopped?.Invoke(this);

                StartCoroutine(PopBalloon());
            }
        }

        private IEnumerator PopBalloon()
        {
            // Compute the target scale
            Vector3 popScale = this.transform.localScale * 1.25f;

            // Speed in scale-units per second (tweak as needed)
            const float scaleSpeed = 0.75f;

            // Squared epsilon for comparison to avoid using exact equality
            const float sqrEpsilon = 0.0001f;

            // Animate towards the target scale using MoveTowards to guarantee progress
            while (Vector3.SqrMagnitude(this.transform.localScale - popScale) > sqrEpsilon)
            {
                this.transform.localScale = Vector3.MoveTowards(this.transform.localScale, popScale, scaleSpeed * Time.deltaTime);
                yield return null;
            }

            // Snap exactly to the target to avoid tiny residual differences
            this.transform.localScale = popScale;

            // Add pop animation or effects here if needed
            // yield return new WaitForSeconds(0.5f); // Simulate some delay for popping effect

            this.gameObject.SetActive(false);
            _isPopping = false;
        }
    }
}
