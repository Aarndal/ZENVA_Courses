using SpawnSystem;
using UnityEngine;

namespace BalloonPopper
{
    [CreateAssetMenu(fileName = "newBalloonData", menuName = "BalloonPopper/Spawnables/BalloonData", order = 0)]
    public class BalloonDataSO : SpawnableDataSO
    {
        [Header("Model Data")]
        [SerializeField]
        private Material material = null;

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
        private AudioClip inflateSound = null;
        [SerializeField]
        private AudioClip popSound = null;


        public Material Material => material;

        public int ClicksToPop => clicksToPop;
        public float InitialScale => initialScale;
        public float ScaleFactor => scaleFactor;
        public int ScoreValue => scoreValue;

        public AudioClip InflateSound => inflateSound;
        public AudioClip PopSound => popSound;
       
    }
}
