using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class HiddenObjectManager : MonoBehaviour
{
    [Header("Data Level")]
    public LevelData currentLevelData;
    public Image backgroundImage;
    public List<Transform> spawnPoints;

    [Header("Setup UI Siluet")]
    public Transform bottomPanelContainer;
    public GameObject silhouetteSlotPrefab;

    private Dictionary<string, Image> silhouetteDictionary = new Dictionary<string, Image>();

    void Start()
    {
        LoadLevel();
    }

    void LoadLevel()
    {
        backgroundImage.sprite = currentLevelData.backgroundSprite;
        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        foreach (GameObject itemPrefab in currentLevelData.itemPrefabs)
        {
            if (availablePoints.Count == 0) break;

            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform selectedPoint = availablePoints[randomIndex];

            GameObject spawnedItem = Instantiate(itemPrefab, selectedPoint.position, Quaternion.identity, selectedPoint);
            HiddenItem itemScript = spawnedItem.GetComponent<HiddenItem>();
            itemScript.gameManager = this;

            availablePoints.RemoveAt(randomIndex);

            // Cetak UI Siluet ke panel bawah
            GameObject newSilhouette = Instantiate(silhouetteSlotPrefab, bottomPanelContainer);
            Image silhouetteImage = newSilhouette.GetComponent<Image>();
            silhouetteImage.sprite = itemPrefab.GetComponent<Image>().sprite;
            silhouetteImage.color = new Color(0, 0, 0, 0.8f);
            
            silhouetteDictionary.Add(itemScript.itemID, silhouetteImage);
        }

        foreach (GameObject decoyPrefab in currentLevelData.decoyPrefabs)
        {
            if (availablePoints.Count == 0) break;
            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform selectedPoint = availablePoints[randomIndex];
            Instantiate(decoyPrefab, selectedPoint.position, Quaternion.identity, selectedPoint);
            availablePoints.RemoveAt(randomIndex);
        }
    }

    public void ItemFound(string id, Sprite coloredSprite)
    {
        if (silhouetteDictionary.ContainsKey(id))
        {
            StartCoroutine(UpdateSilhouetteAnim(silhouetteDictionary[id], coloredSprite));
        }
    }

    private IEnumerator UpdateSilhouetteAnim(Image silhouetteImg, Sprite coloredSprite)
    {
        Transform t = silhouetteImg.transform;
        silhouetteImg.color = Color.white; 
        silhouetteImg.sprite = coloredSprite;
        
        float time = 0;
        while(time < 0.15f)
        {
            time += Time.deltaTime;
            t.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.4f, time / 0.15f);
            yield return null;
        }
        
        time = 0;
        while(time < 0.15f)
        {
            time += Time.deltaTime;
            t.localScale = Vector3.Lerp(Vector3.one * 1.4f, Vector3.one, time / 0.15f);
            yield return null;
        }
    }
}