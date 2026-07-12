using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IslandPopupManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject popupOverlay;
    private GameObject currentPopup;

    void Start()
    {
        if (popupOverlay != null)
        {
            popupOverlay.SetActive(false);
        }
    }

    public void OpenPopup(GameObject islandPopupToOpen)
    {
        popupOverlay.SetActive(true);
        currentPopup = islandPopupToOpen;
        currentPopup.SetActive(true);
        currentPopup.transform.localScale = Vector3.zero;
        StartCoroutine(BouncyScale(currentPopup.transform, 1.1f, 1f, 0.3f));
    }

    public void ClosePopup()
    {
        popupOverlay.SetActive(false);

        if (currentPopup != null)
        {
            currentPopup.SetActive(false);
            currentPopup = null;
        }
    }

    private IEnumerator BouncyScale(Transform target, float overshoot, float finalScale, float duration)
    {
        float time = 0f;
        float halfDuration = duration / 2f;

        while(time < halfDuration)
        {
            time += Time.deltaTime;
            float t = time /halfDuration;
            target.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * overshoot, t);
            yield return null;
        }

        time = 0f;
        while(time < halfDuration)
        {
            time += Time.deltaTime;
            float t = time / halfDuration;
            target.localScale = Vector3.Lerp(Vector3.one * overshoot, Vector3.one * finalScale, t);
            yield return null; 
        }

        target.localScale = Vector3.one * finalScale;
    }
}
