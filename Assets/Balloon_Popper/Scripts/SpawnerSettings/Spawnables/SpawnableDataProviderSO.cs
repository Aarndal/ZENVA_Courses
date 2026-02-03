using UnityEditor;
using UnityEngine;

namespace BalloonPopper
{
    [CreateAssetMenu(fileName = "newBalloonData", menuName = "BalloonPopper/Data/BalloonData", order = 0)]
    public class SpawnableDataProviderSO : ScriptableObject, IDataProvider<ISpawnable>
    {
        [Header("Model Data")]
        [SerializeField]
        private Material material = null;
        [SerializeField]
        private GameObject prefab = null;

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

        public string Name => this.name;

        public Material Material => material;
        public GameObject Prefab => prefab;
        public IObjectPool<ISpawnable, IDataProvider<ISpawnable>> Pool { get; set; }

        public int ClicksToPop => clicksToPop;
        public float InitialScale => initialScale;
        public float ScaleFactor => scaleFactor;
        public int ScoreValue => scoreValue;

        public AudioClip InflateSound => inflateSound;
        public AudioClip PopSound => popSound;


        private void Awake()
        {
#if UNITY_EDITOR
            if (prefab == null)
            {
                Debug.LogErrorFormat("Prefab reference is not assigned in SpawnableDataProvider: {0} | ID: {1}", 
                    this.name, 
                    this.GetEntityId());
            }
            else if (PrefabUtility.GetPrefabAssetType(prefab) == PrefabAssetType.NotAPrefab)
            {
                Debug.LogErrorFormat("Assigned GameObject is not a Prefab asset in SpawnableDataProvider: {0} | ID: {1}",
                    this.name,
                    this.GetEntityId());
            }
#endif
        }
    }
}
