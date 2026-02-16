using SpawnSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "newCoinData", menuName = "BalloonPopper/Spawnables/CoinData", order = 1)]
public class CoinDataSO : SpawnableDataSO
{
    [Header("Model Data")]
    [SerializeField]
    private Sprite sprite = null;

    [Space(10)]

    [Header("Gameplay Data")]
    [SerializeField]
    private int clicksToDestroy = 1;
    [SerializeField]
    private int scoreValue = 1;

    [Space(10)]

    [Header("Audio Data")]
    [SerializeField]
    private AudioClip clickSound = null;
    [SerializeField]
    private AudioClip collectSound = null;


    public Sprite Sprite => sprite;

    public int ClicksToDestroy => clicksToDestroy;
    public int ScoreValue => scoreValue;

    public AudioClip ClickSound => clickSound;
    public AudioClip CollectSound => collectSound;

}
