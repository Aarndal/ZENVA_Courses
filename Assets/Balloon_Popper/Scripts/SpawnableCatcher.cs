using UnityEngine;

namespace BalloonPopper
{
    // Catches balloons that have not been popped and returns them to the BalloonPool.
    public class SpawnableCatcher : MonoBehaviour
    {
        [SerializeField]
        private bool logCaughtBalloonsInEditor = false;


        private void OnCollisionEnter(Collision collision)
        {
            if (!TryGetSpawnable(collision, out var caughtBalloon))
                return;

#if UNITY_EDITOR
            if (logCaughtBalloonsInEditor)
            {
                Debug.LogFormat("Spawnable entered Catcher: {0}",
                        caughtBalloon.GetType().Name);
            }
#endif
            caughtBalloon.Despawn();
        }


        private bool TryGetSpawnable(Collision collision, out ISpawnable caughtBalloon)
        {
            if (!collision.transform.parent.TryGetComponent(out caughtBalloon))
            {
                caughtBalloon = collision.gameObject.GetComponentInParent<ISpawnable>(true);
            }
            return caughtBalloon != null;
        }
    }
}
