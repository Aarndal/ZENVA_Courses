using UnityEngine;

namespace BalloonPopper
{
    [CreateAssetMenu(fileName = "BalloonData", menuName = "BalloonPopper/BalloonData")]
    public class BalloonData : ScriptableObject
    {
        [SerializeField]
        private Material balloonMaterial;
        [SerializeField]
        private int clicksToPop = 5;
        [SerializeField]
        private float initialScale = 1.0f;
        [SerializeField]
        private float scaleFactor = 0.2f;

        public Material BalloonMaterial => balloonMaterial;
        public int ClicksToPop => clicksToPop;
        public float InitialScale => initialScale;
        public float ScaleFactor => scaleFactor;
    }
}
