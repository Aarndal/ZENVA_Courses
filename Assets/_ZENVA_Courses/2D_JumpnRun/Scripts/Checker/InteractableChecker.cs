using InteractableSystem;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractableChecker : MonoBehaviour, IChecker
{
    [SerializeField]
    private PC_Interaction interactor = null;
    [SerializeField]
    private Collider2D checkerCollider = null;
    [SerializeField]
    private LayerMask interactableLayer;
    

    public LayerMask InteractorLayer => (1 << this.gameObject.layer);

    private void Awake()
    {
        if (checkerCollider == null && !this.transform.TryGetComponentInChildren(out checkerCollider))
        {
            Debug.LogError("Collider2D component is missing on " + this.gameObject.name);
            this.enabled = false;
        }

        if (interactor == null && !this.TryGetComponent(out interactor) && !this.transform.parent.TryGetComponent(out interactor))
        {
            Debug.LogError("Interactor reference is missing on " + this.gameObject.name);
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
        if (!Check(other.gameObject))
            return;
    }

    public bool Check(GameObject gameObject)
    {
        if ((interactableLayer & (1 << gameObject.layer)) == 0)
            return false;

        if (!gameObject.TryGetComponent<IInteractable>(out var interactableComponent))
            return false;

        return interactableComponent.TryInteract(interactor);
    }
}
