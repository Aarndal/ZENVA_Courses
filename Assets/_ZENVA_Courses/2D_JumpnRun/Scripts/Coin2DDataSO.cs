using SpawnSystem;
using UnityEngine;

namespace JumpnRun
{
    [CreateAssetMenu(fileName = "newCoin2DData", menuName = "2D Jump'n'Run/Spawnables/Coin2DData", order = 1)]
    public class Coin2DDataSO : SpawnableDataSO
    {
        public const int DEFAULT_SCORE_VALUE = 0;

        [Header("Model Data")]
        [SerializeField]
        private Sprite sprite = null;

        [Space(10)]

        [Header("Gameplay Data")]
        [SerializeField]
        private int scoreValue = 1;

        [Space(10)]

        [Header("Audio Data")]
        [SerializeField]
        private AudioClip collectSound = null;


        public Sprite Sprite => sprite;
        public int ScoreValue => scoreValue;
        public AudioClip CollectSound => collectSound;

    }
}
