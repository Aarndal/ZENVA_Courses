using InteractableSystem;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PC_Interaction : MonoBehaviour, IInteractor
{
    [SerializeField]
    private Collider2D interactionCollider = null;

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
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Interactable"))
        {
            return;
        }
        if (!other.gameObject.TryGetComponent(out IInteractable interactable))
        {
            return;
        }

        interactable?.TryInteract(this);
    }
}
