using UnityEngine;

namespace BalloonPopper
{
    [CreateAssetMenu(fileName = "BalloonData", menuName = "BalloonPopper/BalloonData")]
    public class SOBalloonData : ScriptableObject
    {
        [SerializeField]
        private Material balloonMaterial = default;

        [Space(10)]

        [SerializeField]
        private int clicksToPop = 5;
        [SerializeField]
        private float initialScale = 1.0f;
        [SerializeField]
        private float scaleFactor = 0.2f;
        [SerializeField]
        private int scoreValue = 1;

        [Space(10)]

        [SerializeField]
        private AudioClip inflateSound = default;
        [SerializeField]
        private AudioClip popSound = default;

        public Material BalloonMaterial => balloonMaterial;

        public int ClicksToPop => clicksToPop;
        public float InitialScale => initialScale;
        public float ScaleFactor => scaleFactor;
        public int ScoreValue => scoreValue;

        public AudioClip InflateSound => inflateSound;
        public AudioClip PopSound => popSound;
    }
}
