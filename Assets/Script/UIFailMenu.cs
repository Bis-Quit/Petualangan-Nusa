using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIFailMenu : MonoBehaviour
{
    [Header("Pengaturan Navigasi")]
    public string namaSceneHome = "scnMap";

    [Header("Target Animasi Utama")]
    [Tooltip("Target gambar yang mau dibikin mumbul (Grup Mahkota/Tombol)")]
    public RectTransform popupVisual; 

    [Header("Referensi Tombol (Biar bisa goyang)")]
    public RectTransform tombolTryAgain;
    public RectTransform tombolMainMenu;

    [Header("Pengaturan Animasi Popup")]
    public float durasiAnimasi = 0.4f;
    public AnimationCurve kurvaBouncy = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private CanvasGroup canvasGroup;
    private bool isButtonClicked = false; // Mencegah klik dobel saat animasi jalan

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        if (popupVisual != null && canvasGroup != null)
        {
            isButtonClicked = false; // Buka kunci tombol setiap kali pop-up muncul
            StartCoroutine(AnimasiKombinasi());
        }
    }

    private IEnumerator AnimasiKombinasi()
    {
        canvasGroup.alpha = 0f;
        popupVisual.localScale = Vector3.zero;
        
        float waktu = 0f;
        while (waktu < durasiAnimasi)
        {
            waktu += Time.unscaledDeltaTime; 
            float persentase = waktu / durasiAnimasi;
            
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, persentase);
            float nilaiSkala = kurvaBouncy.Evaluate(persentase);
            popupVisual.localScale = Vector3.one * nilaiSkala;
            
            yield return null; 
        }

        canvasGroup.alpha = 1f;
        popupVisual.localScale = Vector3.one;
    }

    public void UlangiLevel()
    {
        if (isButtonClicked) return;
        isButtonClicked = true;
        
        StartCoroutine(AnimasiGoyangDanEksekusi(tombolTryAgain, true));
    }

    public void KeMainMenu()
    {
        if (isButtonClicked) return;
        isButtonClicked = true;
        
        StartCoroutine(AnimasiGoyangDanEksekusi(tombolMainMenu, false));
    }

    private IEnumerator AnimasiGoyangDanEksekusi(RectTransform targetTombol, bool isUlangi)
    {
        if (targetTombol != null)
        {
            float waktu = 0f;
            float durasiGoyang = 0.3f;
            Quaternion rotasiAwal = targetTombol.localRotation;

            while (waktu < durasiGoyang)
            {
                waktu += Time.unscaledDeltaTime;
                
                float zRot = Mathf.Sin(waktu * 50f) * 10f; 
                targetTombol.localRotation = Quaternion.Euler(0, 0, zRot);
                
                yield return null;
            }

            targetTombol.localRotation = rotasiAwal; 
        }

        Time.timeScale = 1f; 
        if (isUlangi)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            SceneManager.LoadScene(namaSceneHome);
        }
    }
}