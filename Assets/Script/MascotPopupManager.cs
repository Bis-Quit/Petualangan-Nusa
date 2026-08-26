using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public struct MascotSkinAnim
{
    public Sprite frameIdle;  
    public Sprite frameWave1; 
    public Sprite frameWave2; 
}

public class MascotPopupManager : MonoBehaviour
{
    public static MascotPopupManager Instance;

    [Header("UI Elements")]
    public GameObject popupPanel;
    public TextMeshProUGUI chatText;
    public Image popupMascotImage;

    [Header("Data Skin Maskot (Animasi)")]
    public MascotSkinAnim[] mascotSkins; 
    
    [Header("Pengaturan Animasi")]
    public float animSpeed = 0.15f;

    [Header("Scene Settings")]
    public string gameplayScene = "scnHiddenObject";

    private string prefsSkinKey = "SelectedSkinIndex"; 
    private LevelData pendingLevelData; // Menampung SO titipan
    private Coroutine waveCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (popupPanel != null) popupPanel.SetActive(false);
    }

    // --- UBAH PARAMETER JADI LEVEL DATA ---
    public void ShowMascotPopup(LevelData selectedLevel)
    {
        pendingLevelData = selectedLevel;
        
        // Ambil nama pulau otomatis dari SO
        chatText.text = "Halo kawan! Ayo bantu aku mencari barang pusaka di pulau " + pendingLevelData.levelName + "!";
        
        UpdateAndAnimateMascot();

        popupPanel.SetActive(true);
    }

    private void UpdateAndAnimateMascot()
    {
        if (mascotSkins == null || mascotSkins.Length == 0 || popupMascotImage == null) return;

        int currentSkinIndex = PlayerPrefs.GetInt(prefsSkinKey, 0);

        if (currentSkinIndex >= 0 && currentSkinIndex < mascotSkins.Length)
        {
            if (waveCoroutine != null) StopCoroutine(waveCoroutine);
            
            waveCoroutine = StartCoroutine(WaveRoutine(mascotSkins[currentSkinIndex]));
        }
    }

    private IEnumerator WaveRoutine(MascotSkinAnim skin)
    {
        popupMascotImage.sprite = skin.frameIdle;
        yield return new WaitForSeconds(0.2f);
        popupMascotImage.sprite = skin.frameWave1;
        yield return new WaitForSeconds(0.08f);
        popupMascotImage.sprite = skin.frameWave2;
        yield return new WaitForSeconds(0.12f);
        popupMascotImage.sprite = skin.frameWave1;
        yield return new WaitForSeconds(0.08f);
        popupMascotImage.sprite = skin.frameWave2;
        yield return new WaitForSeconds(0.12f);
        popupMascotImage.sprite = skin.frameWave1;
        yield return new WaitForSeconds(0.08f);
        popupMascotImage.sprite = skin.frameWave2;
        yield return new WaitForSeconds(0.12f);
        popupMascotImage.sprite = skin.frameWave1;
        yield return new WaitForSeconds(0.08f);
        popupMascotImage.sprite = skin.frameIdle;
    }

    // --- EKSEKUSI PINDAH SCENE ---
    public void OnClickPlay()
    {
        if (pendingLevelData != null)
        {
            HiddenObjectManager.ActiveLevelData = pendingLevelData;
            
            SceneManager.LoadSceneAsync(gameplayScene);
        }
    }

    public void OnClickClose()
    {
        if (waveCoroutine != null) StopCoroutine(waveCoroutine);
        popupPanel.SetActive(false);
    }
}