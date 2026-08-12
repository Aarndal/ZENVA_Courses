using InteractableSystem;
using UnityEngine;

namespace JumpnRun
{
    [RequireComponent(typeof(Collider2D))]
    internal class CoinSpawner : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private Coin coin = null;
        [SerializeField, 
            Min(1), 
            Tooltip("How many coins this block can spawn before it is exhausted.")]
        private int numberOfSpawnables = 1;
        [SerializeField]
        private Vector3 spawnOffset = Vector3.up;

        private int _remainingSpawns = 0;

        public bool CanBeInteractedWith => coin != null && _remainingSpawns > 0;


        private void Awake()
        {
            if(coin == null)
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
    }
}
