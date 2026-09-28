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
    public AudioClip sfxPilihPulau; // BARU
    public AudioClip sfxMaskot;     // BARU

    [Header("Daftar Suara (SFX) Koper & Puzzle")]
    public AudioClip sfxAmbilBarang; 
    public AudioClip sfxSnapKoper;   
    public AudioClip sfxError;       
    public AudioClip sfxMenang;      
    public AudioClip sfxKalah;       // BARU
    public AudioClip sfxPutarBarang; 
    public AudioClip sfxTutupKoper;  // BARU

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

    void Start()
    {
        // Otomatis putar BGM utama saat game pertama kali dibuka
        if (bgmPetaUtama != null)
        {
            GantiBGM(bgmPetaUtama);
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

    public void HentikanBGM()
    {
        if (bgmSource != null) bgmSource.Stop();
    }
}