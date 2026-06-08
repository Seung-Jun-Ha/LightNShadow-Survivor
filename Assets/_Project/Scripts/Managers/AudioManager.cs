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
        [SerializeField] private AudioClip buttonClickSFX;
        [SerializeField] private AudioClip monsterDeathR1SFX;
        [SerializeField] private AudioClip monsterDeathR2SFX;
        [SerializeField] private AudioClip levelUpSFX;
        [SerializeField] private AudioClip roundClearSFX;
        [SerializeField] private AudioClip playerHitSFX;
        [SerializeField] private AudioClip playerDeathSFX;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
            
            if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();
            ConfigureAudioSource(bgmSource, true);

            if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
            ConfigureAudioSource(sfxSource, false);

            AssignDefaultClipsIfMissing();
            EnsureAudioListener();
        }

        private void Start()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged += HandleStateChanged;

            if (RoundManager.Instance != null)
                RoundManager.Instance.OnRoundStarted += HandleRoundStarted;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnStateChanged -= HandleStateChanged;

            if (RoundManager.Instance != null)
                RoundManager.Instance.OnRoundStarted -= HandleRoundStarted;
        }

        private void HandleRoundStarted(int round)
        {
            PlayRoundMusic(round);
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
        }

        private void AssignDefaultClipsIfMissing()
        {
#if UNITY_EDITOR
            round1Music = LoadClip("Assets/Halloween Game Music Pack/Halloween Music Pack/Ambient/Ambient_1/Ambient_1_LOOP.mp3", round1Music);
            round2Music = LoadClip("Assets/Halloween Game Music Pack/Halloween Music Pack/Ambient/Ambient_2/Ambient_2_LOOP.mp3", round2Music);
            round3Music = LoadClip("Assets/Halloween Game Music Pack/Halloween Music Pack/Suspense/Suspense_1/Suspense_1_LOOP.mp3", round3Music);

            buttonClickSFX = LoadClip("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-03.wav", buttonClickSFX);
            monsterDeathR1SFX = LoadClip("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-05.wav", monsterDeathR1SFX);
            monsterDeathR2SFX = LoadClip("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-39.wav", monsterDeathR2SFX);
            levelUpSFX = LoadClip("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-12.wav", levelUpSFX);
            roundClearSFX = LoadClip("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-18.wav", roundClearSFX);
            playerHitSFX = LoadClip("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-47.wav", playerHitSFX);
            playerDeathSFX = LoadClip("Assets/Casual Game Sounds U6/CasualGameSounds/DM-CGS-49.wav", playerDeathSFX);
#endif
        }

#if UNITY_EDITOR
        private static AudioClip LoadClip(string path, AudioClip fallback)
        {
            AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            return clip != null ? clip : fallback;
        }
#endif
    }
}
