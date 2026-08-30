using UnityEngine;
using System.Collections;

public class UIShineEffect : MonoBehaviour
{
    private RectTransform shineRect;
    private Coroutine shineCoroutine;
    
    [Header("Pengaturan Animasi")]
    public float delayAntarKilap = 2.5f;
    public float kecepatanKilap = 0.6f;
    
    [Header("Batas Kordinat X")]
    public float startX = -200f;
    public float endX = 200f;

    private void Awake()
    {
        shineRect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (shineCoroutine != null) StopCoroutine(shineCoroutine);
        shineCoroutine = StartCoroutine(ShineRoutine());
    }

    private void OnDisable()
    {
        if (shineCoroutine != null) StopCoroutine(shineCoroutine);
    }

    private IEnumerator ShineRoutine()
    {
        while (true)
        {
            shineRect.anchoredPosition = new Vector2(startX, 0);
            yield return new WaitForSeconds(delayAntarKilap);

            float time = 0;
            while (time < kecepatanKilap)
            {
                time += Time.deltaTime;
                float currentX = Mathf.Lerp(startX, endX, time / kecepatanKilap);
                shineRect.anchoredPosition = new Vector2(currentX, 0);
                yield return null;
            }
        }
    }
}