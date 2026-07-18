using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class HiddenObjectManager : MonoBehaviour
{
    [Header("Data Level")]
    public LevelData currentLevelData;
    public Image backgroundImage;
    public List<Transform> spawnPoints;

    [Header("Setup UI Siluet (Bottom)")]
    public Transform bottomPanelContainer;
    public GameObject silhouetteSlotPrefab;
    
    [Header("Setup UI Secret (Right)")]
    public Transform secretPanelContainer; 

    [Header("UI Koin")]
    public TextMeshProUGUI coinTextUI; 

    private Dictionary<string, Image> silhouetteDictionary = new Dictionary<string, Image>();

    private int totalItemsToFind;
    private int itemsFoundCounter = 0;
    private int currentCoins = 0; 

    void Start()
    {
        UpdateCoinUI();
        LoadLevel();
    }

    void LoadLevel()
    {
        backgroundImage.sprite = currentLevelData.backgroundSprite;
        List<Transform> availablePoints = new List<Transform>(spawnPoints);

        totalItemsToFind = currentLevelData.itemPrefabs.Count;
        itemsFoundCounter = 0; 

        // SEBAR BARANG UTAMA
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

        // SEBAR SECRET ITEM
        if (currentLevelData.secretItemPrefabs != null)
        {
            foreach (GameObject secretPrefab in currentLevelData.secretItemPrefabs)
            {
                if (availablePoints.Count == 0) break;
                
                int randomIndex = Random.Range(0, availablePoints.Count);
                Transform selectedPoint = availablePoints[randomIndex];
                
                GameObject spawnedSecret = Instantiate(secretPrefab, selectedPoint.position, Quaternion.identity, selectedPoint);
                SecretItem secretScript = spawnedSecret.GetComponent<SecretItem>();
                if(secretScript != null) secretScript.gameManager = this;

                availablePoints.RemoveAt(randomIndex);
            }
        }

        // SEBAR BARANG PENGECOH
        foreach (GameObject decoyPrefab in currentLevelData.decoyPrefabs)
        {
            if (availablePoints.Count == 0) break;
            
            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform selectedPoint = availablePoints[randomIndex];
            
            GameObject spawnedDecoy = Instantiate(decoyPrefab, selectedPoint.position, Quaternion.identity, selectedPoint);
            DecoyItem decoyScript = spawnedDecoy.GetComponent<DecoyItem>();
            if(decoyScript != null) decoyScript.gameManager = this;

            availablePoints.RemoveAt(randomIndex);
        }
    }

    // --- MESIN KASIR KOIN ---
    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        if (coinTextUI != null) coinTextUI.text = currentCoins.ToString();
    }

    // --- LOGIKA BARANG UTAMA DITEMUKAN ---
    public void ItemFound(string id, Sprite coloredSprite)
    {
        if (silhouetteDictionary.ContainsKey(id))
        {
            StartCoroutine(UpdateSilhouetteAnim(silhouetteDictionary[id], coloredSprite));
            AddCoins(3);
            itemsFoundCounter++;
            CheckWinCondition();
        }
    }

    // --- LOGIKA SECRET ITEM DITEMUKAN ---
    public void SecretItemFound(Sprite secretSprite, int reward)
    {
        GameObject secretUIObj = Instantiate(silhouetteSlotPrefab, secretPanelContainer);
        Image secretImg = secretUIObj.GetComponent<Image>();
        secretImg.sprite = secretSprite;
        secretImg.color = Color.white;

        AddCoins(reward); 
        StartCoroutine(PopAnimation(secretImg.transform)); 
    }

    // --- KONDISI MENANG ---
    private void CheckWinCondition()
    {
        if (itemsFoundCounter >= totalItemsToFind)
        {
            Debug.Log("LEVEL SELESAI!");
        }
    }

    // --- ANIMASI ---
    private IEnumerator PopAnimation(Transform t)
    {
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

    private IEnumerator UpdateSilhouetteAnim(Image silhouetteImg, Sprite coloredSprite)
    {
        silhouetteImg.color = Color.white; 
        silhouetteImg.sprite = coloredSprite;
        yield return StartCoroutine(PopAnimation(silhouetteImg.transform));
    }
}