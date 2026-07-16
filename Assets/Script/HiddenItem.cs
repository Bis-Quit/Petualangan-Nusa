using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Button))]
public class HiddenItem : MonoBehaviour
{
    [Header("Unique ID")]
    public string itemID;

    [HideInInspector] public HiddenObjectManager gameManager;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnItemFound);
    }

    public void OnItemFound()
    {
        GetComponent<Button>().interactable = false;
        Sprite mySprite = GetComponent<Image>().sprite;
        gameManager.ItemFound(itemID, mySprite);

        StartCoroutine(PopAndDissappear());
    }

    private IEnumerator PopAndDissappear()
    {
        Transform t = transform;
        float time = 0;
        while(time < 0.15f)
        {
            time += Time.deltaTime;
            t.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.2f, time / 0.15f);
            yield return null;
        }
        time = 0;
        while(time < 0.2f)
        {
            time += Time.deltaTime;
            t.localScale = Vector3.Lerp(Vector3.one * 1.2f, Vector3.zero, time /0.2f);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
