using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class DecoyItem : MonoBehaviour
{
    [HideInInspector] public HiddenObjectManager gameManager;
    private bool isShaking = false;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnDecoyClicked);
    }

    public void OnDecoyClicked()
    {
        if (gameManager != null && gameManager.isLevelSelesai) return;

        // --- TAMBAHAN AUDIO SALAH KLIK (DECOY) ---
        if (AudioManager.Instance != null && AudioManager.Instance.sfxError != null)
        {
            AudioManager.Instance.MainkanSFX(AudioManager.Instance.sfxError);
        }

        if (gameManager != null)
        {
            gameManager.RegisterMissedClick();
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
        while (time < 0.3f)
        {
            time += Time.deltaTime;
            float zRotation = Mathf.Sin(time * 40f) * 15f;
            t.localRotation = Quaternion.Euler(0, 0, zRotation);
            yield return null;
        }

        t.localRotation = originalRotation;
        isShaking = false;
    }
}