using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class AnimasiPanelInfo : MonoBehaviour
{
    [Header("Pengaturan Animasi Muncul")]
    public float durasiAnimasi = 0.5f;
    public Vector2 offsetPosisiAwal = new Vector2(0, -50f); // Mulai dari agak bawah
    public AnimationCurve kurvaAnimasi = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 posisiAsli;
    private bool sudahInisialisasi = false;

    private void Awake()
    {
        Inisialisasi();
    }

    private void Inisialisasi()
    {
        if (sudahInisialisasi) return;
        
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        posisiAsli = rectTransform.anchoredPosition;
        
        sudahInisialisasi = true;
    }

    private void OnEnable()
    {
        // Pastikan referensi aman sebelum Coroutine jalan
        Inisialisasi(); 
        StartCoroutine(MulaiAnimasi());
    }

    private IEnumerator MulaiAnimasi()
    {
        float waktu = 0f;
        
        // Atur posisi dan transparansi ke kondisi awal (menghilang & di bawah)
        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = posisiAsli + offsetPosisiAwal;

        while (waktu < durasiAnimasi)
        {
            waktu += Time.unscaledDeltaTime; // Wajib unscaled agar jalan saat transisi
            float progres = waktu / durasiAnimasi;
            float nilaiKurva = kurvaAnimasi.Evaluate(progres);

            // Transisi transparansi dan pergerakan
            canvasGroup.alpha = nilaiKurva;
            rectTransform.anchoredPosition = Vector2.LerpUnclamped(posisiAsli + offsetPosisiAwal, posisiAsli, nilaiKurva);

            yield return null;
        }

        // Kunci di posisi akhir biar presisi
        canvasGroup.alpha = 1f;
        rectTransform.anchoredPosition = posisiAsli;
    }
}