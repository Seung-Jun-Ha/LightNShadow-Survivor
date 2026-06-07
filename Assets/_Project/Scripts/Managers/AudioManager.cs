using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        [SerializeField] private AudioClip skillSelectSFX;      // 31
        [SerializeField] private AudioClip monsterDeathR1SFX;   // 05
        [SerializeField] private AudioClip monsterDeathR2SFX;   // 39
        [SerializeField] private AudioClip levelUpSFX;          // 12
        [SerializeField] private AudioClip roundClearSFX;       // 18
        [SerializeField] private AudioClip playerHitSFX;        // 47
        [SerializeField] private AudioClip playerDeathSFX;      // 49
        private bool subscribedToRoundManager;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
            
            if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();
            ConfigureAudioSource(bgmSource, true);

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }
            ConfigureAudioSource(sfxSource, false);

            AssignDefaultClipsIfMissing();
            EnsureAudioListener();
        }

        private void Start()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged += HandleStateChanged;

            SubscribeToRoundManager();
            Debug.Log($"[AudioManager] Output GameObject: {gameObject.name}. BGM Source: {bgmSource.name}, SFX Source: {sfxSource.name}");
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChanged;

            if (RoundManager.Instance != null)
                RoundManager.Instance.OnRoundStarted -= HandleRoundStarted;
        }

        private void HandleStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.Round:
                    SubscribeToRoundManager();
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

        private void SubscribeToRoundManager()
        {
            if (subscribedToRoundManager || RoundManager.Instance == null) return;

            RoundManager.Instance.OnRoundStarted += HandleRoundStarted;
            subscribedToRoundManager = true;
        }

        private void HandleRoundStarted(int round)
        {
            PlayRoundMusic(round);
        }

        private void PlayRoundMusic()
        {
            if (RoundManager.Instance == null) return;
            PlayRoundMusic(RoundManager.Instance.CurrentRound);
        }

        private void PlayRoundMusic(int round)
        {
            AudioClip clip = round1Music;
            switch (round)
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
            if (clip == null)
            {
                Debug.LogWarning("[AudioManager] Tried to play a null BGM clip.");
                return;
            }

            bgmSource.clip = clip;
            bgmSource.Play();
            Debug.Log($"[AudioManager] Playing BGM '{clip.name}' from '{bgmSource.gameObject.name}'.");
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null || sfxSource == null)
            {
                Debug.LogWarning("[AudioManager] Tried to play a null SFX clip or SFX source is missing.");
                return;
            }

            sfxSource.PlayOneShot(clip);
        }

        public void PlayButtonClickSFX() => PlaySFX(buttonClickSFX);
        public void PlaySkillSelectSFX() => PlaySFX(skillSelectSFX);
        public void PlayMonsterDeathSFX()
        {
            int round = RoundManager.Instance != null ? RoundManager.Instance.CurrentRound : 1;
            PlaySFX(round == 1 ? monsterDeathR1SFX : monsterDeathR2SFX);
        }
        public void PlayLevelUpSFX() => PlaySFX(levelUpSFX);
        public void PlayRoundClearSFX() => PlaySFX(roundClearSFX);
        public void PlayPlayerHitSFX() => PlaySFX(playerHitSFX);
        public void PlayPlayerDeathSFX() => PlaySFX(playerDeathSFX);

        private void AssignDefaultClipsIfMissing()
        {
#if UNITY_EDITOR
            round1Music ??= LoadClip("Assets/OccaSoftware/Fantasy Music Pack/3_Pre-Battle.wav");
            round2Music ??= LoadClip("Assets/OccaSoftware/Fantasy Music Pack/4_Combat_Loop.wav");
            round3Music ??= LoadClip("Assets/OccaSoftware/Fantasy Music Pack/5_Combat_Loop.wav");
            endingMusic ??= LoadClip("Assets/OccaSoftware/Fantasy Music Pack/7_Victory.wav");

            buttonClickSFX ??= LoadClip("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-03.wav");
            skillSelectSFX ??= LoadClip("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-31.wav");
            monsterDeathR1SFX ??= LoadClip("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-05.wav");
            monsterDeathR2SFX ??= LoadClip("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-39.wav");
            levelUpSFX ??= LoadClip("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-12.wav");
            roundClearSFX ??= LoadClip("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-18.wav");
            playerHitSFX ??= LoadClip("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-47.wav");
            playerDeathSFX ??= LoadClip("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-49.wav");
#endif
        }

        private static void ConfigureAudioSource(AudioSource source, bool loop)
        {
            if (source == null) return;

            source.playOnAwake = false;
            source.loop = loop;
            source.mute = false;
            source.volume = 1f;
            source.spatialBlend = 0f;
            source.ignoreListenerPause = true;
        }

        private void EnsureAudioListener()
        {
            if (FindAnyObjectByType<AudioListener>() != null) return;

            gameObject.AddComponent<AudioListener>();
            Debug.LogWarning("[AudioManager] No AudioListener was found, so one was added to the AudioManager GameObject.");
        }

#if UNITY_EDITOR
        private static AudioClip LoadClip(string path)
        {
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            if (clip == null)
            {
                Debug.LogWarning($"[AudioManager] Audio clip not found at path: {path}");
            }

            return clip;
        }
#endif
    }
}
