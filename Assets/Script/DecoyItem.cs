using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class DecoyItem : MonoBehaviour
{
    private bool isShaking = false;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnDecoyClicked);
    }

    public void OnDecoyClicked()
    {
        if (!isShaking)
        {
            StartCoroutine(ShakeAnimation());
        }
    }

    private IEnumerator ShakeAnimation()
    {
        isShaking = true;
        Transform t = transform;
        Quaternion originalRotation = t.localRotation;

        float time = 0;
        while (time < 0.3)
        {
            time += Time.deltaTime;
            float zRotation = Mathf.Sin(time * 40f) * 15f;
            t.localRotation = Quaternion.Euler(0,0, zRotation);
            yield return null;
        }

        t.localRotation = originalRotation;
        isShaking = false;
    }
}
