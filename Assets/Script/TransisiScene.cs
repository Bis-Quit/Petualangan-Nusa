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

        // 1. TUTUP LAYAR (Awan merapat + Kabut menebal)
        yield return StartCoroutine(AnimasiGelombang(true));

        // 2. LOADING SCENE DI BACKGROUND
        yield return SceneManager.LoadSceneAsync(namaScene);

        yield return null; 
        yield return new WaitForSecondsRealtime(0.3f); 

        // 3. BUKA LAYAR (Awan menyingkir + Kabut memudar)
        yield return StartCoroutine(AnimasiGelombang(false));

        canvasGroup.blocksRaycasts = false;
        sedangPindah = false;
    }

    private IEnumerator BukaLayarAwan()
    {
        sedangPindah = true;
        canvasGroup.blocksRaycasts = true;

        foreach (RectTransform awan in gelombangPertama) if (awan != null) awan.anchoredPosition = posisiTutup[awan];
        foreach (RectTransform awan in gelombangKedua) if (awan != null) awan.anchoredPosition = posisiTutup[awan];
        
        // Kunci kabut di kondisi pekat saat game baru mulai
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

            // Gerakkan Awan
            float progres1 = Mathf.Clamp01(waktuBerjalan / durasiPerGelombang);
            GerakkanGrup(gelombangPertama, kurvaTransisi.Evaluate(progres1), isTutupLayar);

            float progres2 = Mathf.Clamp01((waktuBerjalan - jedaAntarGelombang) / durasiPerGelombang);
            GerakkanGrup(gelombangKedua, kurvaTransisi.Evaluate(progres2), isTutupLayar);

            // Fade in/out Overlay Kabut menggunakan kurva yang sama
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