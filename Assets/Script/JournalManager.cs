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

    [Header("Halaman Kanan (Benda)")]
    public ItemDataSO dataBenda;
    public string asalBenda;
    
    [TextArea(3, 5)]
    [Tooltip("Isi dengan sejarah atau penjelasan barang")]
    public string deskripsiBenda; // BARU: Menampung teks panjang
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
    public Image portraitBenda;
    public TextMeshProUGUI teksNamaBenda;
    public TextMeshProUGUI teksAsalBenda;
    public TextMeshProUGUI teksDeskripsiBenda; // BARU: Slot UI Deskripsi

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

        if (portraitDaerah != null) portraitDaerah.sprite = data.gambarDaerah;
        if (teksHeadline != null) teksHeadline.text = data.namaDaerah;
        if (teksSubHeadline != null) teksSubHeadline.text = data.julukanDaerah;

        SetupSlotBenda(data, portraitBenda, teksNamaBenda, teksAsalBenda, teksDeskripsiBenda);
    }

    private void SetupSlotBenda(DataHalamanJurnal data, Image foto, TextMeshProUGUI teksNama, TextMeshProUGUI asal, TextMeshProUGUI deskripsi)
    {
        if (foto == null) return;

        GameObject bingkaiPolaroid = foto.transform.parent.gameObject;

        if (data.dataBenda == null) 
        {
            bingkaiPolaroid.SetActive(false); 
            if (teksNama != null) teksNama.gameObject.SetActive(false); 
            if (asal != null) asal.gameObject.SetActive(false); 
            if (deskripsi != null) deskripsi.gameObject.SetActive(false);
            return;
        }

        bingkaiPolaroid.SetActive(true); 
        if (teksNama != null) teksNama.gameObject.SetActive(true); 
        if (asal != null) asal.gameObject.SetActive(true); 
        if (deskripsi != null) deskripsi.gameObject.SetActive(true);

        bool sudahKetemu = PlayerPrefs.GetInt("Koleksi_" + data.dataBenda.namaItem, 0) == 1;

        if (sudahKetemu)
        {
            foto.sprite = data.dataBenda.gambarItem;
            foto.transform.localScale = Vector3.one; 
            if (teksNama != null) teksNama.text = data.dataBenda.namaItem;
            if (asal != null) asal.text = data.asalBenda;
            if (deskripsi != null) deskripsi.text = data.deskripsiBenda;
        }
        else
        {
            foto.sprite = spriteTandaTanya;
            foto.transform.localScale = new Vector3(0.5f, 0.5f, 1f); 
            if (teksNama != null) teksNama.text = "???";
            if (asal != null) asal.text = "???";
            if (deskripsi != null) deskripsi.text = "Deskripsi belum tersedia. Temukan pusaka ini terlebih dahulu untuk membaca informasinya.";
        }
    }

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