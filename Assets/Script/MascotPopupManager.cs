using UnityEngine;
using UnityEngine.UI; // Wajib ditambah biar bisa akses komponen Image
using TMPro; 
using UnityEngine.SceneManagement;

public class MascotPopupManager : MonoBehaviour
{
    public static MascotPopupManager Instance;

    [Header("UI Elements")]
    public GameObject popupPanel;
    public TextMeshProUGUI chatText;
    
    // --- TAMBAHAN BARU: Referensi Gambar ---
    public Image popupMascotImage; // Wadah gambar maskot di UI Popup

    [Header("Data Skin Maskot")]
    public Sprite[] mascotSkins; // Daftar semua gambar wajah/baju maskot
    
    // Kunci buat nyimpen data baju di memori (harus sama persis dengan sistem toko/gallery lu nanti)
    private string prefsSkinKey = "SelectedSkinIndex"; 

    private string sceneToLoad;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (popupPanel != null) popupPanel.SetActive(false);
    }

    public void ShowMascotPopup(string namaPulau, string namaScene)
    {
        sceneToLoad = namaScene;
        
        chatText.text = "Halo kawan! Ayo bantu aku mencari barang pusaka di pulau " + namaPulau + "!";
        
        // --- TAMBAHAN BARU: Ganti baju maskot sebelum popup muncul ---
        UpdateMascotSkin();

        popupPanel.SetActive(true);
    }

    private void UpdateMascotSkin()
    {
        // Cegah error kalau kolom gambar lupa diisi di Inspector
        if (mascotSkins == null || mascotSkins.Length == 0 || popupMascotImage == null) return;

        // Ambil data skin terakhir yang dipilih pemain (kalau pemain baru, otomatis dikasih angka 0 / default)
        int currentSkinIndex = PlayerPrefs.GetInt(prefsSkinKey, 0);

        // Pastikan angkanya valid biar ga error out of bounds
        if (currentSkinIndex >= 0 && currentSkinIndex < mascotSkins.Length)
        {
            popupMascotImage.sprite = mascotSkins[currentSkinIndex];
        }
    }

    public void OnClickPlay()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public void OnClickClose()
    {
        popupPanel.SetActive(false);
    }
}