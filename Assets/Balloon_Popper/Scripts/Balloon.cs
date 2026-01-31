using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace BalloonPopper
{
    public class Balloon : MonoBehaviour, IClickable, ISpawnable
    {
        public static event Action<Balloon, BalloonDataSO> BalloonPopped;

        private int _counter = 0;
        private bool _isInitialized = false;
        private bool _isPopping = false;

        private BalloonDataSO _data;
        private Renderer _renderer;


        public BalloonDataSO Data => _data;
        public IObjectPool Pool { get; private set; }


        private void Awake()
        {
            // The Renderer should be located on the Model child object
            _renderer = this.GetComponentInChildren<Renderer>(true);

            if (_renderer == null)
            {
                Debug.LogErrorFormat("Renderer component not found on Spawnable: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
            }
        }

        private void OnDestroy()
        {
            _counter = 0;
            _isInitialized = false;
            _isPopping = false;
        }


        public async void OnClick()
        {
            // Check if the balloon is active and initialized
            if (this.gameObject.activeInHierarchy && _isInitialized)
            {
                // Ignore clicks if the balloon is already popping
                if (_isPopping)
                    return;

                _counter--;

                this.transform.localScale += Vector3.one * _data.ScaleFactor;

                // Check if the balloon should start popping
                if (_counter <= 0)
                    await PopBalloon();
            }
        }


        public void Despawn()
        {
            Pool.TryReturn(this);
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
            if (_data != null && _isInitialized)
            {
                this.transform.localScale = Vector3.one * _data.InitialScale;
            }
        }

        public bool TryInitialize(BalloonDataSO balloonData)
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

            _data = balloonData;
            _renderer.material = _data.Material;
            this.transform.localScale = Vector3.one * _data.InitialScale;
            _isInitialized = true;

            //Debug.LogFormat("Balloon initialized successfully: {0} | ID: {1}",
            //    this.gameObject.name,
            //    this.gameObject.GetEntityId());

            return true;
        }


        private async UniTask PopBalloon()
        {
            _isPopping = true;

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
                await UniTask.Yield();
            }

            // Snap exactly to the target to avoid tiny residual differences
            this.transform.localScale = popScale;

            BalloonPopped?.Invoke(this, _data);

            BalloonPool.Instance.TryReturn(this);

            _isPopping = false;

            await UniTask.CompletedTask;
        }

        private bool TryResetCounter()
        {
            if (_data.ClicksToPop <= 0)
            {
                Debug.LogErrorFormat("Data has invalid ClicksToPop value in Balloon script: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());

                return false;
            }

            if (_counter != _data.ClicksToPop)
            {
                _counter = _data.ClicksToPop;
            }

            return true;
        }

        public bool TryAssignPool(IObjectPool pool)
        {
            if (pool == null)
            {
                Debug.LogErrorFormat("Cannot assign null pool to Balloon: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            if (pool is not IObjectPool<Balloon, BalloonDataSO>)
            {
                Debug.LogErrorFormat("Assigned pool is of incorrect type for Balloon: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            Pool = pool;
            return true;
        }
    }
}
