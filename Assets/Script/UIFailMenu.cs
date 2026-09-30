using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIFailMenu : MonoBehaviour
{
    [Header("Pengaturan Navigasi")]
    public string namaSceneHome = "scnMap";

    [Header("Target Animasi Utama")]
    public RectTransform popupVisual; 

    [Header("Referensi Tombol (Biar bisa goyang)")]
    public RectTransform tombolTryAgain;
    public RectTransform tombolMainMenu;

    [Header("Pengaturan Animasi Jatuh (Slam)")]
    public float durasiJatuh = 0.35f;
    public AnimationCurve kurvaJatuh = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Pengaturan Getar Berkala")]
    public float durasiGetar = 0.15f; 
    public float intensitasGetar = 10f; 
    public float jedaAntarGetar = 2f; 

    [Header("VFX Hantaman (Baru)")]
    public UIDustImpact efekDebu; 

    private CanvasGroup canvasGroup;
    private bool isButtonClicked = false; 
    private Vector2 posisiAsli;
    private Coroutine getarCoroutine;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (popupVisual != null)
        {
            posisiAsli = popupVisual.anchoredPosition;
        }
    }

    private void OnEnable()
    {
        if (popupVisual != null && canvasGroup != null)
        {
            isButtonClicked = false; 
            StartCoroutine(SekuensAnimasiKalah());
        }
    }

    private IEnumerator SekuensAnimasiKalah()
    {
        // 1. Setup Awal: Transparan dan ditarik jauh ke atas layar
        canvasGroup.alpha = 0f;
        popupVisual.anchoredPosition = posisiAsli + new Vector2(0, 1000f);
        popupVisual.localScale = Vector3.one; 

        StartCoroutine(FadeInBackground());

        // 2. Animasi Jatuh dulu sampai mentok dan berhenti
        float waktu = 0f;
        while (waktu < durasiJatuh)
        {
            waktu += Time.unscaledDeltaTime; 
            float persentase = waktu / durasiJatuh;
            
            popupVisual.anchoredPosition = Vector2.LerpUnclamped(
                posisiAsli + new Vector2(0, 1000f), 
                posisiAsli, 
                kurvaJatuh.Evaluate(persentase)
            );
            
            yield return null; 
        }
        
        // Kunci di tengah supaya pas!
        popupVisual.anchoredPosition = posisiAsli;

        // 3. TRIGGER DEBU DI SINI (Pas banting ke tengah)
        if (efekDebu != null) 
        {
            efekDebu.LedakkanDebu();
        }

        // 4. Mulai loop getaran berkala SETELAH jatuh selesai
        if (getarCoroutine != null) StopCoroutine(getarCoroutine);
        getarCoroutine = StartCoroutine(LoopGetaranBerkala());
    }

    private IEnumerator FadeInBackground()
    {
        float waktu = 0f;
        float durasi = 0.2f;
        while (waktu < durasi)
        {
            waktu += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, waktu / durasi);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator LoopGetaranBerkala()
    {
        while (!isButtonClicked)
        {
            float waktu = 0f;
            while (waktu < durasiGetar)
            {
                waktu += Time.unscaledDeltaTime;
                
                float sisaKekuatan = 1f - (waktu / durasiGetar); 
                float geserX = Random.Range(-1f, 1f) * intensitasGetar * sisaKekuatan;
                float geserY = Random.Range(-1f, 1f) * intensitasGetar * sisaKekuatan;

                popupVisual.anchoredPosition = posisiAsli + new Vector2(geserX, geserY);
                yield return null;
            }

            popupVisual.anchoredPosition = posisiAsli;
            yield return new WaitForSecondsRealtime(jedaAntarGetar);
        }
    }

    public void UlangiLevel()
    {
        if (isButtonClicked) return;
        isButtonClicked = true;
        
        if (getarCoroutine != null) StopCoroutine(getarCoroutine); 
        
        StartCoroutine(AnimasiGoyangDanEksekusi(tombolTryAgain, true));
    }

    public void KeMainMenu()
    {
        if (isButtonClicked) return;
        isButtonClicked = true;
        
        if (getarCoroutine != null) StopCoroutine(getarCoroutine); 
        
        StartCoroutine(AnimasiGoyangDanEksekusi(tombolMainMenu, false));
    }

    private IEnumerator AnimasiGoyangDanEksekusi(RectTransform targetTombol, bool isUlangi)
    {
        if (targetTombol != null)
        {
            float waktu = 0f;
            float durasiGoyang = 0.3f;
            Quaternion rotasiAwal = targetTombol.localRotation;

            while (waktu < durasiGoyang)
            {
                waktu += Time.unscaledDeltaTime;
                float zRot = Mathf.Sin(waktu * 50f) * 10f; 
                targetTombol.localRotation = Quaternion.Euler(0, 0, zRot);
                yield return null;
            }
            targetTombol.localRotation = rotasiAwal; 
        }

        Time.timeScale = 1f; 
        if (isUlangi)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            SceneManager.LoadScene(namaSceneHome);
        }
    }
}