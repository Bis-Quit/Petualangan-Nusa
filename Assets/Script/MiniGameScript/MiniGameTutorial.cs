using UnityEngine;

public class MiniGameTutorial : MonoBehaviour
{
    [Header("Referensi Sistem")]
    public GameManagerMiniGame gameManager;

    [Header("Referensi UI Tutorial")]
    public GameObject bungkusTutorialUI; // Masukkan induk UI (Panel Teks & Kursor) ke sini
    public RectTransform objekKursor;    // Masukkan spesifik gambar kursor (Elips + Panah) ke sini

    [Header("Pengaturan Animasi & AFK")]
    public float jarakGeser = 150f;      // Seberapa jauh kursor bergerak ke kiri-kanan
    public float kecepatanGeser = 3f;    // Kecepatan gerak kursor
    public float batasWaktuAFK = 5f;     // Waktu didiamkan (detik) sebelum tutorial muncul lagi

    private float waktuTanpaInput = 0f;
    private Vector2 posisiAwalKursor;
    private bool isTutorialAktif = true;

    void Start()
    {
        if (objekKursor != null)
        {
            posisiAwalKursor = objekKursor.anchoredPosition;
        }
        
        // Selalu tampilkan tutorial dan bekukan game di awal
        TampilkanTutorial();
    }

    void Update()
    {
        // 1. Kalo udah game over, matiin fungsi tutorial ini biar ga bentrok
        if (gameManager != null && gameManager.panelGameOver.gameObject.activeInHierarchy) 
            return;

        // 2. Deteksi input sentuhan atau klik mouse
        if (Input.GetMouseButton(0) || Input.GetMouseButtonDown(0))
        {
            waktuTanpaInput = 0f; // Reset timer AFK
            
            if (isTutorialAktif)
            {
                SembunyikanTutorial();
                if (gameManager != null) gameManager.MulaiPermainan(); 
            }
        }
        else
        {
            // 3. Tambah waktu AFK cuma pas game lagi jalan (tutorial mati)
            if (!isTutorialAktif)
            {
                waktuTanpaInput += Time.unscaledDeltaTime; 
                
                if (waktuTanpaInput >= batasWaktuAFK)
                {
                    TampilkanTutorial();
                }
            }
        }

        // 4. Animasi kursor (Wajib pakai unscaledTime biar tetep gerak walau game ke-pause)
        if (isTutorialAktif && objekKursor != null)
        {
            float geserX = Mathf.Sin(Time.unscaledTime * kecepatanGeser) * jarakGeser;
            objekKursor.anchoredPosition = posisiAwalKursor + new Vector2(geserX, 0);
        }
    }

    private void TampilkanTutorial()
    {
        isTutorialAktif = true;
        if (bungkusTutorialUI != null) bungkusTutorialUI.SetActive(true);
        
        // PAUSE GAME: Biar barang ga jatuh dan spawner ketahan pas AFK
        Time.timeScale = 0f;
    }

    private void SembunyikanTutorial()
    {
        isTutorialAktif = false;
        if (bungkusTutorialUI != null) bungkusTutorialUI.SetActive(false);
        
        // RESUME GAME: Jalankan waktu lagi
        Time.timeScale = 1f;
    }
}