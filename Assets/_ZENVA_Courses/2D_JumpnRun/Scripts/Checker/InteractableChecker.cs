using InteractableSystem;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractableChecker : MonoBehaviour, IChecker<IInteractable>, IInteractor
{
    [SerializeField]
    private Collider2D checkerCollider = null;
    [SerializeField]
    private LayerMask interactableLayer;
    

    public LayerMask InteractorLayer => (1 << this.gameObject.layer);

    private void Awake()
    {
        if (checkerCollider == null && !this.TryGetComponent(out checkerCollider))
        {
            Debug.LogError("Collider2D component is missing on " + this.gameObject.name);
            this.enabled = false;
        }
    }

    private void Start()
    {
        if (checkerCollider != null)
            checkerCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((interactableLayer & (1 << other.gameObject.layer)) == 0 || other.isTrigger)
            return;

        if(!other.TryGetComponent<IInteractable>(out var interactableComponent))
            return;

        Check(interactableComponent);
    }

    public bool Check(IInteractable interactable)
    {
        return interactable.TryInteract(this);
    }
}
