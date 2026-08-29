using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class HiddenObjectManager : MonoBehaviour
{
    [Header("Data Level")]
    public static LevelData ActiveLevelData;
    public LevelData currentLevelData;
    
    [Header("Environment Setup")]
    public Transform environmentContainer;

    [Header("Setup UI Siluet (Bottom)")]
    public Transform bottomPanelContainer;
    public GameObject silhouetteSlotPrefab;
    public GameObject secretSlotPrefab;
    
    [Header("Setup UI Secret (Right)")]
    public Transform secretPanelContainer; 

    [Header("UI Koin & Timer")]
    public TextMeshProUGUI coinTextUI;
    public TextMeshProUGUI timerTextUI;

    [Header("Setup UI Pop-up Secret")]
    public GameObject secretPopupPanel;
    public Image popupItemImage;
    public TextMeshProUGUI popupNameText;
    public TextMeshProUGUI popupDescText;
    
    private Sprite pendingSprite;
    private int pendingReward;

    private Dictionary<string, Image> silhouetteDictionary = new Dictionary<string, Image>();

    private int totalItemsToFind;
    private int itemsFoundCounter = 0;
    private int currentCoins = 0; 
    private float currentTime;
    private bool isTimerRunning = false;

    [Header("Pengaturan Penalti")]
    public float penaltyTime = 3f; 
    public int maxMissedClicks = 3; 
    private int currentMissedClicks = 0;

    void Start()
    {
        if (ActiveLevelData != null) currentLevelData = ActiveLevelData; 

        if (currentLevelData == null || currentLevelData.environmentPrefab == null)
        {
            Debug.LogError("ERROR: Level Data atau Environment Prefab kosong!");
            return; 
        }

        if(secretPopupPanel != null) secretPopupPanel.SetActive(false);

        UpdateCoinUI();
        LoadLevel();
    }

    void LoadLevel()
    {
        // --- LOGIKA SPAWN ENVIRONMENT ---
        // Munculkan Prefab ke layar
        GameObject spawnedEnv = Instantiate(currentLevelData.environmentPrefab, environmentContainer);
        
        // Kumpulkan titik koordinat otomatis dari dalam Prefab
        Transform spawnPointsParent = spawnedEnv.transform.Find("SpawnPointHolder");
        List<Transform> availablePoints = new List<Transform>();
        
        if (spawnPointsParent != null)
        {
            foreach (Transform child in spawnPointsParent) availablePoints.Add(child);
        }
        else
        {
            Debug.LogError("Objek 'SpawnPointHolder' tidak ditemukan di dalam Prefab Level!");
        }

        totalItemsToFind = currentLevelData.itemPrefabs.Count;
        itemsFoundCounter = 0; 

        currentTime = currentLevelData.timeLimit;
        isTimerRunning = true;
        UpdateTimerUI();

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

    // --- LOGIKA WAKTU ---
    void Update()
    {
        if (isTimerRunning)
        {
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                currentTime = 0;
                isTimerRunning = false;
                GameOver();
            }
            UpdateTimerUI();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerTextUI != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerTextUI.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void GameOver()
    {
        Debug.Log("Waktu Habis! GAME OVER!");
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

    // --- MESIN PENALTI SPAM CLICK ---
    public void RegisterMissedClick()
    {
        if (!isTimerRunning) return;

        currentMissedClicks++;
        
        if (currentMissedClicks >= maxMissedClicks)
        {
            ApplyPenalty();
            currentMissedClicks = 0; 
        }
    }

    private void ApplyPenalty()
    {
        currentTime -= penaltyTime;
        if (currentTime < 0) currentTime = 0; 
        
        UpdateTimerUI();
        StartCoroutine(TimerWarningAnim());
    }

    private IEnumerator TimerWarningAnim()
    {
        if (timerTextUI != null)
        {
            Color originalColor = timerTextUI.color;
            timerTextUI.color = Color.red;
            
            RectTransform timerRect = timerTextUI.GetComponent<RectTransform>();
            Vector3 originalPos = timerRect.anchoredPosition;
            timerRect.anchoredPosition = originalPos + new Vector3(Random.Range(-5f, 5f), 0, 0);
            
            yield return new WaitForSeconds(0.15f);
            timerRect.anchoredPosition = originalPos - new Vector3(Random.Range(-5f, 5f), 0, 0);
            yield return new WaitForSeconds(0.15f);
            
            timerRect.anchoredPosition = originalPos;
            timerTextUI.color = originalColor;
        }
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

    // --- LOGIKA MUNCULIN POPUP ---
    public void ShowSecretPopup(Sprite img, string name, string desc, int reward)
    {
        isTimerRunning = false; 
        
        pendingSprite = img;
        pendingReward = reward;

        if(popupItemImage != null) popupItemImage.sprite = img;
        if(popupNameText != null) popupNameText.text = name;
        if(popupDescText != null) popupDescText.text = desc;
        
        if(secretPopupPanel != null) secretPopupPanel.SetActive(true);
    }

    // --- LOGIKA TOMBOL AMBIL DIKLIK ---
    public void ClaimSecretItem()
    {
        if(secretPopupPanel != null) secretPopupPanel.SetActive(false);
        
        isTimerRunning = true;
        SecretItemFound(pendingSprite, pendingReward); 
    }

    // --- LOGIKA SECRET ITEM DITEMUKAN ---
    public void SecretItemFound(Sprite secretSprite, int reward)
    {
        GameObject secretUIObj = Instantiate(secretSlotPrefab, secretPanelContainer);
        Image secretImg = secretUIObj.GetComponent<Image>();
        secretImg.sprite = secretSprite;
        secretImg.color = Color.white;

        AddCoins(reward); 
        StartCoroutine(PopAnimation(secretImg.transform)); 
        
        Transform shineObj = secretUIObj.transform.Find("Shine");
        if (shineObj != null) 
        {
            StartCoroutine(ShineSweepRoutine(shineObj.GetComponent<RectTransform>())); 
        }
    }

    // --- MESIN ANIMASI MENGKILAP ---
    private IEnumerator ShineSweepRoutine(RectTransform shineRect)
    {
        while(true)
        {
            shineRect.anchoredPosition = new Vector2(-100f, 0);
            yield return new WaitForSeconds(2.5f);
            
            float time = 0;
            float duration = 0.6f; 
            while(time < duration)
            {
                time += Time.deltaTime;
                float currentX = Mathf.Lerp(-100f, 100f, time / duration);
                shineRect.anchoredPosition = new Vector2(currentX, 0);
                yield return null;
            }
        }
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