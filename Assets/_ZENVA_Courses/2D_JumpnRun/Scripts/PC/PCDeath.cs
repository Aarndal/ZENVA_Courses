using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JumpnRun
{
    public class PCDeath : MonoBehaviour, IKillable
    {
        [SerializeField]
        private SpriteRenderer playerSprite = null;

        private bool _isDead = false;


        private void OnEnable() => HasBeenKilled += OnHasBeenKilled;

        private void Start()
        {
            _isDead = false;
            playerSprite.gameObject.SetActive(true);
        }

        public event Action HasBeenKilled;


        private void OnDisable() => HasBeenKilled -= OnHasBeenKilled;

        public bool TryKill()
        {
            HasBeenKilled?.Invoke();
            playerSprite.gameObject.SetActive(false);
            return true;
        }

        private void OnHasBeenKilled()
        {
            if (_isDead) return;
            _isDead = true;
            StartCoroutine(ReturnToScene());
        }

        private IEnumerator ReturnToScene()
        {
            yield return new WaitForFixedUpdate();
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
