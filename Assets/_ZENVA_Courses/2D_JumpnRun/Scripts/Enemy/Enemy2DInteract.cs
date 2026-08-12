using Debugging;
using InteractableSystem;
using UnityEngine;

namespace JumpnRun
{
    public class Enemy2DInteract : MonoBehaviour, IInteractor
    {
        [SerializeField]
        private LayerMask playerCharacterLayer = 0;

        private Collider2D _collider = null;

        public LayerMask InteractorLayer => this.gameObject.layer;
        public GameObject GameObject => this.gameObject;

        private void Awake()
        {
            if(!this.TryGetComponent(out _collider))
            {
                DebugLogger.Log(
                    LogMessageType.Error, 
                    this.transform, 
                    "Enemy2DInteract requires a Collider2D component to be attached to the same GameObject: {0}", 
                    true,
                    this.gameObject.name);
                return;
            }

            _collider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Check if the collided object is on the player character layer
            if ((playerCharacterLayer & (1 << collision.gameObject.layer)) == 0)
                return;

            // Check if the collided object has a component that implements IKillable
            if (!collision.TryGetComponent(out IKillable killable))
            {
                return;
            }

            killable.TryKill();
        }
    }
}
