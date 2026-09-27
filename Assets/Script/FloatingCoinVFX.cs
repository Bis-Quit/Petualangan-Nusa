using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class FloatingCoinVFX : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float fadeDuration = 1f;
    
    private CanvasGroup canvasGroup;
    private TextMeshProUGUI textMesh;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    public void Setup(int coinAmount)
    {
        if (textMesh != null)
        {
            textMesh.text = "+" + coinAmount.ToString();
            textMesh.color = new Color(1f, 0.8f, 0f, 1f); 
        }
        StartCoroutine(AnimateAndDestroy());
    }

    private IEnumerator AnimateAndDestroy()
    {
        float time = 0f;
        Vector3 startPos = transform.position;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            
            yield return null;
        }

        Destroy(gameObject);
    }
}