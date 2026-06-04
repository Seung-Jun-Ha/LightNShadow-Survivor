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

        [Header("SFX Settings")]
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioClip buttonClickSFX;      // 03
        [SerializeField] private AudioClip monsterDeathR1SFX;   // 05
        [SerializeField] private AudioClip monsterDeathR2SFX;   // 39
        [SerializeField] private AudioClip levelUpSFX;          // 12
        [SerializeField] private AudioClip roundClearSFX;       // 18
        [SerializeField] private AudioClip playerHitSFX;        // 47
        [SerializeField] private AudioClip playerDeathSFX;      // 49

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
            
            if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }
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

        public void PlayBGM(AudioClip clip)
        {
            if (clip == null) return;
            bgmSource.clip = clip;
            bgmSource.Play();
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null || sfxSource == null) return;
            sfxSource.PlayOneShot(clip);
        }

        public void PlayButtonClickSFX() => PlaySFX(buttonClickSFX);
        public void PlayMonsterDeathSFX()
        {
            int round = RoundManager.Instance != null ? RoundManager.Instance.CurrentRound : 1;
            PlaySFX(round == 1 ? monsterDeathR1SFX : monsterDeathR2SFX);
        }
        public void PlayLevelUpSFX() => PlaySFX(levelUpSFX);
        public void PlayRoundClearSFX() => PlaySFX(roundClearSFX);
        public void PlayPlayerHitSFX() => PlaySFX(playerHitSFX);
        public void PlayPlayerDeathSFX() => PlaySFX(playerDeathSFX);
    }
}
