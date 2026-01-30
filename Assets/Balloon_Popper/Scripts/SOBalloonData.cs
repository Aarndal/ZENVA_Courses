using UnityEngine;

namespace BalloonPopper
{
    [CreateAssetMenu(fileName = "BalloonData", menuName = "BalloonPopper/BalloonData")]
    public class SOBalloonData : ScriptableObject
    {
        [Header("Model Data")]
        [SerializeField]
        private Material material = default;
        [SerializeField]
        private GameObject prefab = default;

        [Space(10)]

        [Header("Gameplay Data")]
        [SerializeField]
        private int clicksToPop = 5;
        [SerializeField]
        private float initialScale = 1.0f;
        [SerializeField]
        private float scaleFactor = 0.2f;
        [SerializeField]
        private int scoreValue = 1;

        [Space(10)]

        [Header("Audio Data")]
        [SerializeField]
        private AudioClip inflateSound = default;
        [SerializeField]
        private AudioClip popSound = default;


        public Material Material => material;
        public GameObject Prefab => prefab;

        public int ClicksToPop => clicksToPop;
        public float InitialScale => initialScale;
        public float ScaleFactor => scaleFactor;
        public int ScoreValue => scoreValue;

        public AudioClip InflateSound => inflateSound;
        public AudioClip PopSound => popSound;
    }
}
