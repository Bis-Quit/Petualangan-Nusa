using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class StoryComicManager : MonoBehaviour
{
    [Header("Komponen Utama")]
    public ScrollRect scrollRect;
    
    [Header("Pengaturan Scroll")]
    [Tooltip("Semakin kecil nilainya, semakin lambat. Contoh: 0.005 untuk komik panjang")]
    public float kecepatanScroll = 0.005f; 
    public float jedaAwal = 2.0f; 

    [Header("Pengaturan Tombol Close")]
    public GameObject tombolClose; 
    public float waktuTampilTombol = 3f; 
    public string namaSceneKembali = "scnMainMenu"; 

    private bool isAutoScrolling = false;
    private Coroutine hideButtonCoroutine;

    void Start()
    {
        // Pastikan posisi komik ada di paling atas saat baru mulai
        scrollRect.verticalNormalizedPosition = 1f;
        
        // Sembunyikan tombol close di awal
        if (tombolClose != null) tombolClose.SetActive(false);

        // Mulai jalankan auto-scroll setelah jeda
        Invoke(nameof(MulaiAutoScroll), jedaAwal);
    }

    private void MulaiAutoScroll()
    {
        isAutoScrolling = true;
    }

    void Update()
    {
        // 1. DETEKSI SENTUHAN GLOBAL (Bypass halangan UI)
        // Mengecek apakah ada klik mouse kiri ATAU sentuhan jari di layar HP
        bool isJariMenyentuh = Input.GetMouseButton(0) || Input.touchCount > 0;
        bool isBaruSajaDisentuh = Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        // Jika layar baru saja di-tap, langsung munculkan tombol Close
        if (isBaruSajaDisentuh)
        {
            TampilkanTombolClose();
        }

        // 2. LOGIKA SCROLL
        // Auto-scroll jalan kalau jari nggak nempel dan belum mentok bawah
        if (isAutoScrolling && !isJariMenyentuh)
        {
            scrollRect.verticalNormalizedPosition -= kecepatanScroll * Time.deltaTime;

            if (scrollRect.verticalNormalizedPosition <= 0.001f)
            {
                scrollRect.verticalNormalizedPosition = 0f;
                isAutoScrolling = false; 
            }
        }
    }

    private void TampilkanTombolClose()
    {
        if (tombolClose != null)
        {
            tombolClose.SetActive(true);
            
            // Hapus timer lama dan mulai hitung mundur 3 detik dari awal
            if (hideButtonCoroutine != null) StopCoroutine(hideButtonCoroutine);
            hideButtonCoroutine = StartCoroutine(HitungMundurSembunyi());
        }
    }

    private IEnumerator HitungMundurSembunyi()
    {
        // Tunggu 3 detik tanpa interaksi
        yield return new WaitForSeconds(waktuTampilTombol);
        
        // Sembunyikan tombolnya lagi
        if (tombolClose != null) tombolClose.SetActive(false);
    }

    // Fungsi ini dipanggil saat tombol Close diklik
    public void KeluarDariCerita()
    {
        SceneManager.LoadScene(namaSceneKembali);
    }
}