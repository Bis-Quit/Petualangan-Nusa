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
    public Image backgroundImage;
    public List<Transform> spawnPoints;

    [Header("Setup UI Siluet (Bottom)")]
    public Transform bottomPanelContainer;
    public GameObject silhouetteSlotPrefab;
    public GameObject secretSlotPrefab;
    
    [Header("Setup UI Secret (Right)")]
    public Transform secretPanelContainer; 

    [Header("UI Koin")]
    public TextMeshProUGUI coinTextUI;

    [Header("UI Timer")]
    public TextMeshProUGUI timerTextUI;

    private Dictionary<string, Image> silhouetteDictionary = new Dictionary<string, Image>();

    private int totalItemsToFind;
    private int itemsFoundCounter = 0;
    private int currentCoins = 0; 

    private float currentTime;
    private bool isTimerRunning = false;

    [Header("Pengaturan Penalti")]
    public float penaltyTime = 3f; // Waktu yang dikurangi (detik)
    public int maxMissedClicks = 3; // Batas salah klik sebelum dihukum
    private int currentMissedClicks = 0;

    void Start()
    {
        // Validasi dan assign data dari map jika tersedia
        if (ActiveLevelData != null)
        {
            currentLevelData = ActiveLevelData; 
        }

        if (currentLevelData == null)
        {
            Debug.LogError("ERROR: Level Data kosong! Silakan mainkan dari scene Map, atau isi slot 'Current Level Data' di Inspector untuk testing.");
            return; // Hentikan proses agar tidak error beruntun
        }

        UpdateCoinUI();
        LoadLevel();
    }

    void LoadLevel()
    {
        backgroundImage.sprite = currentLevelData.backgroundSprite;
        List<Transform> availablePoints = new List<Transform>(spawnPoints);

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

    // --- MESIN PENALTI SPAM CLICK ---
    public void RegisterMissedClick()
    {
        if (!isTimerRunning) return;

        currentMissedClicks++;
        
        if (currentMissedClicks >= maxMissedClicks)
        {
            ApplyPenalty();
            currentMissedClicks = 0; // Reset hitungan setelah kena hukum
        }
    }

    private void ApplyPenalty()
    {
        currentTime -= penaltyTime;
        if (currentTime < 0) currentTime = 0; // Biar waktu ga minus
        
        UpdateTimerUI();
        StartCoroutine(TimerWarningAnim());
    }

    private IEnumerator TimerWarningAnim()
    {
        // Ubah warna teks jadi merah sejenak sebagai feedback visual
        if (timerTextUI != null)
        {
            Color originalColor = timerTextUI.color;
            timerTextUI.color = Color.red;
            
            // Goyangkan teks sedikit (opsional, biar kerasa juice-nya)
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
        GameObject secretUIObj = Instantiate(secretSlotPrefab, secretPanelContainer);
        Image secretImg = secretUIObj.GetComponent<Image>();
        secretImg.sprite = secretSprite;
        secretImg.color = Color.white;

        AddCoins(reward); 
        StartCoroutine(PopAnimation(secretImg.transform)); 
        
        // Cari objek cahaya dan jalankan efek mengkilap
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
            // 1. Taruh cahaya di luar kiri siluet
            shineRect.anchoredPosition = new Vector2(-100f, 0);
            
            // 2. Jeda diam (misal 2.5 detik sekali kilap)
            yield return new WaitForSeconds(2.5f);
            
            // 3. Cahaya meluncur ke kanan dengan cepat
            float time = 0;
            float duration = 0.6f; // Kecepatan kilap
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