using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 
using System.Collections; 

public class KoperManager : MonoBehaviour
{
    public static KoperManager Instance;

    [Header("Pengaturan Peta Koper")]
    public int jumlahKolom = 5; 
    public int jumlahBaris = 4; 
    public Vector2 ukuranKotak = new Vector2(125.4f, 122.4f); 
    public bool[,] petaKoper; 
    public RectTransform itemContainer; 

    [Header("Setup Indikator Warna (Highlight)")]
    public RectTransform highlightRect; // Tarik objek Highlight_Grid ke sini
    public Image highlightImage;        // Tarik komponen Image dari Highlight_Grid ke sini

    [Header("Setup Kemenangan")]
    public Transform wadahPanelMeja; 

    [Header("Setup Animasi Koper")]
    public RectTransform koperUtamaRect; 
    public GameObject visualKoperTertutup; 
    public GameObject efekCahaya; 
    public GameObject visualKoperTerbuka; 
    
    [Header("Setup Pop-up Kemenangan")]
    public UIWinMenu winMenu;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        petaKoper = new bool[jumlahKolom, jumlahBaris];
    }

    // --- FUNGSI BARU UNTUK HIGHLIGHT GRID ---
    public void AturHighlight(int x, int y, int lebar, int tinggi)
    {
        if (highlightRect == null) return;
        
        // Cek warna: Hijau (muat) atau Merah (nabrak/penuh)
        bool muat = CekBisaMuat(x, y, lebar, tinggi);
        highlightImage.color = muat ? new Color(0.2f, 1f, 0.2f, 0.5f) : new Color(1f, 0.2f, 0.2f, 0.5f);
        
        // Sesuaikan ukuran kotak indikator dengan barang
        highlightRect.sizeDelta = new Vector2(lebar * ukuranKotak.x, tinggi * ukuranKotak.y);
        
        // Sesuaikan titik Anchor dan Pivot agar sama dengan perhitungan Grid
        highlightRect.anchorMin = new Vector2(0.5f, 0.5f);
        highlightRect.anchorMax = new Vector2(0.5f, 0.5f);
        highlightRect.pivot = new Vector2(0.5f, 0.5f);

        // Hitung posisi snap indikator
        float posX = - (itemContainer.rect.width / 2f) + (x * ukuranKotak.x) + ((lebar * ukuranKotak.x) / 2f);
        float posY = (itemContainer.rect.height / 2f) - (y * ukuranKotak.y) - ((tinggi * ukuranKotak.y) / 2f);
        
        highlightRect.anchoredPosition = new Vector2(posX, posY);
        highlightRect.gameObject.SetActive(true);
    }

    public void SembunyikanHighlight()
    {
        if (highlightRect != null) highlightRect.gameObject.SetActive(false);
    }
    // ----------------------------------------

    public bool CekBisaMuat(int startX, int startY, int lebarItem, int tinggiItem)
    {
        if (startX < 0 || startY < 0 || startX + lebarItem > jumlahKolom || startY + tinggiItem > jumlahBaris)
        {
            return false; 
        }

        for (int x = startX; x < startX + lebarItem; x++)
        {
            for (int y = startY; y < startY + tinggiItem; y++)
            {
                if (petaKoper[x, y] == true) 
                {
                    return false; 
                }
            }
        }
        return true; 
    }

    public void CekKemenangan()
    {
        if (wadahPanelMeja != null && wadahPanelMeja.childCount == 0)
        {
            StartCoroutine(AnimasiKemasDanPergi());
        }
    }

    private IEnumerator AnimasiKemasDanPergi()
    {
        yield return new WaitForSeconds(0.5f);

        float waktu = 0;
        float durasi = 1f; 
        Vector2 posisiAwal = koperUtamaRect.anchoredPosition;
        Vector2 posisiTarget = new Vector2(360f, 0f); 

        while (waktu < durasi)
        {
            waktu += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, waktu / durasi);
            koperUtamaRect.anchoredPosition = Vector2.Lerp(posisiAwal, posisiTarget, t);
            yield return null;
        }

        if (visualKoperTertutup != null) visualKoperTertutup.SetActive(true);
        if (efekCahaya != null) efekCahaya.SetActive(true);
        if (visualKoperTerbuka != null) visualKoperTerbuka.SetActive(false); 

        yield return new WaitForSeconds(2f);

        if (efekCahaya != null) efekCahaya.SetActive(false);

        waktu = 0;
        durasi = 0.8f; 
        posisiAwal = koperUtamaRect.anchoredPosition; 
        Vector2 posisiAtas = new Vector2(posisiAwal.x, 1500f); 

        while (waktu < durasi)
        {
            waktu += Time.deltaTime;
            float t = waktu / durasi;
            float easeIn = t * t; 
            koperUtamaRect.anchoredPosition = Vector2.Lerp(posisiAwal, posisiAtas, easeIn);
            yield return null;
        }

        if (winMenu != null)
        {
            winMenu.TampilkanMenang(3);
        }
    }
}