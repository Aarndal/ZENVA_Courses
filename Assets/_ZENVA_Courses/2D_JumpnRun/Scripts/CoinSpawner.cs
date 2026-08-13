using InteractableSystem;
using SpawnSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpnRun
{
    [RequireComponent(typeof(Collider2D))]
    internal class CoinSpawner : MonoBehaviour, IInteractable, ISpawner
    {
        [SerializeField]
        private Coin2D coin = null;
        [SerializeField, Min(1)]
        [Tooltip("How many coins this block can spawn before it is exhausted.")]
        private int numberOfSpawnables = 1;
        [SerializeField]
        private List<ISpawnerInstruction> instructions = new List<ISpawnerInstruction>();
        [SerializeField]
        private Vector3 spawnOffset = Vector3.up;


        private int _remainingSpawns = 0;


        public bool CanBeInteractedWith => coin != null && _remainingSpawns > 0;
        public Queue<ISpawnerInstruction> Instructions => throw new NotImplementedException();


        public event Action SpawningStarted;
        public event Action SpawningStopped;


        private void Awake()
        {
            if (coin == null)
            {
                Debug.LogErrorFormat("Coin reference is missing on {0} | ID: {1}" +
                    "\nDisabling the component.",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                this.enabled = false;
                return;
            }

            _remainingSpawns = numberOfSpawnables;
        }

        public bool TryInteract(IInteractor interactor)
        {
            if ((interactor.InteractorLayer & (1 << LayerMask.NameToLayer("PlayerCharacter"))) == 0)
            {
                return false;
            }

            if (!CanBeInteractedWith)
            {
                return false;
            }

            coin.Spawn(this.transform.position + spawnOffset);
            _remainingSpawns--;

            return true;
        }

        public void StartSpawning()
        {
            throw new NotImplementedException();
        }

        public void StopSpawning()
        {
            throw new NotImplementedException();
        }

        public bool TrySetInstructions(IEnumerable<ISpawnerInstruction> instructions)
        {
            throw new NotImplementedException();
        }
    }
}
