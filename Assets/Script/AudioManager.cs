using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Komponen Pemutar Suara")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("BGM Global")]
    public AudioClip bgmPetaUtama; 

    [Header("Daftar Suara (SFX) Global")]
    public AudioClip sfxKlikTombol;
    public AudioClip sfxKertasJurnal;
    public AudioClip sfxDapatBarang;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GantiBGM(AudioClip bgmBaru)
    {
        if (bgmSource == null || bgmBaru == null || bgmSource.clip == bgmBaru) return;
        bgmSource.clip = bgmBaru;
        bgmSource.Play();
    }

    public void MainkanSFX(AudioClip klipSuara)
    {
        if (klipSuara != null && sfxSource != null) sfxSource.PlayOneShot(klipSuara);
    }
}