using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIPopupAnimator : MonoBehaviour
{
    [Header("Target Animasi")]
    [Tooltip("Masukkan gambar bodi/background utama popup ke sini")]
    public RectTransform popupBodi;

    [Header("Pengaturan Animasi")]
    public float durasiMuncul = 0.4f;
    public AnimationCurve kurvaMuncul = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private CanvasGroup canvasGroup;
    private Vector3 skalaAsli;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (popupBodi != null)
        {
            // Rekam skala asli dari Editor
            skalaAsli = popupBodi.localScale;
        }
    }

    private void OnEnable()
    {
        // Jaga-jaga kalau diaktifkan manual tanpa Manager
        MulaiAnimasi();
    }

    // Fungsi Publik yang dipanggil paksa oleh Manager
    public void MulaiAnimasi()
    {
        if (popupBodi != null && canvasGroup != null)
        {
            // Hentikan animasi sebelumnya (kalau ada) biar nggak numpuk
            StopAllCoroutines();
            StartCoroutine(AnimasiMunculMulus());
        }
    }

    private IEnumerator AnimasiMunculMulus()
    {
        canvasGroup.alpha = 0f;
        popupBodi.localScale = Vector3.zero;

        float waktu = 0f;
        while (waktu < durasiMuncul)
        {
            waktu += Time.unscaledDeltaTime;
            float persen = waktu / durasiMuncul;

            canvasGroup.alpha = Mathf.Lerp(0f, 1f, persen);
            popupBodi.localScale = skalaAsli * kurvaMuncul.Evaluate(persen);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        popupBodi.localScale = skalaAsli;
    }
}