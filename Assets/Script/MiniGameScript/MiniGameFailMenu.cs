using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class MiniGameFailMenu : MonoBehaviour
{
    [Header("Pengaturan Navigasi")]
    public string namaSceneHome = "scnMap";

    [Header("Referensi Teks Skor & Koin")]
    public TextMeshProUGUI teksYourScore;
    public TextMeshProUGUI teksHighScore;
    public TextMeshProUGUI teksKoinDidapat;

    [Header("Target Animasi Utama")]
    public RectTransform popupVisual; 

    [Header("Efek Asap & Serpihan (Looping)")]
    public RectTransform[] daftarAsap;
    public RectTransform[] daftarSerpihan;
    public float durasiLedakan = 0.3f;
    public AnimationCurve kurvaLedakan = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Referensi Tombol")]
    public RectTransform tombolTryAgain;
    public RectTransform tombolMainMenu;

    [Header("Pengaturan Animasi Jatuh (Slam)")]
    public float durasiJatuh = 0.35f;
    public AnimationCurve kurvaJatuh = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Pengaturan Getar Berkala")]
    public float durasiGetar = 0.15f; 
    public float intensitasGetar = 10f; 
    public float jedaAntarGetar = 2f; 

    private CanvasGroup canvasGroup;
    private bool isButtonClicked = false; 
    private Vector2 posisiAsliPopup;
    private Coroutine getarCoroutine;
    private Coroutine loopPartikelCoroutine;

    private Vector2[] posisiAsliAsap;
    private Vector3[] skalaAsliAsap;
    private CanvasGroup[] cgAsap;

    private Vector2[] posisiAsliSerpihan;
    private Vector3[] skalaAsliSerpihan;
    private CanvasGroup[] cgSerpihan;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (popupVisual != null) posisiAsliPopup = popupVisual.anchoredPosition;

        if (daftarAsap != null)
        {
            posisiAsliAsap = new Vector2[daftarAsap.Length];
            skalaAsliAsap = new Vector3[daftarAsap.Length];
            cgAsap = new CanvasGroup[daftarAsap.Length];
            for (int i = 0; i < daftarAsap.Length; i++)
            {
                if (daftarAsap[i] != null)
                {
                    posisiAsliAsap[i] = daftarAsap[i].anchoredPosition;
                    skalaAsliAsap[i] = daftarAsap[i].localScale;
                    cgAsap[i] = TambahkanCanvasGroupAman(daftarAsap[i].gameObject);
                }
            }
        }

        if (daftarSerpihan != null)
        {
            posisiAsliSerpihan = new Vector2[daftarSerpihan.Length];
            skalaAsliSerpihan = new Vector3[daftarSerpihan.Length];
            cgSerpihan = new CanvasGroup[daftarSerpihan.Length];
            for (int i = 0; i < daftarSerpihan.Length; i++)
            {
                if (daftarSerpihan[i] != null)
                {
                    posisiAsliSerpihan[i] = daftarSerpihan[i].anchoredPosition;
                    skalaAsliSerpihan[i] = daftarSerpihan[i].localScale;
                    cgSerpihan[i] = TambahkanCanvasGroupAman(daftarSerpihan[i].gameObject);
                }
            }
        }
    }

    private CanvasGroup TambahkanCanvasGroupAman(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;
        return cg;
    }

    public void TampilkanPanel(int skorAkhir, int koinAkhir)
    {
        gameObject.SetActive(true);

        int highScore = PlayerPrefs.GetInt("HighScoreMiniGame", 0);
        if (skorAkhir > highScore)
        {
            highScore = skorAkhir;
            PlayerPrefs.SetInt("HighScoreMiniGame", highScore);
            PlayerPrefs.Save();
        }

        if (teksYourScore != null) teksYourScore.text = skorAkhir.ToString();
        if (teksHighScore != null) teksHighScore.text = highScore.ToString();
        if (teksKoinDidapat != null) teksKoinDidapat.text = koinAkhir.ToString();

        if (popupVisual != null && canvasGroup != null)
        {
            isButtonClicked = false; 
            StartCoroutine(SekuensAnimasiKalah());
        }
    }

    private IEnumerator SekuensAnimasiKalah()
    {
        canvasGroup.alpha = 0f;
        popupVisual.anchoredPosition = posisiAsliPopup + new Vector2(0, 1000f);
        popupVisual.localScale = Vector3.one; 
        
        SembunyikanEfekTambahan();
        StartCoroutine(FadeInBackground());

        float waktu = 0f;
        while (waktu < durasiJatuh)
        {
            waktu += Time.unscaledDeltaTime; 
            float persentase = waktu / durasiJatuh;
            popupVisual.anchoredPosition = Vector2.LerpUnclamped(posisiAsliPopup + new Vector2(0, 1000f), posisiAsliPopup, kurvaJatuh.Evaluate(persentase));
            yield return null; 
        }
        popupVisual.anchoredPosition = posisiAsliPopup;

        if (AudioManager.Instance != null && AudioManager.Instance.sfxKalah != null)
            AudioManager.Instance.MainkanSFX(AudioManager.Instance.sfxKalah);

        if (getarCoroutine != null) StopCoroutine(getarCoroutine);
        getarCoroutine = StartCoroutine(LoopGetaranBerkala());

        if (loopPartikelCoroutine != null) StopCoroutine(loopPartikelCoroutine);
        loopPartikelCoroutine = StartCoroutine(LoopAnimasiPartikel());
    }

    private void SembunyikanEfekTambahan()
    {
        foreach (var asap in daftarAsap) if (asap != null) asap.localScale = Vector3.zero;
        foreach (var serpihan in daftarSerpihan) 
        {
            if (serpihan != null)
            {
                serpihan.anchoredPosition = posisiAsliPopup + new Vector2(0, 150f); 
                serpihan.localScale = Vector3.zero;
            }
        }
    }

    private IEnumerator LoopAnimasiPartikel()
    {
        while (!isButtonClicked)
        {
            float waktu = 0f;
            float[] arahRotasi = new float[daftarSerpihan.Length];
            for (int i = 0; i < arahRotasi.Length; i++) arahRotasi[i] = Random.Range(-400f, 400f);

            SetAlphaSemuaEfek(1f);

            while (waktu < durasiLedakan && !isButtonClicked)
            {
                waktu += Time.unscaledDeltaTime;
                float kurva = kurvaLedakan.Evaluate(waktu / durasiLedakan);
                for (int i = 0; i < daftarAsap.Length; i++) if (daftarAsap[i] != null) daftarAsap[i].localScale = Vector3.LerpUnclamped(Vector3.zero, skalaAsliAsap[i], kurva);
                for (int i = 0; i < daftarSerpihan.Length; i++)
                {
                    if (daftarSerpihan[i] != null)
                    {
                        Vector2 pusatLedakan = posisiAsliPopup + new Vector2(0, 150f); 
                        daftarSerpihan[i].anchoredPosition = Vector2.LerpUnclamped(pusatLedakan, posisiAsliSerpihan[i], kurva);
                        daftarSerpihan[i].localScale = Vector3.LerpUnclamped(Vector3.zero, skalaAsliSerpihan[i], kurva);
                        daftarSerpihan[i].localRotation = Quaternion.Euler(0, 0, arahRotasi[i] * kurva);
                    }
                }
                yield return null;
            }

            waktu = 0f;
            float durasiJatuhDanMemudar = 1.3f;
            while (waktu < durasiJatuhDanMemudar && !isButtonClicked)
            {
                waktu += Time.unscaledDeltaTime;
                float progres = waktu / durasiJatuhDanMemudar;
                for (int i = 0; i < daftarAsap.Length; i++)
                {
                    if (daftarAsap[i] != null)
                    {
                        daftarAsap[i].localScale = skalaAsliAsap[i] * (1f + (progres * 0.3f));
                        if (cgAsap[i] != null) cgAsap[i].alpha = 1f - progres;
                    }
                }
                for (int i = 0; i < daftarSerpihan.Length; i++)
                {
                    if (daftarSerpihan[i] != null)
                    {
                        float gravitasi = Mathf.Pow(progres, 2) * -350f; 
                        daftarSerpihan[i].anchoredPosition = posisiAsliSerpihan[i] + new Vector2(0, gravitasi);
                        daftarSerpihan[i].Rotate(0, 0, (arahRotasi[i] * 0.5f) * Time.unscaledDeltaTime);
                        if (cgSerpihan[i] != null) cgSerpihan[i].alpha = Mathf.Clamp01(1f - (progres * 1.5f));
                    }
                }
                yield return null;
            }
            SembunyikanEfekTambahan();
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    private void SetAlphaSemuaEfek(float targetAlpha)
    {
        if (cgAsap != null) foreach (var cg in cgAsap) if (cg != null) cg.alpha = targetAlpha;
        if (cgSerpihan != null) foreach (var cg in cgSerpihan) if (cg != null) cg.alpha = targetAlpha;
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
                popupVisual.anchoredPosition = posisiAsliPopup + new Vector2(geserX, geserY);
                yield return null;
            }
            popupVisual.anchoredPosition = posisiAsliPopup;
            yield return new WaitForSecondsRealtime(jedaAntarGetar);
        }
    }

    public void UlangiLevel()
    {
        if (isButtonClicked) return;
        isButtonClicked = true;
        HentikanSemuaLoop();
        StartCoroutine(AnimasiGoyangDanEksekusi(tombolTryAgain, true));
    }

    public void KeMainMenu()
    {
        if (isButtonClicked) return;
        isButtonClicked = true;
        HentikanSemuaLoop();
        StartCoroutine(AnimasiGoyangDanEksekusi(tombolMainMenu, false));
    }

    private void HentikanSemuaLoop()
    {
        if (getarCoroutine != null) StopCoroutine(getarCoroutine); 
        if (loopPartikelCoroutine != null) StopCoroutine(loopPartikelCoroutine);
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
        
        // Kembalikan waktu ke normal
        Time.timeScale = 1f; 
        
        // Tentukan target scene (restart level saat ini, atau ke main menu)
        string targetScene = isUlangi ? SceneManager.GetActiveScene().name : namaSceneHome;

        // Jika sistem transisi ada, gunakan itu. Jika tidak, load langsung!
        if (TransisiScene.Instance != null)
        {
            TransisiScene.Instance.PindahScene(targetScene);
        }
        else
        {
            SceneManager.LoadScene(targetScene);
        }
    }
}