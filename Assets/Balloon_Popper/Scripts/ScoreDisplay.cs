using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField]
    private ScoreManager scoreManager = default;
    [SerializeField]
    private TMP_Text scoreText = default;


    private void Awake()
    {
        // Search for ScoreManager component in game object and parent objects if not assigned
        scoreManager = scoreManager == null ? GetComponentInParent<ScoreManager>(true) : scoreManager;

        if (scoreManager == null)
        {
            Debug.LogErrorFormat("ScoreManager reference is missing and could not be found in parent objects: {0} | ID: {1}",
                this.gameObject.name,
                this.gameObject.GetEntityId());

            return;
        }

        // Search for TextMeshPro component in game object and child objects if not assigned
        scoreText = scoreText == null ? GetComponentInChildren<TMP_Text>(true) : scoreText;

        if (scoreText == null)
        {

            Debug.LogErrorFormat("TextMeshPro reference for score text is missing and could not be found in child objects: {0} | ID: {1}",
                this.gameObject.name,
                this.gameObject.GetEntityId());
            return;
        }
    }

    private void OnEnable()
    {
        if (scoreManager != null)
        {
            scoreManager.ScoreUpdated += OnScoreUpdated;
        }
    }

    //private void Start()
    //{
    //    // Initialize score display
    //    if (scoreManager == null || scoreText == null)
    //        return;

    //    scoreText.text = $"Score: {scoreManager.CurrentScore}";
    //}

    private void OnDisable()
    {
        if (scoreManager != null)
        {
            scoreManager.ScoreUpdated -= OnScoreUpdated;
        }
    }

    private void OnScoreUpdated(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {newScore}";
        }
    }
}
