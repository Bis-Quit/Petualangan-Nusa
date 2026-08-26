using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class DecoyItem : MonoBehaviour
{
    [HideInInspector] public HiddenObjectManager gameManager;
    private bool isShaking = false;
    private bool hasYieldCoin = false;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnDecoyClicked);
    }

    public void OnDecoyClicked()
    {
        if (gameManager != null)
        {
            gameManager.RegisterMissedClick();

            if (!hasYieldCoin)
            {
                gameManager.AddCoins(1);
                hasYieldCoin = true;
            }
        }

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
