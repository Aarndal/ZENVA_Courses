namespace InteractableSystem
{
    public interface IInteractable
    {
        bool TryInteract<T>(IInteractor interactor, T data = default);
    }

    public interface IInteractable<T> : IInteractable where T : IDataProvider
    {
        bool TryInteract(IInteractor interactor, T data);
    }
}