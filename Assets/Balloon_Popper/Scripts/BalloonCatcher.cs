using UnityEngine;

namespace BalloonPopper
{
    // Catches balloons that have not been popped and returns them to the BalloonPool.
    public class BalloonCatcher : MonoBehaviour
    {
        [SerializeField]
        private bool logCaughtBalloonsInEditor = false;


        private void OnCollisionEnter(Collision collision)
        {
            if (!TryGetCaughtBalloon(collision, out var caughtBalloon))
                return;

#if UNITY_EDITOR
            if (logCaughtBalloonsInEditor)
            {
                Debug.LogFormat("Balloon entered BalloonCatcher: {0} | ID: {1}",
                        caughtBalloon.gameObject.name,
                        caughtBalloon.gameObject.GetEntityId());
            }
#endif
            caughtBalloon.Despawn();
        }


        private bool TryGetCaughtBalloon(Collision collision, out Balloon caughtBalloon)
        {
            caughtBalloon = collision.gameObject.GetComponentInParent<Balloon>(true);
            return caughtBalloon != null;
        }
    }
}
