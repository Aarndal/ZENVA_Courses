using UnityEngine;

namespace InteractableSystem
{
    public interface IInteractor
    {
        LayerMask InteractorLayer { get; }

        GameObject GameObject { get; }
    }
}