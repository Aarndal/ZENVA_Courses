using CollectibleSystem;
using Core;
using Debugging;
using InteractableSystem;
using SpawnSystem;
using System;
using UnityEngine;

using static Core.IToggleable;

namespace JumpnRun
{
    [RequireComponent(typeof(Collider2D))]
    public class Coin2D : MonoBehaviour, ICollectible, ISpawnable, IInteractable, IScoreChanger
    {
        [SerializeField]
        private Coin2DDataSO data = null;


        private SpriteRenderer _spriteRenderer = null;
        private ToggleState _toggleState = ToggleState.Off;


        public bool CanBeInteractedWith => this.gameObject.activeInHierarchy;
        public ISpawnableData Data => data;
        public GameObject GameObject => this.gameObject;
        /// <summary>
        /// State of the coin, whether it is active (On) or inactive (Off).
        /// </summary>
        public ToggleState CurrentState => _toggleState;
        public int ScoreChangeValue
        {
            get;
            protected set;
        }

        bool IToggleable.CanToggle => throw new NotImplementedException();

        bool ICollectible.CanBeCollected => throw new NotImplementedException();

        public event Func<ISpawnable, bool> DespawnRequested;

        event Action<ToggleState> IToggleable.StateChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }


        #region Unity Lifecycle Methods
        private void Awake()
        {
            if (!this.transform.TryGetComponentInChildren(out _spriteRenderer))
            {
                DebugLogger.Log(
                    LogMessageType.ErrorFormatted, 
                    this.gameObject, 
                    "Renderer component not found on Spawnable or Children: {0} | ID: {1}",
                    true,
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                
                return;
            }

            _toggleState = ToggleState.Off;
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            DespawnRequested += OnDespawnRequested;
        }

        private void OnDisable()
        {
            DespawnRequested -= OnDespawnRequested;
        }

        private void OnDestroy()
        {
            _toggleState = ToggleState.Off;
            this.gameObject.SetActive(false);
        }
        #endregion


                #region Callback Functions
        private bool OnDespawnRequested(ISpawnable spawnable)
        {
            if (spawnable.GameObject != this.gameObject)
                return false;

            _toggleState = ToggleState.Off;
            this.gameObject.SetActive(false);
            return true;
        }
        #endregion


        #region Public Methods
        public void Despawn()
        {
            if (DespawnRequested?.Invoke(this) == true)
            {
                _toggleState = ToggleState.Off;
                this.gameObject.SetActive(false);
            }
        }

        public void Spawn(Vector3 spawnPosition, ISpawnContext context = null)
        {
            _toggleState = ToggleState.On;

            this.transform.position = spawnPosition;
            this.gameObject.SetActive(true);
        }

        public bool TryInitialize(ISpawnableData data)
        {
            if (data == null)
            {
                Debug.LogErrorFormat("Coin initialization failed, missing data: {0} | Type: {1}",
                    data.InstanceName,
                    data.ProvidedType);
                return false;
            }

            if (_spriteRenderer == null)
            {
                Debug.LogErrorFormat("Coin initialization failed, missing Renderer: {0} | ID: {1}",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                return false;
            }

            return TryInitializeCoin(data);
        }

        public bool TryToggle()
        {
            throw new System.NotImplementedException();
        }

        public bool TryInteract(IInteractor interactor)
        {
            if ((interactor.InteractorLayer & (1 << LayerMask.NameToLayer("PlayerCharacter"))) == 0)
            {
                return false;
            }

            if(!interactor.GameObject.transform.TryGetComponentInChildren(out ICollector collector))
            {
                return false;
            }

            return TryCollect(collector);
        }

        public bool TryCollect(ICollector collector)
        {
            if(collector.TryAdd(this))
            {
                Despawn();
                return true;
            }
            return false;
        }
        #endregion


        #region Private Methods
        private bool TryInitializeCoin(ISpawnableData data)
        {
            this.data = data as Coin2DDataSO;

            if (this.data == null)
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
                _spriteRenderer.sprite = this.data.Sprite;

            ScoreChangeValue = Math.Max(Coin2DDataSO.DEFAULT_SCORE_VALUE, this.data.ScoreValue);

            return true;
        }

        bool IToggleable.TryToggle(object requester)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}