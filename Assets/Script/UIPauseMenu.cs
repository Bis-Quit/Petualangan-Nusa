using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIPauseMenu : MonoBehaviour
{
    [Header("Referensi UI")]
    [Tooltip("Masukkan panel utama Pause Menu (yang ada background gelapnya)")]
    public GameObject pausePanel;

    [Header("Pengaturan Navigasi")]
    [Tooltip("Nama scene untuk tombol Home (misal: mainmenu atau scnMap)")]
    public string namaSceneHome = "mainMenu";

    private void Start()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
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
    
    public void SetMusicVolume(float volume)
    {
        Debug.Log("Volume Musik diubah ke: " + volume);
    }

    public void SetSFXVolume(float volume)
    {
        Debug.Log("Volume SFX diubah ke: " + volume);
    }
}