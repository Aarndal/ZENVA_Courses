using UnityEngine;

namespace SpawnSystem
{
    // Catches balloons that have not been popped and returns them to the BalloonPool.
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class SpawnableCatcher : MonoBehaviour
    {
        private void OnTriggerEnter(Collider otherCollider)
        {
            if (!TryGetSpawnable(otherCollider, out var caughtBalloon))
                return;

            caughtBalloon.Despawn();
        }


        private bool TryGetSpawnable(Collider otherCollider, out ISpawnable caughtBalloon)
        {
            if (!otherCollider.TryGetComponent(out caughtBalloon))
            {
                caughtBalloon = otherCollider.GetComponentInChildren<ISpawnable>(true);
            }
            return caughtBalloon != null;
        }
    }
}
