using System;
using System.Collections;
using UnityEngine;

namespace BalloonPopper
{
    public class Balloon : MonoBehaviour, IClickable
    {
        [SerializeField]
        private SOBalloonData balloonData;

        private int _counter = 0;
        private bool _isPopping = false;

        private Renderer _renderer;

        public static event Action<Balloon, SOBalloonData> BalloonPopped;

        private void Awake()
        {
            if (balloonData == null)
            {
                Debug.LogErrorFormat("BalloonData is not assigned in Balloon script: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
            }

            // The Renderer should be located on the Model child object
            _renderer = this.GetComponentInChildren<Renderer>(true);

            if (_renderer == null)
            {
                Debug.LogErrorFormat("Renderer component not found on Balloon: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
            }
        }

        private void OnEnable()
        {
            // If we fail to set the counter, deactivate the balloon
            if (!TrySetCounter())
            {
                this.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            // Initialize the balloon's material and scale
            if (balloonData != null && _renderer != null)
            {
                _renderer.material = balloonData.BalloonMaterial;
                this.transform.localScale = Vector3.one * balloonData.InitialScale;
            }
        }

        public void OnClick()
        {
            // Check if the balloon is active
            if (this.gameObject.activeInHierarchy)
            {
                // Ignore clicks if the balloon is already popping
                if (_isPopping) 
                    return;

                _counter--;

                this.transform.localScale += Vector3.one * balloonData.ScaleFactor;

                // Check if the balloon should start popping
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

            BalloonPopped?.Invoke(this, balloonData);

            this.gameObject.SetActive(false);
            _isPopping = false;
            TrySetCounter();
        }

        private bool TrySetCounter()
        {
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
