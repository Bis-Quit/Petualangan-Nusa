using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class HintAnimator : MonoBehaviour
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        // Reset rotasi dan transparansi setiap kali hint muncul
        canvasGroup.alpha = 1f;
        rectTransform.localRotation = Quaternion.identity;
        StartCoroutine(AnimasiPutarJeda());
    }

    private IEnumerator AnimasiPutarJeda()
    {
        while (true)
        {
            // Putar 180 derajat pertama
            yield return StartCoroutine(PutarSebagian(-180f, 0.4f));
            yield return new WaitForSeconds(0.25f); // Jeda sebentar
            
            // Putar 180 derajat sisanya
            yield return StartCoroutine(PutarSebagian(-180f, 0.4f));
            yield return new WaitForSeconds(1.5f); // Jeda lama sebelum animasi ngulang
        }
    }

    private IEnumerator PutarSebagian(float sudutTambahan, float durasi)
    {
        Quaternion rotasiAwal = rectTransform.localRotation;
        Quaternion rotasiTarget = rotasiAwal * Quaternion.Euler(0, 0, sudutTambahan);
        float waktu = 0f;

        while (waktu < durasi)
        {
            waktu += Time.deltaTime;
            float t = waktu / durasi;
            // Rumus Ease-In-Out biar putarannya smooth di awal dan akhir
            float smoothT = t * t * (3f - 2f * t); 
            rectTransform.localRotation = Quaternion.Lerp(rotasiAwal, rotasiTarget, smoothT);
            yield return null;
        }
        rectTransform.localRotation = rotasiTarget;
    }

    public void FadeOutDanMati()
    {
        StopAllCoroutines(); // Hentikan animasi putar
        StartCoroutine(FadeOutRoutine());
    }

    private IEnumerator FadeOutRoutine()
    {
        float waktu = 0;
        float durasi = 0.25f;
        while (waktu < durasi)
        {
            waktu += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, waktu / durasi);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}