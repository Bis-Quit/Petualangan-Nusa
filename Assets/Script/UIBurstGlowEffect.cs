using UnityEngine;
using UnityEngine.UI;

public class UIBurstGlowEffect : MonoBehaviour
{
    [Header("Pengaturan Redup-Terang (Fading)")]
    public float kecepatanAnimasi = 2f;
    public float alphaMin = 0.3f;
    public float alphaMax = 1f;

    [Header("Pengaturan Ukuran (Breathing)")]
    public float skalaMin = 0.95f; // Mengecil sedikit
    public float skalaMax = 1.05f; // Membesar sedikit

    private Image gambarCahaya;
    private Vector3 skalaAwal;

    void Awake()
    {
        gambarCahaya = GetComponent<Image>();
        skalaAwal = transform.localScale;
    }

    void Update()
    {
        // Menghasilkan nilai gelombang mulus dari 0 ke 1
        float kurva = (Mathf.Sin(Time.unscaledTime * kecepatanAnimasi) + 1f) / 2f;

        // 1. Terapkan efek mengembang dan mengempis (Breathing)
        float ukuranSekarang = Mathf.Lerp(skalaMin, skalaMax, kurva);
        transform.localScale = skalaAwal * ukuranSekarang;

        // 2. Terapkan efek redup terang (Glow/Fade)
        if (gambarCahaya != null)
        {
            Color warna = gambarCahaya.color;
            warna.a = Mathf.Lerp(alphaMin, alphaMax, kurva);
            gambarCahaya.color = warna;
        }
    }
}