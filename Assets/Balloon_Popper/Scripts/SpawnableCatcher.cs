using UnityEngine;

namespace BalloonPopper
{
    // Catches balloons that have not been popped and returns them to the BalloonPool.
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class SpawnableCatcher : MonoBehaviour
    {
        [SerializeField]
        private bool logCaughtBalloonsInEditor = false;


        private void OnTriggerEnter(Collider otherCollider)
        {
            if (!TryGetSpawnable(otherCollider, out var caughtBalloon))
                return;

#if UNITY_EDITOR
            if (logCaughtBalloonsInEditor)
            {
                Debug.LogFormat("Spawnable entered Catcher: {0}",
                        caughtBalloon.TypeName);
            }
#endif
            caughtBalloon.Despawn();
        }


        private bool TryGetSpawnable(Collider otherCollider, out ISpawnable caughtBalloon)
        {
            if (!otherCollider.transform.parent.TryGetComponent(out caughtBalloon))
            {
                caughtBalloon = otherCollider.gameObject.GetComponentInParent<ISpawnable>(true);
            }
            return caughtBalloon != null;
        }
    }
}
