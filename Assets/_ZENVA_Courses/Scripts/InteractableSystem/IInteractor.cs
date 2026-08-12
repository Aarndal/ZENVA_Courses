using UnityEngine;

namespace InteractableSystem
{
    public interface IInteractor
    {
        /// <summary>
        /// The interactor's layer as a <see cref="LayerMask"/> bitmask (i.e. <c>1 << gameObject.layer</c>), so consumers can test it with a bitwise-AND against another mask. 
        /// This is NOT the raw layer index.
        /// </summary>
         LayerMask InteractorLayer { get; }

        /// <summary>The GameObject initiating the interaction.</summary>
         GameObject GameObject { get; }
    }
}