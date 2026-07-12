using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;   // для фоновой музыки (loop)
    [SerializeField] private AudioSource sfxSource;     // для звуковых эффектов (one-shot)

    // Фоновое
    [Header("Music Clips")]
    [SerializeField] private AudioClip musicMainMenu;
    [SerializeField] private AudioClip musicBattle;
    [SerializeField] private AudioClip musicAnomaly;

    // Эффекты
    [Header("SFX Clips")]
    [SerializeField] private AudioClip sfxStartFight;
    [SerializeField] private AudioClip sfxLevelUp;
    [SerializeField] private AudioClip sfxHarp;
    [SerializeField] private AudioClip sfxPortal;
    [SerializeField] private AudioClip sfxAttack1;
    [SerializeField] private AudioClip sfxAttack2;
    [SerializeField] private AudioClip sfxDeath;

    // Громкость
    [Header("Volume")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 0.8f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); 
            return;
        }

        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;              // музыка зациклена
            musicSource.volume = musicVolume;
        }
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;               // SFX не зациклены
            sfxSource.volume = sfxVolume;
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (musicSource != null)
            musicSource.volume = musicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        if (sfxSource != null)
            sfxSource.volume = sfxVolume;
    }

    public void PlayMusicMainMenu() => PlayMusic(musicMainMenu);
    public void PlayMusicBattle() => PlayMusic(musicBattle);
    public void PlayMusicAnomaly() => PlayMusic(musicAnomaly);

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("⚠️ Попытка сыграть пустой музыкальный клип!");
            return;
        }

        // Если та же музыка уже играет — не перезапускаем
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
        musicSource.clip = null;
    }


    public void PlayStartFight() => PlaySFX(sfxStartFight);
    public void PlayLevelUp() => PlaySFX(sfxLevelUp);
    public void PlayHarp() => PlaySFX(sfxHarp);
    public void PlayPortal() => PlaySFX(sfxPortal);
    public void PlayAttack1() => PlaySFX(sfxAttack1);
    public void PlayAttack2() => PlaySFX(sfxAttack2);
    public void PlayDeath() => PlaySFX(sfxDeath);

    // Случайная атака
    public void PlayRandomAttack()
    {
        if (Random.Range(0, 2) == 0)
            PlayAttack1();
        else
            PlayAttack2();
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("⚠️ Попытка сыграть пустой SFX клип!");
            return;
        }
        sfxSource.PlayOneShot(clip); // PlayOneShot — звук поверх других, не перебивает
    }
}