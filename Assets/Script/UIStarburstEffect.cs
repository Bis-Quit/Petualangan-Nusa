using UnityEngine;
using System.Collections;

public class UIStarburstEffect : MonoBehaviour
{
    [Header("Pengaturan Durasi Ledakan")]
    [Tooltip("Waktu yang dibutuhkan cahaya untuk menutupi layar")]
    [SerializeField] private float duration = 1.5f; 
    
    [Header("Pengaturan Transisi")]
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    [Header("Batas Skala")]
    [SerializeField] private Vector3 initialScale = Vector3.zero; 
    
    // Default dibikin sangat besar (25x) biar pasti tembus melebihi ukuran layar
    [SerializeField] private Vector3 targetScale = new Vector3(25f, 25f, 1f);   

    private void OnEnable()
    {
        // Langsung eksekusi ledakan cahaya tepat saat koper tertutup
        StopAllCoroutines();
        StartCoroutine(PlayMassiveScale());
    }

    private IEnumerator PlayMassiveScale()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            // Transisi dari kecil ke super besar dengan smooth
            float curveValue = scaleCurve.Evaluate(elapsedTime / duration);
            transform.localScale = Vector3.LerpUnclamped(initialScale, targetScale, curveValue);
            
            yield return null;
        }

        // Kunci di ukuran masif
        transform.localScale = targetScale;
    }
}