using System;
using System.Collections;
using UnityEngine;

namespace BalloonPopper
{
    public class Balloon : MonoBehaviour, IClickable, ISpawn
    {
        public static event Action<Balloon, SOBalloonData> BalloonPopped;


        [SerializeField]
        private SOBalloonData data;


        private int _counter = 0;
        private bool _isInitialized = false;
        private bool _isPopping = false;

        private Renderer _renderer;


        public SOBalloonData Data => data;


        private void Awake()
        {
            // The Renderer should be located on the Model child object
            _renderer = this.GetComponentInChildren<Renderer>(true);

            if (_renderer == null)
            {
                Debug.LogErrorFormat("Renderer component not found on Balloon: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
            }

            // Auto-initialize if data is already assigned
            if (data != null && !_isInitialized)
            {
                TryInitialize(data);
            }
        }

        private void OnDestroy()
        {
            _counter = 0;
        }


        public void OnClick()
        {
            // Check if the balloon is active and initialized
            if (this.gameObject.activeInHierarchy && _isInitialized)
            {
                // Ignore clicks if the balloon is already popping
                if (_isPopping)
                    return;

                _counter--;

                this.transform.localScale += Vector3.one * data.ScaleFactor;

                // Check if the balloon should start popping
                if (_counter <= 0)
                {
                    _isPopping = true;
                    StartCoroutine(PopBalloon());
                }
            }
        }


        public void Despawn()
        {
            BalloonPool.Instance.ReturnToPool(this);
        }

        public void Spawn(Vector3 spawnPosition)
        {
            // If we fail to reset the counter, return the balloon to the pool
            if (!TryResetCounter())
            {
                Despawn();
                return;
            }
            
            this.transform.position = spawnPosition;

            // Reset scale to initial value
            if (data != null && _isInitialized)
            {
                this.transform.localScale = Vector3.one * data.InitialScale;
            }
        }
        
        public bool TryInitialize(SOBalloonData balloonData)
        {
            // Don't allow re-initialization
            if (_isInitialized)
            {
                Debug.LogWarningFormat("Balloon is already initialized: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            if (balloonData == null)
            {
                Debug.LogErrorFormat("Balloon initialization failed, missing data: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            if (_renderer == null)
            {
                Debug.LogErrorFormat("Balloon initialization failed, missing Renderer: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            data = balloonData;
            _renderer.material = data.Material;
            this.transform.localScale = Vector3.one * data.InitialScale;
            _isInitialized = true;

            //Debug.LogFormat("Balloon initialized successfully: {0} | ID: {1}",
            //    this.gameObject.name,
            //    this.gameObject.GetEntityId());

            return true;
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

            BalloonPopped?.Invoke(this, data);

            BalloonPool.Instance.ReturnToPool(this);

            _isPopping = false;
        }

        private bool TryResetCounter()
        {
            if (data.ClicksToPop <= 0)
            {
                Debug.LogErrorFormat("Data has invalid ClicksToPop value in Balloon script: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());

                return false;
            }

            if (_counter != data.ClicksToPop)
            {
                _counter = data.ClicksToPop;
            }

            return true;
        }
        
    }
}
