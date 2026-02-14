using Cysharp.Threading.Tasks;
using SpawnSystem;
using System;
using UnityEngine;

using static IToggleable;

namespace BalloonPopper
{
    public class Balloon : MonoBehaviour, ISpawnable, IClickable
    {
        // Public Static Events
        public static event Action<ISpawnable, BalloonDataSO> BalloonPopped;
        public event Func<ISpawnable, bool> DespawnRequested;

        // Private Member Variables
        private int _counter = 0;
        private bool _isInitialized = false;
        private ToggleState _toggleState = ToggleState.On;

        private BalloonDataSO _data = null;
        private Renderer _renderer = null;

        // Properties
        public ISpawnableData Data => _data;
        public GameObject GameObject => this.gameObject;
        public string SpawnableType => _data.InstanceName;

        public ToggleState State => _toggleState;

        #region Unity Lifecycle Methods
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
            _toggleState = ToggleState.On;
            this.gameObject.SetActive(false);
        }
        #endregion


        #region Public Methods
        public async void Click()
        {
            // Check if the balloon is active and initialized
            if (this.gameObject.activeInHierarchy && _isInitialized)
            {
                // Ignore clicks if the balloon's toggle state is not Off
                if (_toggleState != ToggleState.Off)
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
            if (DespawnRequested?.Invoke(this) == true)
            {
                _toggleState = ToggleState.On;
                this.gameObject.SetActive(false);
            }
        }

        public void Spawn(Vector3 spawnPosition, ISpawnContext context = null)
        {
            _toggleState = ToggleState.Off;

            this.gameObject.SetActive(true);

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

        public bool TryInitialize(ISpawnableData dataProvider)
        {
            // Don't allow re-initialization
            if (_isInitialized)
            {
                Debug.LogWarningFormat("Balloon is already initialized: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            if (dataProvider == null)
            {
                Debug.LogErrorFormat("Balloon initialization failed, missing data: {0} | Type: {1}",
                    dataProvider.InstanceName,
                    dataProvider.ProvidedType);
                return false;
            }

            if (_renderer == null)
            {
                Debug.LogErrorFormat("Balloon initialization failed, missing Renderer: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            return TryInitializeBalloon(dataProvider);
        }

        private bool TryInitializeBalloon(ISpawnableData data)
        {
            _data = (BalloonDataSO)data;

            if (_data == null)
            {
                Debug.LogErrorFormat("Balloon initialization failed, incorrect data type: {0} | Type: {1}",
                    data.InstanceName,
                    data.ProvidedType);
                return false;
            }

            _renderer.material = _data.Material;
            this.transform.localScale = Vector3.one * _data.InitialScale;

            return _isInitialized = true;
        }
        #endregion


        #region Private Methods
        private async UniTask PopBalloon()
        {
            _toggleState = ToggleState.Pending;
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
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            // Snap exactly to the target to avoid tiny residual differences
            this.transform.localScale = popScale;

            BalloonPopped?.Invoke(this, _data);

            Despawn();
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

        public bool TryToggle()
        {
            return true;
        }
        #endregion
    }
}
