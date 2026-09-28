using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
public class BouncyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Animation Settings")]
    [Tooltip("Scale multiplier when the button is pressed (e.g., 0.85 = 85% of original size)")]
    public float pressedScale = 0.85f;
    public float pressDuration = 0.1f;
    public float releaseDuration = 0.15f;

    private Vector3 initialScale;
    private Coroutine activeAnimation;

    private void Awake()
    {
        initialScale = transform.localScale;
    }

    private void OnEnable()
    {
        transform.localScale = initialScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // --- TAMBAHAN AUDIO KLIK TOMBOL ---
        if (AudioManager.Instance != null && AudioManager.Instance.sfxKlikTombol != null)
        {
            AudioManager.Instance.MainkanSFX(AudioManager.Instance.sfxKlikTombol);
        }

        if (activeAnimation != null) StopCoroutine(activeAnimation);
        activeAnimation = StartCoroutine(ScaleAnimation(initialScale * pressedScale, pressDuration));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (activeAnimation != null) StopCoroutine(activeAnimation);
        activeAnimation = StartCoroutine(ScaleAnimation(initialScale, releaseDuration));
    }

    private IEnumerator ScaleAnimation(Vector3 targetScale, float duration)
    {
        float elapsedTime = 0f;
        Vector3 currentScale = transform.localScale;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            transform.localScale = Vector3.Lerp(currentScale, targetScale, elapsedTime / duration);
            yield return null;
        }
        
        transform.localScale = targetScale;
    }
}