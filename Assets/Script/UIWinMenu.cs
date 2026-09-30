using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIWinMenu : MonoBehaviour
{
    [Header("Pengaturan Navigasi")]
    public string namaSceneNextLevel = "scnMap"; 
    public string namaSceneRetry = "scnHiddenObject"; 

    [Header("Target Animasi Utama")]
    public RectTransform popupVisual; 

    [Header("Referensi UI")]
    public TextMeshProUGUI textKoin;
    public GameObject[] bintangMenyala; 
    
    [Header("Efek Visual Latar (Baru)")]
    [Tooltip("Masukkan objek Pancaran Cahaya (BurstEffect) ke sini")]
    public GameObject efekPancaranCahaya; 
    [Tooltip("Masukkan ke-4 objek pecahan SparkEffect (Bintang) ke sini")]
    public GameObject[] bintangLatarBelakang; 

    [Header("Pengaturan Animasi")]
    public float durasiAnimasi = 0.4f;
    public AnimationCurve kurvaBouncy = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private CanvasGroup canvasGroup;
    private bool isButtonClicked = false;

    private Vector3 skalaAwalPopup;
    private Vector3[] skalaAwalBintang;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (popupVisual != null)
        {
            skalaAwalPopup = popupVisual.localScale;
        }

        skalaAwalBintang = new Vector3[bintangMenyala.Length];
        for (int i = 0; i < bintangMenyala.Length; i++)
        {
            if (bintangMenyala[i] != null)
            {
                skalaAwalBintang[i] = bintangMenyala[i].GetComponent<RectTransform>().localScale;
            }
        }
    }

    public void TampilkanMenang(int jumlahBintang)
    {
        int totalKoin = 0;
        if (CoinManager.Instance != null)
        {
            totalKoin = CoinManager.Instance.koinLevelTerakhir;
        }

        if (textKoin != null) textKoin.text = "0 Koin";

        for (int i = 0; i < bintangMenyala.Length; i++)
        {
            if (bintangMenyala[i] != null) bintangMenyala[i].SetActive(false);
        }
        
        // Matikan burst dan bintang latar sebelum popup muncul
        if (efekPancaranCahaya != null) efekPancaranCahaya.SetActive(false);
        if (bintangLatarBelakang != null)
        {
            for (int i = 0; i < bintangLatarBelakang.Length; i++)
            {
                if (bintangLatarBelakang[i] != null) bintangLatarBelakang[i].SetActive(false);
            }
        }

        gameObject.SetActive(true); 
        isButtonClicked = false;
        
        StartCoroutine(SekuensAnimasiMenang(jumlahBintang, totalKoin));
    }

    private IEnumerator SekuensAnimasiMenang(int jumlahBintang, int targetKoin)
    {
        canvasGroup.alpha = 0f;
        popupVisual.localScale = Vector3.zero;
        
        float waktu = 0f;
        while (waktu < durasiAnimasi)
        {
            waktu += Time.unscaledDeltaTime; 
            float persentase = waktu / durasiAnimasi;
            
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, persentase);
            popupVisual.localScale = skalaAwalPopup * kurvaBouncy.Evaluate(persentase);
            
            yield return null; 
        }

        canvasGroup.alpha = 1f;
        popupVisual.localScale = skalaAwalPopup;

        // FUNGSI BARU: Munculkan Pancaran Cahaya dengan efek nge-per
        if (efekPancaranCahaya != null)
        {
            efekPancaranCahaya.SetActive(true);
            StartCoroutine(AnimasiPopSederhana(efekPancaranCahaya.GetComponent<RectTransform>()));
        }

        yield return new WaitForSecondsRealtime(0.2f);
        
        StartCoroutine(MunculkanBintangLatarBergiliran());

        float durasiBintang = 0.3f; 
        for (int i = 0; i < jumlahBintang; i++)
        {
            if (i < bintangMenyala.Length && bintangMenyala[i] != null)
            {
                GameObject bintang = bintangMenyala[i];
                bintang.SetActive(true);
                
                RectTransform rectBintang = bintang.GetComponent<RectTransform>();
                
                if (rectBintang != null)
                {
                    rectBintang.localScale = Vector3.zero;
                    float waktuBintang = 0f;
                    
                    while (waktuBintang < durasiBintang)
                    {
                        waktuBintang += Time.unscaledDeltaTime;
                        float persenBintang = waktuBintang / durasiBintang;
                        rectBintang.localScale = skalaAwalBintang[i] * kurvaBouncy.Evaluate(persenBintang);
                        yield return null;
                    }
                    rectBintang.localScale = skalaAwalBintang[i];
                }
                
                yield return new WaitForSecondsRealtime(0.15f);
            }
        }

        if (textKoin != null && targetKoin > 0)
        {
            float waktuHitung = 0f;
            float durasiHitung = 0.8f; 
            
            while (waktuHitung < durasiHitung)
            {
                waktuHitung += Time.unscaledDeltaTime;
                float persentaseHitung = waktuHitung / durasiHitung;
                
                int koinSaatIni = Mathf.RoundToInt(Mathf.Lerp(0, targetKoin, persentaseHitung));
                textKoin.text = koinSaatIni.ToString() + " Koin";
                
                yield return null;
            }

            textKoin.text = targetKoin.ToString() + " Koin";
            StartCoroutine(AnimasiPopTeks(textKoin.GetComponent<RectTransform>()));
        }
    }
    
    // FUNGSI BARU: Animasi membesar membal untuk Pancaran Cahaya
    private IEnumerator AnimasiPopSederhana(RectTransform rect)
    {
        if (rect == null) yield break;
        
        Vector3 skalaAsli = rect.localScale;
        rect.localScale = Vector3.zero;

        float waktu = 0f;
        float durasi = 0.35f;

        while (waktu < durasi)
        {
            waktu += Time.unscaledDeltaTime;
            // Pinjam kurvaBouncy yang udah diset di Inspector biar gerakannya seragam
            rect.localScale = skalaAsli * kurvaBouncy.Evaluate(waktu / durasi);
            yield return null;
        }

        rect.localScale = skalaAsli;
    }

    private IEnumerator MunculkanBintangLatarBergiliran()
    {
        if (bintangLatarBelakang == null) yield break;

        for (int i = 0; i < bintangLatarBelakang.Length; i++)
        {
            if (bintangLatarBelakang[i] != null)
            {
                bintangLatarBelakang[i].SetActive(true);
            }
            yield return new WaitForSecondsRealtime(0.15f); 
        }
    }

    private IEnumerator AnimasiPopTeks(RectTransform rect)
    {
        if (rect == null) yield break;
        
        Vector3 skalaAwalTeks = Vector3.one; 
        float waktu = 0f;
        float durasiPop = 0.15f;
        
        while (waktu < durasiPop)
        {
            waktu += Time.unscaledDeltaTime;
            rect.localScale = Vector3.Lerp(skalaAwalTeks, skalaAwalTeks * 1.3f, waktu / durasiPop);
            yield return null;
        }
        
        waktu = 0f;
        while (waktu < durasiPop)
        {
            waktu += Time.unscaledDeltaTime;
            rect.localScale = Vector3.Lerp(skalaAwalTeks * 1.3f, skalaAwalTeks, waktu / durasiPop);
            yield return null;
        }
        
        rect.localScale = skalaAwalTeks;
    }

    public void UlangiLevel()
    {
        if (isButtonClicked) return; 
        isButtonClicked = true;
        Time.timeScale = 1f; 
        SceneManager.LoadScene(namaSceneRetry); 
    }

    public void LanjutLevel()
    {
        if (isButtonClicked) return;
        isButtonClicked = true;

        if (HiddenObjectManager.ActiveLevelData != null)
        {
            PlayerPrefs.SetString("LevelBaruSelesai", HiddenObjectManager.ActiveLevelData.levelName);
            PlayerPrefs.Save();
        }

        Time.timeScale = 1f; 
        SceneManager.LoadScene(namaSceneNextLevel);
    }
}