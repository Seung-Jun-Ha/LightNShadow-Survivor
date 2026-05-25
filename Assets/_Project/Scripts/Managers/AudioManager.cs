using UnityEngine;
using System.Collections.Generic;

namespace LightNShadowSurvivor
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("BGM Settings")]
        [SerializeField] private AudioSource bgmSource;
        [SerializeField] private AudioClip round1Music;
        [SerializeField] private AudioClip round2Music;
        [SerializeField] private AudioClip round3Music;
        [SerializeField] private AudioClip endingMusic;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
            
            if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
        }

        private void Start()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged += HandleStateChanged;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Round:
                    PlayRoundMusic();
                    break;
                case GameState.Ending:
                    PlayBGM(endingMusic);
                    break;
                case GameState.GameOver:
                    bgmSource.Stop();
                    break;
            }
        }

        private void PlayRoundMusic()
        {
            if (RoundManager.Instance == null) return;

            AudioClip clip = round1Music;
            switch (RoundManager.Instance.CurrentRound)
            {
                case 1: clip = round1Music; break;
                case 2: clip = round2Music; break;
                case 3: clip = round3Music; break;
            }

            if (bgmSource.clip != clip)
            {
                PlayBGM(clip);
            }
        }

        private void PlayBGM(AudioClip clip)
        {
            if (clip == null) return;
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }
}
