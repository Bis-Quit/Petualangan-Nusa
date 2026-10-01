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

[RequireComponent(typeof(CanvasGroup))] 
public class MascotPopupManager : MonoBehaviour
{
    public static MascotPopupManager Instance;

    [Header("UI Elements")]
    public GameObject popupPanel; 
    public RectTransform popupVisual; 
    public TextMeshProUGUI chatText;
    public Image popupMascotImage;

    [Header("Data Skin Maskot (Animasi)")]
    public MascotSkinAnim[] mascotSkins; 
    
    [Header("Pengaturan Animasi Skin")]
    public float animSpeed = 0.15f;

    [Header("Pengaturan Animasi Popup")]
    public float durasiAnimasi = 0.5f;
    public AnimationCurve kurvaBouncy = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Scene Settings")]
    public string gameplayScene = "scnHiddenObject";
    public string koperScene = "scnKoper"; 

    private string prefsSkinKey = "SelectedSkinIndex"; 
    private LevelData pendingLevelData; 
    private Coroutine waveCoroutine;
    
    private bool isWinState = false;

    private CanvasGroup canvasGroup;
    private bool isButtonClicked = false;
    private Vector3 skalaAwalPopup;
    private Vector2 posisiAsliPopup; 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        canvasGroup = GetComponent<CanvasGroup>();

        if (popupVisual != null)
        {
            skalaAwalPopup = popupVisual.localScale;
            posisiAsliPopup = popupVisual.anchoredPosition;
        }
    }

    private void Start()
    {
        if (popupPanel != null) popupPanel.SetActive(false);
    }

    private void SetIdleFrame()
    {
        if (mascotSkins == null || mascotSkins.Length == 0 || popupMascotImage == null) return;
        
        int currentSkinIndex = PlayerPrefs.GetInt(prefsSkinKey, 0);
        if (currentSkinIndex >= 0 && currentSkinIndex < mascotSkins.Length)
        {
            popupMascotImage.sprite = mascotSkins[currentSkinIndex].frameIdle;
        }
    }

    public void ShowMascotPopup(LevelData selectedLevel)
    {
        isWinState = false;
        pendingLevelData = selectedLevel;
        chatText.text = "Halo kawan! Ayo bantu aku mencari barang pusaka di pulau " + pendingLevelData.levelName + "!";
        
        SetIdleFrame(); 
        MulaiAnimasiPopup(); 
    }

    public void ShowWinPopup()
    {
        isWinState = true;
        chatText.text = "Wah hebat kawan! Kamu berhasil mengumpulkan semua pusaka. Ayo kita kemas ke dalam koper!";
        
        SetIdleFrame(); 
        MulaiAnimasiPopup(); 
    }

    private void MulaiAnimasiPopup()
    {
        popupPanel.SetActive(true);
        isButtonClicked = false;
        
        if (AudioManager.Instance != null && AudioManager.Instance.sfxMaskot != null)
        {
            AudioManager.Instance.MainkanSFX(AudioManager.Instance.sfxMaskot);
        }

        StartCoroutine(AnimasiMumbulMaskot());
    }

    private IEnumerator AnimasiMumbulMaskot()
    {
        if (popupVisual == null || canvasGroup == null)
        {
            Debug.LogWarning("Popup Visual atau Canvas Group belum diisi di Inspector!");
            UpdateAndAnimateMascot();
            yield break; 
        }

        canvasGroup.alpha = 0f;

        Vector2 posisiBawah = posisiAsliPopup - new Vector2(0, 800f);
        
        popupVisual.anchoredPosition = posisiBawah;
        popupVisual.localScale = skalaAwalPopup; 

        float waktu = 0f;
        while (waktu < durasiAnimasi)
        {
            waktu += Time.unscaledDeltaTime;
            float persentase = waktu / durasiAnimasi;

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, persentase);
            
            float kurva = kurvaBouncy.Evaluate(persentase);
            popupVisual.anchoredPosition = Vector2.LerpUnclamped(posisiBawah, posisiAsliPopup, kurva);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        popupVisual.anchoredPosition = posisiAsliPopup;

        UpdateAndAnimateMascot(); 
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
        yield return new WaitForSecondsRealtime(0.2f);
        popupMascotImage.sprite = skin.frameWave1;
        yield return new WaitForSecondsRealtime(0.08f);
        popupMascotImage.sprite = skin.frameWave2;
        yield return new WaitForSecondsRealtime(0.12f);
        popupMascotImage.sprite = skin.frameWave1;
        yield return new WaitForSecondsRealtime(0.08f);
        popupMascotImage.sprite = skin.frameWave2;
        yield return new WaitForSecondsRealtime(0.12f);
        popupMascotImage.sprite = skin.frameWave1;
        yield return new WaitForSecondsRealtime(0.08f);
        popupMascotImage.sprite = skin.frameWave2;
        yield return new WaitForSecondsRealtime(0.12f);
        popupMascotImage.sprite = skin.frameWave1;
        yield return new WaitForSecondsRealtime(0.08f);
        popupMascotImage.sprite = skin.frameIdle;
    }

    public void OnClickPlay()
    {
        if (isButtonClicked) return; 
        isButtonClicked = true;
        
        if (isWinState)
        {
            TransisiScene.Instance.PindahScene(koperScene);
        }
        else if (pendingLevelData != null)
        {
            HiddenObjectManager.ActiveLevelData = pendingLevelData;
            TransisiScene.Instance.PindahScene(gameplayScene);
        }
    }

    public void OnClickClose()
    {
        if (waveCoroutine != null) StopCoroutine(waveCoroutine);
        popupPanel.SetActive(false);
    }
}