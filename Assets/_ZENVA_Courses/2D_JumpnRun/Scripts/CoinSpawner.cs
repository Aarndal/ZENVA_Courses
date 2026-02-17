using InteractableSystem;
using SpawnSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JumpnRun
{
    internal class CoinSpawner : MonoBehaviour, ISpawner, IInteractable
    {
        [SerializeField]
        private Coin coin = null;

        public bool CanBeInteractedWith => throw new NotImplementedException();

        Queue<ISpawnerInstruction> ISpawner.Instructions => throw new NotImplementedException();

        event Action ISpawner.SpawningStarted
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

        event Action ISpawner.SpawningStopped
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

        public void StartSpawning()
        {
            coin.Spawn(this.transform.position + Vector3.up);
        }

        public void StopSpawning()
        {
            throw new System.NotImplementedException();
        }

        public bool TryInteract<T>(IInteractor interactor, T data = default)
        {
            if ((interactor.InteractorLayer & (1 << LayerMask.NameToLayer("PlayerCharacter"))) == 0)
            {
                return false;
            }
            StartSpawning();
            return true;
        }

        public bool TryInteract(IInteractor interactor)
        {
            throw new NotImplementedException();
        }

        bool ISpawner.TrySetInstructions(IEnumerable<ISpawnerInstruction> instructions)
        {
            throw new NotImplementedException();
        }
    }
}
