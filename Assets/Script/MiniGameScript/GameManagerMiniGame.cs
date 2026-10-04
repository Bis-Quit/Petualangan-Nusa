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
    public string namaSceneKembali = "mainMenu";

    private int skorTotal = 0;
    private int koinSesiIni = 0;

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
        spawner.MulaiSpawner();
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
        // Cukup matikan interaksinya, wujud maskot tetap dibiarkan aktif di layar
        mascot.enabled = false; 
        
        spawner.HentikanSpawner();
        Time.timeScale = 0f; 
        
        panelGameOver.TampilkanPanel(skorTotal, koinSesiIni); 
    }
}