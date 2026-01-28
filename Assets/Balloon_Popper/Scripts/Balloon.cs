using System;
using System.Collections;
using UnityEngine;

namespace BalloonPopper
{
    public class Balloon : MonoBehaviour, IClickable
    {
        [SerializeField]
        private BalloonData balloonData;

        private int _counter = 0;
        private bool _isPopping = false;

        private Renderer _renderer;

        public static event Action<Balloon> BalloonPopped;

        private void Awake()
        {
            _renderer = this.GetComponentInChildren<Renderer>();

            if (_renderer == null)
            {
                Debug.LogErrorFormat("Renderer component not found on Balloon: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
            }
        }

        private void OnEnable()
        {
            TrySetCounter();
        }

        private void Start()
        {
            _renderer.material = balloonData.BalloonMaterial;
        }

        public void OnClick()
        {
            if (this.gameObject.activeInHierarchy)
            {
                if (_isPopping) return;

                _counter--;

                Vector3 scaleIncrease = new(balloonData.ScaleFactor, balloonData.ScaleFactor, balloonData.ScaleFactor);
                this.transform.localScale += scaleIncrease;

                if (_counter <= 0)
                {
                    _isPopping = true;
                    StartCoroutine(PopBalloon());
                }
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

            BalloonPopped?.Invoke(this);
            
            this.gameObject.SetActive(false);
            _isPopping = false;
            TrySetCounter();
        }

        private bool TrySetCounter()
        {
            if (balloonData == null)
            {
                Debug.LogErrorFormat("BalloonData is not assigned in Balloon script: {0} | ID: {1}", 
                    this.gameObject.name, 
                    this.gameObject.GetEntityId());

                return false;
            }

            if (balloonData.ClicksToPop <= 0)
            {
                Debug.LogErrorFormat("BalloonData has invalid ClicksToPop value in Balloon script: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());

                return false;
            }

            if (_counter != balloonData.ClicksToPop)
            {
                _counter = balloonData.ClicksToPop;
            }

            return true;
        }
    }
}
