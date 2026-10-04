using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class MascotCatcher : MonoBehaviour
{
    public GameManagerMiniGame gameManager; 

    [Header("Animasi Wajah")]
    public Sprite spriteIdle;       
    public Sprite spriteMakan;      
    public Sprite spriteMati; 
    private SpriteRenderer spriteRenderer;

    [Header("Referensi Bayangan")]
    public Transform shadowTransform; 
    public Vector3 offsetShadowMati = new Vector3(0.8f, 0f, 0f); // Geser X ke kanan biar pas di perut

    [Header("Batas Geser Mascot")]
    public float batasKiri = -2.5f; 
    public float batasKanan = 2.5f; 

    [Header("Efek Visual Floating Text")]
    public GameObject floatingTextPrefab; 
    public Transform canvasUIParent;
    public float offsetTextY = 2.5f; 

    private Camera mainCam;
    private bool isDragging = false;
    private bool isDead = false; 

    void Start()
    {
        mainCam = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteIdle != null) spriteRenderer.sprite = spriteIdle;

        GetComponent<BoxCollider2D>().isTrigger = true; 
    }

    void Update()
    {
        if (isDead) return; 

        if (Input.GetMouseButtonDown(0)) isDragging = true;
        else if (Input.GetMouseButtonUp(0)) isDragging = false;

        if (isDragging)
        {
            Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            float clampedX = Mathf.Clamp(mousePos.x, batasKiri, batasKanan);
            transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        ItemJatuh item = collision.GetComponent<ItemJatuh>();
        if (item != null)
        {
            Vector3 titikSpawnDunia = transform.position + (Vector3.up * offsetTextY);

            if (item.tipeItem == TipeItemJatuh.Makanan)
            {
                if (AudioManager.Instance != null) AudioManager.Instance.MainkanSFX(AudioManager.Instance.sfxDapatBarang);
                gameManager.TambahSkor(10); 
                MunculkanVFX(titikSpawnDunia, "+10", new Color(0.2f, 0.9f, 0.2f));
                StartCoroutine(AnimasiMangap());
            }
            else if (item.tipeItem == TipeItemJatuh.Koin)
            {
                if (AudioManager.Instance != null) AudioManager.Instance.MainkanSFX(AudioManager.Instance.sfxDapatBarang); 
                if (CoinManager.Instance != null) CoinManager.Instance.AddCoins(1); 
                gameManager.TambahKoinSesi(1); 
                MunculkanVFX(titikSpawnDunia, "+1", new Color(1f, 0.8f, 0f));
                StartCoroutine(AnimasiMangap()); 
            }
            else if (item.tipeItem == TipeItemJatuh.Bahaya)
            {
                if (AudioManager.Instance != null) AudioManager.Instance.MainkanSFX(AudioManager.Instance.sfxError);
                StartCoroutine(ProsesKenaBahaya()); 
            }
            
            Destroy(collision.gameObject);
        }
    }

    private void MunculkanVFX(Vector3 worldPos, string pesan, Color warna)
    {
        if (floatingTextPrefab != null && canvasUIParent != null)
        {
            GameObject vfxObj = Instantiate(floatingTextPrefab, canvasUIParent);
            vfxObj.transform.localScale = Vector3.one;

            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasUIParent.GetComponent<RectTransform>(), 
                screenPos, 
                Camera.main, 
                out Vector2 localPos);

            vfxObj.transform.localPosition = new Vector3(localPos.x, localPos.y, 0f);

            FloatingVFXMiniGame vfx = vfxObj.GetComponent<FloatingVFXMiniGame>();
            if (vfx != null)
            {
                vfx.Setup(pesan, warna);
            }
        }
    }

    private IEnumerator AnimasiMangap()
    {
        if (spriteMakan != null) spriteRenderer.sprite = spriteMakan;
        yield return new WaitForSeconds(0.25f); 
        if (!isDead && spriteIdle != null) spriteRenderer.sprite = spriteIdle;
    }

    private IEnumerator ProsesKenaBahaya()
    {
        isDead = true; 
        
        GetComponent<BoxCollider2D>().enabled = false;

        if (spriteMati != null) spriteRenderer.sprite = spriteMati;
        else if (spriteMakan != null) spriteRenderer.sprite = spriteMakan;

        Vector3 posisiAwal = transform.position;

        Vector3 skalaBayanganAwal = Vector3.one;
        float yLantaiBayangan = 0f;
        if (shadowTransform != null)
        {
            skalaBayanganAwal = shadowTransform.localScale;
            yLantaiBayangan = shadowTransform.position.y; 
        }

        spriteRenderer.color = new Color(1f, 0.2f, 0.2f); 
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white; 

        float waktuJatuh = 0.4f;
        float time = 0;
        
        while(time < waktuJatuh)
        {
            time += Time.deltaTime;
            float progres = time / waktuJatuh;
            
            float tinggiLompatan = Mathf.Sin(progres * Mathf.PI) * 1.2f; 
            transform.position = posisiAwal + new Vector3(0, tinggiLompatan, 0);
            
            float rotasi = Mathf.Lerp(0, -90f, progres);
            transform.rotation = Quaternion.Euler(0, 0, rotasi);

            if (shadowTransform != null)
            {
                shadowTransform.rotation = Quaternion.identity; 
                
                // Geser posisi X bayangan pelan-pelan mengikuti putaran badan
                float geserX = Mathf.Lerp(0, offsetShadowMati.x, progres);
                shadowTransform.position = new Vector3(transform.position.x + geserX, yLantaiBayangan, shadowTransform.position.z);
                
                float scaleMultiplier = 1f - (tinggiLompatan * 0.35f); 
                shadowTransform.localScale = skalaBayanganAwal * scaleMultiplier;
            }
            
            yield return null;
        }

        transform.position = posisiAwal; 
        transform.rotation = Quaternion.Euler(0, 0, -90f);
        
        if (shadowTransform != null)
        {
            shadowTransform.rotation = Quaternion.identity;
            // Pastikan posisi akhir bayangan berada di offset yang sudah diset
            shadowTransform.position = new Vector3(transform.position.x + offsetShadowMati.x, yLantaiBayangan, shadowTransform.position.z);
            shadowTransform.localScale = skalaBayanganAwal;
        }

        spriteRenderer.color = new Color(0.6f, 0.6f, 0.6f); 

        yield return new WaitForSeconds(0.3f); 
        
        gameManager.GameSelesai(); 
    }
}