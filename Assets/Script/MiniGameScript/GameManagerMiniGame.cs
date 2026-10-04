using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManagerMiniGame : MonoBehaviour
{
    [Header("Referensi Sistem")]
    public SpawnerManager spawner;
    public MascotCatcher mascot;

    [Header("Referensi UI")]
    public TextMeshProUGUI teksSkor;
    public MiniGameFailMenu panelGameOver; 
    public UICoinDisplay coinDisplayGlobal;
    
    [Header("Pengaturan Navigasi")]
    public string namaSceneKembali = "scnMap";

    private int skorTotal = 0;
    private int koinSesiIni = 0;
    private bool gameSudahMulai = false;

    void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        Time.timeScale = 1f; 
    }

    void Start()
    {
        panelGameOver.gameObject.SetActive(false);
        teksSkor.text = "0";
        koinSesiIni = 0;

        mascot.gameObject.SetActive(true);
        mascot.enabled = true;
        
        // HAPUS spawner.MulaiSpawner() dari sini agar barang tidak langsung jatuh
    }

    // Fungsi baru ini dipanggil saat player pertama kali menyentuh layar
    public void MulaiPermainan()
    {
        if (!gameSudahMulai)
        {
            gameSudahMulai = true;
            spawner.MulaiSpawner();
        }
    }

    public void TambahSkor(int poin)
    {
        skorTotal += poin;
        teksSkor.text = skorTotal.ToString();
    }

    public void TambahKoinSesi(int poin)
    {
        koinSesiIni += poin;
        if (coinDisplayGlobal != null) coinDisplayGlobal.UpdateCoinUI();
    }

    public void GameSelesai()
    {
        mascot.enabled = false; 
        spawner.HentikanSpawner();
        Time.timeScale = 0f; 
        panelGameOver.TampilkanPanel(skorTotal, koinSesiIni); 
    }
}