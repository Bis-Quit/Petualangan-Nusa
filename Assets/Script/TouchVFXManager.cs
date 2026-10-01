using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TouchVFXManager : MonoBehaviour
{
    public static TouchVFXManager Instance;

    [Header("Pengaturan Efek Magis")]
    [Tooltip("Gunakan sprite Bintang 4 Sudut (Diamond) atau siluet Daun kecil.")]
    public GameObject prefabEfekKlik; 
    
    private Canvas canvasGlobal;

    [Header("Pengaturan Animasi")]
    public float durasiEfek = 0.8f; 
    public float skalaMaksimal = 0.8f;

    [Header("Pengaturan Jejak (Drag/Scroll)")]
    [Tooltip("Jarak minimum jari bergeser sebelum debu baru muncul (makin kecil makin rapat)")]
    public float jarakMinimumDrag = 40f; 
    private Vector2 posisiTerakhirDrag;
    private bool sedangDrag = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuatCanvasGlobal();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void BuatCanvasGlobal()
    {
        GameObject canvasObj = new GameObject("Canvas_VFX_Global");
        DontDestroyOnLoad(canvasObj);
        
        canvasGlobal = canvasObj.AddComponent<Canvas>();
        canvasGlobal.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGlobal.sortingOrder = 999; 

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080); 
        
        GraphicRaycaster raycaster = canvasObj.AddComponent<GraphicRaycaster>();
        raycaster.enabled = false; 
    }

    void Update()
    {
        // ==========================================
        // 1. DETEKSI MOUSE (PC / MAC TESTER)
        // ==========================================
        if (Input.GetMouseButtonDown(0))
        {
            TriggerDebuMagis(Input.mousePosition); // Ledakan awal
            posisiTerakhirDrag = Input.mousePosition;
            sedangDrag = true;
        }
        else if (Input.GetMouseButton(0) && sedangDrag)
        {
            Vector2 posisiSekarang = Input.mousePosition;
            
            // Cek apakah kursor sudah geser cukup jauh
            if (Vector2.Distance(posisiSekarang, posisiTerakhirDrag) > jarakMinimumDrag)
            {
                TriggerEfekDrag(posisiSekarang);
                posisiTerakhirDrag = posisiSekarang; // Reset titik ukur
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            sedangDrag = false;
        }

        // ==========================================
        // 2. DETEKSI SENTUHAN (MOBILE DEVICE)
        // ==========================================
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                TriggerDebuMagis(touch.position); // Ledakan awal
                posisiTerakhirDrag = touch.position;
                sedangDrag = true;
            }
            else if (touch.phase == TouchPhase.Moved && sedangDrag)
            {
                if (Vector2.Distance(touch.position, posisiTerakhirDrag) > jarakMinimumDrag)
                {
                    TriggerEfekDrag(touch.position);
                    posisiTerakhirDrag = touch.position;
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                sedangDrag = false;
            }
        }
    }

    // --- FUNGSI TRIGGER AWAL KETIKA DIKLIK ---
    private void TriggerDebuMagis(Vector2 posisiLayar)
    {
        if (prefabEfekKlik == null || canvasGlobal == null) return;

        SpawnPartikelMagis(posisiLayar, skalaMaksimal * 1.5f, true);

        int jumlahDebu = Random.Range(3, 5);
        for (int i = 0; i < jumlahDebu; i++)
        {
            SpawnPartikelMagis(posisiLayar, Random.Range(0.2f, 0.5f), false);
        }
    }

    // --- FUNGSI TRIGGER BARU KETIKA DI-DRAG ---
    private void TriggerEfekDrag(Vector2 posisiLayar)
    {
        if (prefabEfekKlik == null || canvasGlobal == null) return;

        // Cukup keluarkan 1 atau 2 debu kecil sebagai jejak (trail) biar gak menuhin layar
        int jumlahDebu = Random.Range(1, 3);
        for (int i = 0; i < jumlahDebu; i++)
        {
            // Ukuran dibuat sedikit lebih kecil dari debu klik
            SpawnPartikelMagis(posisiLayar, Random.Range(0.15f, 0.35f), false);
        }
    }

    private void SpawnPartikelMagis(Vector2 posisi, float targetScale, bool isCore)
    {
        GameObject partikel = Instantiate(prefabEfekKlik);
        partikel.transform.SetParent(canvasGlobal.transform, false);

        RectTransform rect = partikel.GetComponent<RectTransform>();
        rect.position = posisi; 
        
        Image img = partikel.GetComponent<Image>();
        if (img != null) img.raycastTarget = false; 

        StartCoroutine(AnimasiDebuMagis(partikel, rect, img, targetScale, isCore));
    }

    private IEnumerator AnimasiDebuMagis(GameObject obj, RectTransform rect, Image img, float targetScale, bool isCore)
    {
        float waktu = 0f;
        float durasi = isCore ? durasiEfek * 0.5f : durasiEfek * Random.Range(0.8f, 1.2f); 
        
        Vector2 posisiAwal = rect.anchoredPosition;
        float targetY = posisiAwal.y + Random.Range(80f, 150f); 
        
        float swaySpeed = Random.Range(2f, 5f);
        float swayAmount = Random.Range(15f, 40f);
        float swayOffset = Random.Range(0f, Mathf.PI * 2f); 

        float rotasiAwal = Random.Range(0f, 360f);
        float arahRotasi = Random.value > 0.5f ? 1f : -1f;

        while (waktu < durasi)
        {
            // --- PERUBAHAN DI SINI: Gunakan unscaledDeltaTime ---
            waktu += Time.unscaledDeltaTime; 
            
            float t = waktu / durasi; 
            
            if (isCore)
            {
                float pop = targetScale * Mathf.Sin(t * Mathf.PI);
                rect.localScale = Vector3.one * pop;
                rect.localRotation = Quaternion.Euler(0, 0, rotasiAwal + (t * 90f * arahRotasi));
            }
            else
            {
                float currentY = Mathf.Lerp(posisiAwal.y, targetY, t);
                float currentX = posisiAwal.x + Mathf.Sin((waktu * swaySpeed) + swayOffset) * swayAmount;
                
                rect.anchoredPosition = new Vector2(currentX, currentY);
                rect.localRotation = Quaternion.Euler(0, 0, rotasiAwal + (waktu * 60f * arahRotasi));
                
                float scaleCurve = Mathf.Sin(t * Mathf.PI); 
                rect.localScale = Vector3.one * (targetScale * scaleCurve);
            }

            if (img != null)
            {
                Color warna = img.color;
                warna.a = 1f - t; 
                img.color = warna;
            }

            yield return null;
        }

        Destroy(obj);
    }
}