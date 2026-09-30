using UnityEngine;
using UnityEngine.UI;

public class UISparkleEffect : MonoBehaviour
{
    [Header("Pengaturan Kedap-Kedip")]
    public float kecepatan = 2.5f; 
    public float ukuranTerkecil = 0.4f; 
    public float ukuranTerbesar = 1.15f; 

    [Header("Pengaturan Tambahan (Biar Hidup)")]
    public float kecepatanRotasi = 20f; 

    private Vector3 ukuranAsli;
    private float randomOffset;
    private Image gambarBintang;

    void Awake()
    {
        // Pindah dari Start ke Awake agar ukuran asli tersimpan lebih awal
        ukuranAsli = transform.localScale;
        gambarBintang = GetComponent<Image>();
    }

    void OnEnable()
    {
        // OnEnable memastikan offset dan rotasi diacak ulang setiap kali popup muncul
        randomOffset = Random.Range(0f, 100f); 
        
        // Acak arah bintang saat pertama kali muncul biar nggak seragam
        transform.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
    }

    void Update()
    {
        // Pakai unscaledTime biar kedipannya tetap jalan meskipun Time.timeScale = 0 (Game Pause/Win)
        float wave = Mathf.Sin((Time.unscaledTime + randomOffset) * kecepatan);
        float normalizedWave = (wave + 1f) / 2f; // Menghasilkan nilai 0 sampai 1

        // 1. Animasi Skala
        float currentScale = Mathf.Lerp(ukuranTerkecil, ukuranTerbesar, normalizedWave);
        transform.localScale = ukuranAsli * currentScale;

        // 2. Animasi Rotasi (Muter pelan terus-menerus)
        transform.Rotate(0, 0, kecepatanRotasi * Time.unscaledDeltaTime);

        // 3. Animasi Pudar/Alpha (Meredup saat kecil, terang saat besar)
        if (gambarBintang != null)
        {
            Color warna = gambarBintang.color;
            warna.a = Mathf.Lerp(0.1f, 1f, normalizedWave); 
            gambarBintang.color = warna;
        }
    }
}