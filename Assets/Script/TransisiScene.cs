using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(CanvasGroup))]
public class TransisiScene : MonoBehaviour
{
    public static TransisiScene Instance;

    [Header("Pengelompokan Awan (Timing)")]
    public RectTransform[] gelombangPertama;
    public RectTransform[] gelombangKedua;

    [Header("Efek Kabut / Penambal")]
    [Tooltip("Masukkan CanvasGroup dari Background Overlay ke sini")]
    public CanvasGroup overlayKabut;

    [Header("UI Info Rotasi Layar")]
    [Tooltip("Masukkan GameObject induk UI_InfoRotasi ke sini")]
    public GameObject uiInfoRotasi;

    [Header("Pengaturan Kecepatan & Smoothness")]
    public float durasiPerGelombang = 1.3f; 
    public float jedaAntarGelombang = 0.35f; 
    public AnimationCurve kurvaTransisi = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private CanvasGroup canvasGroup;
    private bool sedangPindah = false;

    private Dictionary<RectTransform, Vector2> posisiTutup = new Dictionary<RectTransform, Vector2>();
    private Dictionary<RectTransform, Vector2> arahBuka = new Dictionary<RectTransform, Vector2>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            canvasGroup = GetComponent<CanvasGroup>();

            // Pastikan UI mati dari awal
            if (uiInfoRotasi != null) uiInfoRotasi.SetActive(false);

            InisialisasiDataAwan();
            StartCoroutine(BukaLayarAwan());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InisialisasiDataAwan()
    {
        float w = 2500f; 
        float h = 1500f;

        HitungArahPojokOtomatis(gelombangPertama, w, h);
        HitungArahPojokOtomatis(gelombangKedua, w, h);
    }

    private void HitungArahPojokOtomatis(RectTransform[] grupAwan, float w, float h)
    {
        foreach (RectTransform awan in grupAwan)
        {
            if (awan != null)
            {
                posisiTutup[awan] = awan.anchoredPosition; 
                float arahX = (awan.anchoredPosition.x >= 0) ? w : -w; 
                float arahY = (awan.anchoredPosition.y >= 0) ? h : -h; 
                arahBuka[awan] = new Vector2(arahX, arahY);
            }
        }
    }

    public void PindahScene(string namaScene)
    {
        if (!sedangPindah)
        {
            StartCoroutine(ProsesPindahScene(namaScene));
        }
    }

    private IEnumerator ProsesPindahScene(string namaScene)
    {
        sedangPindah = true;
        canvasGroup.blocksRaycasts = true;

        // Cek apakah transisi ini butuh info rotasi
        string sceneSekarang = SceneManager.GetActiveScene().name;
        bool butuhInfoRotasi = (sceneSekarang == "scnMap" && namaScene == "scnMiniGame") || 
                               (sceneSekarang == "scnMiniGame" && namaScene == "scnMap");

        // 1. TUTUP LAYAR (Awan merapat + Kabut menebal)
        yield return StartCoroutine(AnimasiGelombang(true));

        // Munculkan UI Putar Layar jika kondisinya terpenuhi
        if (butuhInfoRotasi && uiInfoRotasi != null)
        {
            uiInfoRotasi.SetActive(true);
            
            // Jeda tambahan sedikit biar pemain sadar harus putar HP (bebas disesuaikan)
            yield return new WaitForSecondsRealtime(1.2f); 
        }

        // 2. LOADING SCENE DI BACKGROUND
        yield return SceneManager.LoadSceneAsync(namaScene);

        yield return null; 
        yield return new WaitForSecondsRealtime(0.3f); 

        // Matikan kembali UI info rotasi sebelum awan terbuka
        if (uiInfoRotasi != null)
        {
            uiInfoRotasi.SetActive(false);
        }

        // 3. BUKA LAYAR (Awan menyingkir + Kabut memudar)
        yield return StartCoroutine(AnimasiGelombang(false));

        canvasGroup.blocksRaycasts = false;
        sedangPindah = false;
    }

    private IEnumerator BukaLayarAwan()
    {
        sedangPindah = true;
        canvasGroup.blocksRaycasts = true;
        if (uiInfoRotasi != null) uiInfoRotasi.SetActive(false);

        foreach (RectTransform awan in gelombangPertama) if (awan != null) awan.anchoredPosition = posisiTutup[awan];
        foreach (RectTransform awan in gelombangKedua) if (awan != null) awan.anchoredPosition = posisiTutup[awan];
        
        if (overlayKabut != null) overlayKabut.alpha = 1f;

        yield return null;
        yield return new WaitForSecondsRealtime(0.3f); 

        yield return StartCoroutine(AnimasiGelombang(false));

        canvasGroup.blocksRaycasts = false;
        sedangPindah = false;
    }

    private IEnumerator AnimasiGelombang(bool isTutupLayar)
    {
        float totalWaktu = durasiPerGelombang + jedaAntarGelombang;
        float waktuBerjalan = 0f;

        while (waktuBerjalan < totalWaktu)
        {
            waktuBerjalan += Time.unscaledDeltaTime;

            float progres1 = Mathf.Clamp01(waktuBerjalan / durasiPerGelombang);
            GerakkanGrup(gelombangPertama, kurvaTransisi.Evaluate(progres1), isTutupLayar);

            float progres2 = Mathf.Clamp01((waktuBerjalan - jedaAntarGelombang) / durasiPerGelombang);
            GerakkanGrup(gelombangKedua, kurvaTransisi.Evaluate(progres2), isTutupLayar);

            if (overlayKabut != null)
            {
                float progresKabut = Mathf.Clamp01(waktuBerjalan / totalWaktu);
                overlayKabut.alpha = isTutupLayar ? kurvaTransisi.Evaluate(progresKabut) : 1f - kurvaTransisi.Evaluate(progresKabut);
            }

            yield return null;
        }

        GerakkanGrup(gelombangPertama, 1f, isTutupLayar);
        GerakkanGrup(gelombangKedua, 1f, isTutupLayar);
        if (overlayKabut != null) overlayKabut.alpha = isTutupLayar ? 1f : 0f;
    }

    private void GerakkanGrup(RectTransform[] grup, float persentaseKurva, bool isTutup)
    {
        foreach (RectTransform awan in grup)
        {
            if (awan == null) continue;

            Vector2 posTengah = posisiTutup[awan];
            Vector2 posLuar = posTengah + arahBuka[awan];

            if (isTutup)
            {
                awan.anchoredPosition = Vector2.LerpUnclamped(posLuar, posTengah, persentaseKurva);
            }
            else
            {
                awan.anchoredPosition = Vector2.LerpUnclamped(posTengah, posLuar, persentaseKurva);
            }
        }
    }
}