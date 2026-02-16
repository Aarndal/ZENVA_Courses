using SpawnSystem;
using System;
using UnityEngine;

using static IToggleable;

namespace BalloonPopper
{
    public class Coin : MonoBehaviour, ISpawnable, IClickable, IScoreChanger
    {
        private bool _isInitialized = false;

        private CoinDataSO _data = null;
        private SpriteRenderer _spriteRenderer = null;
        private ToggleState _toggleState = ToggleState.On;

        public ISpawnableData Data => _data;
        public GameObject GameObject => this.gameObject;
        public string SpawnableType => _data.InstanceName;
        public ToggleState State => _toggleState;

        public int ScoreChangeValue => _data.ScoreValue;

        public event Func<ISpawnable, bool> DespawnRequested;

        private void Awake()
        {
            if (!this.transform.TryGetComponentInChildren(out _spriteRenderer))
            {
                Debug.LogErrorFormat("Renderer component not found on Spawnable or Children: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());

                return;
            }
        }

        private void OnDestroy()
        {
            _isInitialized = false;
            _toggleState = ToggleState.On;
            this.gameObject.SetActive(false);
        }


        public void Click()
        {
            // Check if the object is active and initialized
            if (this.gameObject.activeInHierarchy && _isInitialized)
            {
                // Ignore clicks if the spawnable's toggle state is not Off
                if (_toggleState != ToggleState.Off)
                    return;

                _toggleState = ToggleState.Pending;

                Debug.LogFormat("Clicked on {0} | ID: {1}", this.gameObject.name, this.gameObject.GetEntityId());

                Despawn();
                IScoreChanger.ScoreChanged?.Invoke(this);
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

            this.transform.position = spawnPosition;
        }

        public bool TryInitialize(ISpawnableData data)
        {
            // Don't allow re-initialization
            if (_isInitialized)
            {
                Debug.LogWarningFormat("Balloon is already initialized: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            if (data == null)
            {
                Debug.LogErrorFormat("Balloon initialization failed, missing data: {0} | Type: {1}",
                    data.InstanceName,
                    data.ProvidedType);
                return false;
            }

            if (_spriteRenderer == null)
            {
                Debug.LogErrorFormat("Balloon initialization failed, missing Renderer: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            return TryInitializeCoin(data);
        }

        private bool TryInitializeCoin(ISpawnableData data)
        {
            _data = data as CoinDataSO;

            if (_data == null)
            {
                Debug.LogErrorFormat(
                    "Spawnable initialization failed: {0} | ID: {1}" +
                    "incorrect data type: {2} | ID: {3}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId(),
                    data.InstanceName,
                    data.ID);
                return false;
            }

            if (_spriteRenderer != null)
                _spriteRenderer.sprite = _data.Sprite;

            return _isInitialized = true;
        }

        public bool TryToggle()
        {
            return true;
        }
    }

    public interface IScoreChanger
    {
        static Action<IScoreChanger> ScoreChanged;

        int ScoreChangeValue { get; }
    }
}
