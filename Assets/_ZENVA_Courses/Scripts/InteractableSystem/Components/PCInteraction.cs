using UnityEngine;

namespace InteractableSystem
{
    [RequireComponent(typeof(Collider2D))]
    public class PCInteraction : MonoBehaviour, IInteractor
    {
        [SerializeField]
        private Collider2D interactionCollider = null;

        private int _interactableLayer = -1;

        public LayerMask InteractorLayer => (1 << this.gameObject.layer);

        public GameObject GameObject => this.gameObject;

        private void Awake()
        {
            if (interactionCollider == null && !TryGetComponent(out interactionCollider))
            {
                Debug.LogErrorFormat("No collider found: {0} | ID: {1}" +
                    "\nDisabling the component.",
                    this.gameObject.name,
                    this.gameObject.GetEntityId());
                this.enabled = false;
                return;
            }

            interactionCollider.isTrigger = true;
            _interactableLayer = LayerMask.NameToLayer("Interactable");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != _interactableLayer)
            {
                return;
            }
            if (!other.gameObject.TryGetComponent(out IInteractable interactable))
            {
                return;
            }
            if (!interactable.CanBeInteractedWith)
            {
                return;
            }

            interactable?.TryInteract(this);
        }
    }
}