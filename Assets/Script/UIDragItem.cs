using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 
using System.Collections;

public class UIDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Data Utama")]
    public ItemDataSO dataPusaka; 

    [Header("Referensi Petunjuk")]
    public GameObject hintRotate; 

    [HideInInspector] public int lebarItem;
    [HideInInspector] public int tinggiItem;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image itemImage; 
    
    [HideInInspector] public Transform parentAsli; 
    [HideInInspector] public Vector2 posisiAwal;

    private bool sudahDiKoper = false;
    private int gridX;
    private int gridY;
    
    private bool isRotated = false; 
    private bool isDragging = false; 

    private bool isMenungguKlikKedua = false;
    private float timerJedaKlik = 0f;
    private float batasWaktuJeda = 3.5f; 

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        itemImage = GetComponent<Image>();
    }

    void Start()
    {
        if (dataPusaka != null)
        {
            lebarItem = dataPusaka.lebar;
            tinggiItem = dataPusaka.tinggi;
            
            if (KoperManager.Instance != null)
            {
                rectTransform.sizeDelta = new Vector2(
                    dataPusaka.lebar * KoperManager.Instance.ukuranKotak.x,
                    dataPusaka.tinggi * KoperManager.Instance.ukuranKotak.y
                );
            }
            
            if (itemImage != null && dataPusaka.gambarItem != null)
            {
                itemImage.sprite = dataPusaka.gambarItem;
                itemImage.preserveAspect = true; 
            }
        }
    }

    void Update()
    {
        if (isMenungguKlikKedua)
        {
            timerJedaKlik += Time.deltaTime;
            
            if (timerJedaKlik >= batasWaktuJeda)
            {
                isMenungguKlikKedua = false;
                MatikanHintDenganFade();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDragging) return;

        if (sudahDiKoper)
        {
            KosongkanMemoriKoper();
            KembalikanKePosisiAwal();
        }

        if (!isMenungguKlikKedua)
        {
            StartCoroutine(AnimasiBouncy());
            isMenungguKlikKedua = true;
            timerJedaKlik = 0f; 
            
            if (hintRotate != null) hintRotate.SetActive(true);
        }
        else
        {
            PutarBarang();
        }
    }

    private IEnumerator AnimasiBouncy()
    {
        Vector3 skalaAwal = Vector3.one; 
        float durasi = 0.12f;
        float waktu = 0;
        
        while (waktu < durasi)
        {
            waktu += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(skalaAwal, skalaAwal * 1.15f, waktu / durasi);
            yield return null;
        }
        
        waktu = 0;
        while (waktu < durasi)
        {
            waktu += Time.deltaTime;
            rectTransform.localScale = Vector3.Lerp(skalaAwal * 1.15f, skalaAwal, waktu / durasi);
            yield return null;
        }
        rectTransform.localScale = skalaAwal;
    }

    private void PutarBarang()
    {
        isRotated = !isRotated;
        isMenungguKlikKedua = false; 

        MatikanHintDenganFade();

        int ukuranSementara = lebarItem;
        lebarItem = tinggiItem;
        tinggiItem = ukuranSementara;

        if (isRotated)
            rectTransform.localRotation = Quaternion.Euler(0, 0, -90f);
        else
            rectTransform.localRotation = Quaternion.Euler(0, 0, 0f);
    }

    private void MatikanHintDenganFade()
    {
        if (hintRotate != null && hintRotate.activeSelf)
        {
            HintAnimator animator = hintRotate.GetComponent<HintAnimator>();
            if (animator != null) 
            {
                animator.FadeOutDanMati();
            }
            else 
            {
                hintRotate.SetActive(false);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true; 
        isMenungguKlikKedua = false;
        if (hintRotate != null) hintRotate.SetActive(false);
        
        if (!sudahDiKoper)
        {
            posisiAwal = rectTransform.anchoredPosition;
            parentAsli = transform.parent;
        }
        
        canvasGroup.alpha = 0.7f;
        canvasGroup.blocksRaycasts = false; 
        
        if (sudahDiKoper) KosongkanMemoriKoper();

        transform.SetParent(GetComponentInParent<Canvas>().transform, true); 
        
        if (KoperManager.Instance != null)
        {
            rectTransform.sizeDelta = new Vector2(
                dataPusaka.lebar * KoperManager.Instance.ukuranKotak.x,
                dataPusaka.tinggi * KoperManager.Instance.ukuranKotak.y
            );
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / GetComponentInParent<Canvas>().scaleFactor;

        // --- SISTEM INDIKATOR WARNA SAAT DI-DRAG ---
        int targetX, targetY;
        bool diAreaKoper;
        HitungPosisiGrid(eventData, out targetX, out targetY, out diAreaKoper);

        if (diAreaKoper)
        {
            KoperManager.Instance.AturHighlight(targetX, targetY, lebarItem, tinggiItem);
        }
        else
        {
            KoperManager.Instance.SembunyikanHighlight();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false; 
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        if (KoperManager.Instance != null) KoperManager.Instance.SembunyikanHighlight();

        CobaMasukKoper(eventData);
    }

    // --- SENSOR TERBARU (Sistem Magnet + Clamp Batas Koper) ---
    private void HitungPosisiGrid(PointerEventData eventData, out int targetX, out int targetY, out bool diAreaKoper)
    {
        targetX = -1;
        targetY = -1;
        
        RectTransform container = KoperManager.Instance.itemContainer;
        diAreaKoper = RectTransformUtility.RectangleContainsScreenPoint(container, eventData.position, eventData.pressEventCamera);

        if (diAreaKoper)
        {
            Vector2 posisiMouseLokal;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(container, eventData.position, eventData.pressEventCamera, out posisiMouseLokal);

            // Geser titik baca ke ujung kiri atas objek agar selaras dengan grid
            float posX = posisiMouseLokal.x + (container.rect.width / 2f) - (rectTransform.rect.width / 2f);
            float posY = (container.rect.height / 2f) - posisiMouseLokal.y - (rectTransform.rect.height / 2f); 

            // Menggunakan RoundToInt agar otomatis mencari kotak grid terdekat
            targetX = Mathf.RoundToInt(posX / KoperManager.Instance.ukuranKotak.x);
            targetY = Mathf.RoundToInt(posY / KoperManager.Instance.ukuranKotak.y);

            // --- KODE BARU: Cegah Bocor (Clamp / Mentok di pinggir) ---
            // Memaksa nilai X dan Y tidak pernah melebihi ukuran maksimal koper dikurangi ukuran barang
            targetX = Mathf.Clamp(targetX, 0, KoperManager.Instance.jumlahKolom - lebarItem);
            targetY = Mathf.Clamp(targetY, 0, KoperManager.Instance.jumlahBaris - tinggiItem);
            // -----------------------------------------------------------
        }
    }

    private void CobaMasukKoper(PointerEventData eventData)
    {
        if (KoperManager.Instance == null) 
        {
            KembalikanKePosisiAwal();
            return;
        }

        int targetX, targetY;
        bool diAreaKoper;
        HitungPosisiGrid(eventData, out targetX, out targetY, out diAreaKoper); // Pakai sensor baru

        if (diAreaKoper)
        {
            if (KoperManager.Instance.CekBisaMuat(targetX, targetY, lebarItem, tinggiItem))
            {
                transform.SetParent(KoperManager.Instance.itemContainer, true);
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.pivot = new Vector2(0.5f, 0.5f);

                for (int x = targetX; x < targetX + lebarItem; x++)
                    for (int y = targetY; y < targetY + tinggiItem; y++)
                        KoperManager.Instance.petaKoper[x, y] = true;

                sudahDiKoper = true;
                gridX = targetX;
                gridY = targetY;

                SnapKeGrid(targetX, targetY);
                KoperManager.Instance.CekKemenangan(); 
                return;
            }
        }
        KembalikanKePosisiAwal();
    }

    private void SnapKeGrid(int x, int y)
    {
        RectTransform container = KoperManager.Instance.itemContainer;
        Vector2 ukuranKotak = KoperManager.Instance.ukuranKotak;
        float startX = - (container.rect.width / 2f) + (x * ukuranKotak.x);
        float startY = (container.rect.height / 2f) - (y * ukuranKotak.y);
        float finalX = startX + ((lebarItem * ukuranKotak.x) / 2f);
        float finalY = startY - ((tinggiItem * ukuranKotak.y) / 2f);
        rectTransform.anchoredPosition = new Vector2(finalX, finalY);
    }

    private void KosongkanMemoriKoper()
    {
        for (int x = gridX; x < gridX + lebarItem; x++)
            for (int y = gridY; y < gridY + tinggiItem; y++)
                KoperManager.Instance.petaKoper[x, y] = false;
        sudahDiKoper = false;
    }

    public void KembalikanKePosisiAwal()
    {
        transform.SetParent(parentAsli, true);
        rectTransform.anchoredPosition = posisiAwal;
        sudahDiKoper = false;
    }
}