using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIPauseMenu : MonoBehaviour
{
    [Header("Referensi UI")]
    [Tooltip("Masukkan panel utama Pause Menu (yang ada background gelapnya)")]
    public GameObject pausePanel;

    [Header("Pengaturan Audio (Slider)")]
    public Slider sliderMusic;
    public Slider sliderSFX;

    [Header("Pengaturan Navigasi")]
    [Tooltip("Nama scene untuk tombol Home (misal: mainmenu atau scnMap)")]
    public string namaSceneHome = "mainMenu";

    private void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // --- BARU: Setel posisi slider sesuai volume yang tersimpan ---
        if (sliderMusic != null)
        {
            sliderMusic.value = PlayerPrefs.GetFloat("VolumeBGM", 100f);
        }
        if (sliderSFX != null)
        {
            sliderSFX.value = PlayerPrefs.GetFloat("VolumeSFX", 100f);
        }
    }

    public void PauseGame()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
        
        Time.timeScale = 0f; 
    }

    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        
        Time.timeScale = 1f; 
    }

    public void GoToHome()
    {
        Time.timeScale = 1f; 
        TransisiScene.Instance.PindahScene(namaSceneHome);
    }
    
    // --- BARU: Fungsi ini sekarang beneran mengubah volume ---
    public void SetMusicVolume(float volume)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.bgmSource != null)
        {
            AudioManager.Instance.bgmSource.volume = volume;
        }
        
        PlayerPrefs.SetFloat("VolumeBGM", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        if (AudioManager.Instance != null && AudioManager.Instance.sfxSource != null)
        {
            AudioManager.Instance.sfxSource.volume = volume;
        }
        
        PlayerPrefs.SetFloat("VolumeSFX", volume);
        PlayerPrefs.Save();
    }
}