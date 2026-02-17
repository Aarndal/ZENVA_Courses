using CollectibleSystem;
using InteractableSystem;
using SpawnSystem;
using System;
using UnityEngine;

using static IToggleable;

namespace JumpnRun
{
    [RequireComponent(typeof(Collider2D))]
    public class Coin : MonoBehaviour, ICollectible, ISpawnable, IInteractable, IScoreChanger
    {
        private CoinDataSO _data = null;
        private SpriteRenderer _spriteRenderer = null;
        private ToggleState _toggleState = ToggleState.On;

        public bool CanBeInteractedWith => this.gameObject.activeInHierarchy;
        public ISpawnableData Data => _data;
        public GameObject GameObject => this.gameObject;
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
            _toggleState = ToggleState.On;
            this.gameObject.SetActive(false);
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

            this.transform.position = spawnPosition;
            this.gameObject.SetActive(true);
        }

        public bool TryInitialize(ISpawnableData data)
        {
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

            return true;
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
            throw new NotImplementedException();
        }
    }

}