using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; 
using System.Collections;

[System.Serializable]
public class DataHalamanJurnal
{
    [Header("Halaman Kiri (Daerah)")]
    public string namaDaerah;      
    public string julukanDaerah;   
    public Sprite gambarDaerah;    

    [Header("Halaman Kanan (Benda 1)")]
    public ItemDataSO dataBenda1;
    public string asalBenda1;      

    [Header("Halaman Kanan (Benda 2)")]
    public ItemDataSO dataBenda2;
    public string asalBenda2;      
}

public class JournalManager : MonoBehaviour
{
    [Header("Database Jurnal")]
    public DataHalamanJurnal[] daftarHalaman;
    private int halamanAktif = 0;

    [Header("Referensi UI: Halaman Kiri")]
    public GameObject wadahHalamanKiri;
    public Image portraitDaerah;
    public TextMeshProUGUI teksHeadline;
    public TextMeshProUGUI teksSubHeadline;

    [Header("Referensi UI: Halaman Kanan")]
    public GameObject wadahHalamanKanan;
    public Image portraitBenda1;
    public TextMeshProUGUI teksNamaBenda1;
    public TextMeshProUGUI teksAsalBenda1;
    
    public Image portraitBenda2;
    public TextMeshProUGUI teksNamaBenda2;
    public TextMeshProUGUI teksAsalBenda2;

    [Header("Aset Global")]
    public Sprite spriteTandaTanya; 

    [Header("Animasi Kertas")]
    public GameObject turningPage; 
    public float durasiBalik = 0.2f;

    [Header("Pengaturan Swipe")]
    public float minJarakSwipe = 50f; 
    private Vector2 posisiAwalSentuh;
    private Vector2 posisiAkhirSentuh;

    [Header("Pengaturan Kembali")]
    public string namaSceneKembali = "scnMap"; 

    private bool sedangAnimasi = false;

    void Start()
    {
        if (turningPage != null) turningPage.SetActive(false);
        MuatDataHalaman(halamanAktif);
    }

    void Update()
    {
        if (sedangAnimasi) return;

        // 1. Deteksi Swipe di HP (Touch)
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                posisiAwalSentuh = touch.position;
                posisiAkhirSentuh = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                posisiAkhirSentuh = touch.position;
                CekArahSwipe();
            }
        }
        // 2. Deteksi Mouse Drag
        else if (Input.GetMouseButtonDown(0))
        {
            posisiAwalSentuh = Input.mousePosition;
            posisiAkhirSentuh = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            posisiAkhirSentuh = Input.mousePosition;
            CekArahSwipe();
        }
    }

    private void CekArahSwipe()
    {
        float jarakGeserX = posisiAkhirSentuh.x - posisiAwalSentuh.x;
        float jarakGeserY = posisiAkhirSentuh.y - posisiAwalSentuh.y;

        if (Mathf.Abs(jarakGeserX) > minJarakSwipe && Mathf.Abs(jarakGeserX) > Mathf.Abs(jarakGeserY))
        {
            if (jarakGeserX < 0) KlikHalamanBerikutnya();
            else KlikHalamanSebelumnya();
        }
    }

    private void MuatDataHalaman(int index)
    {
        if (daftarHalaman.Length == 0) return;
        DataHalamanJurnal data = daftarHalaman[index];

        // --- SET HALAMAN KIRI ---
        if (portraitDaerah != null) portraitDaerah.sprite = data.gambarDaerah;
        if (teksHeadline != null) teksHeadline.text = data.namaDaerah;
        if (teksSubHeadline != null) teksSubHeadline.text = data.julukanDaerah;

        // --- SET HALAMAN KANAN ---
        SetupSlotBenda(data.dataBenda1, data.asalBenda1, portraitBenda1, teksNamaBenda1, teksAsalBenda1);
        SetupSlotBenda(data.dataBenda2, data.asalBenda2, portraitBenda2, teksNamaBenda2, teksAsalBenda2);
    }

    private void SetupSlotBenda(ItemDataSO dataBenda, string teksAsal, Image foto, TextMeshProUGUI teksNama, TextMeshProUGUI asal)
    {
        if (foto == null) return;

        // Tangkap objek bingkai utama (Parent dari foto)
        GameObject bingkaiPolaroid = foto.transform.parent.gameObject;

        // KALAU BARANGNYA KOSONG (Cuma ada 1 barang di daerah ini)
        if (dataBenda == null) 
        {
            bingkaiPolaroid.SetActive(false); 
            if (teksNama != null) teksNama.gameObject.SetActive(false); 
            if (asal != null) asal.gameObject.SetActive(false); 
            return;
        }

        // KALAU BARANGNYA ADA
        bingkaiPolaroid.SetActive(true); 
        if (teksNama != null) teksNama.gameObject.SetActive(true); 
        if (asal != null) asal.gameObject.SetActive(true); 

        // Cek save data
        bool sudahKetemu = PlayerPrefs.GetInt("Koleksi_" + dataBenda.namaItem, 0) == 1;

        if (sudahKetemu)
        {
            foto.sprite = dataBenda.gambarItem;
            foto.transform.localScale = Vector3.one; // Kembalikan ke ukuran 100%
            if (teksNama != null) teksNama.text = dataBenda.namaItem;
            if (asal != null) asal.text = teksAsal;
        }
        else
        {
            foto.sprite = spriteTandaTanya;
            foto.transform.localScale = new Vector3(0.5f, 0.5f, 1f); // Kecilkan jadi 50%
            if (teksNama != null) teksNama.text = "???";
            if (asal != null) asal.text = "???";
        }
    }

    // --- FUNGSI INTERAKSI TOMBOL ---
    public void KlikHalamanBerikutnya()
    {
        if (sedangAnimasi || halamanAktif >= daftarHalaman.Length - 1) return;
        StartCoroutine(ProsesBalikHalaman(1));
    }

    public void KlikHalamanSebelumnya()
    {
        if (sedangAnimasi || halamanAktif <= 0) return;
        StartCoroutine(ProsesBalikHalaman(-1));
    }

    private IEnumerator ProsesBalikHalaman(int arah)
    {
        sedangAnimasi = true;

        if (AudioManager.Instance != null && AudioManager.Instance.sfxKertasJurnal != null)
        {
            AudioManager.Instance.MainkanSFX(AudioManager.Instance.sfxKertasJurnal);
        }

        if (wadahHalamanKiri != null) wadahHalamanKiri.SetActive(false);
        if (wadahHalamanKanan != null) wadahHalamanKanan.SetActive(false);
        if (turningPage != null) turningPage.SetActive(true);

        yield return new WaitForSeconds(durasiBalik);

        halamanAktif += arah;
        MuatDataHalaman(halamanAktif);

        if (turningPage != null) turningPage.SetActive(false);
        if (wadahHalamanKiri != null) wadahHalamanKiri.SetActive(true);
        if (wadahHalamanKanan != null) wadahHalamanKanan.SetActive(true);

        sedangAnimasi = false;
    }

    public void TombolTutup()
    {
        TransisiScene.Instance.PindahScene(namaSceneKembali);
    }
}