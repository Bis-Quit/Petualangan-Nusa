using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class FloatingVFXMiniGame : MonoBehaviour
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

    public void Setup(string teksPesan, Color warnaTeks)
    {
        if (textMesh != null)
        {
            textMesh.text = teksPesan;
            textMesh.color = warnaTeks; 
        }
        StartCoroutine(AnimateAndDestroy());
    }

    private IEnumerator AnimateAndDestroy()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            
            // PAKE LOCAL POSITION BIAR GERAKNYA NORMAL DI UI
            transform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;
            
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            
            yield return null;
        }

        Destroy(gameObject);
    }
}