namespace InteractableSystem
{
    public interface IInteractable 
    {
        bool CanBeInteractedWith { get; }

        bool TryInteract(IInteractor interactor);
    }
}