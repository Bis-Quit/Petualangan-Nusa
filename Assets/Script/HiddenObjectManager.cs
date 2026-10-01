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

    [Header("UI Timer & Game Over")]
    public TextMeshProUGUI timerTextUI;
    public GameObject panelFail;

    [Header("Setup UI Pop-up Secret")]
    public GameObject secretPopupPanel;
    [Tooltip("Tarik panel popup secret yang udah dikasih script UIPopupAnimator ke sini")]
    public UIPopupAnimator secretPopupAnimator; 

    public Image popupItemImage;
    public Image popupMaskImage;
    public TextMeshProUGUI popupNameText;
    public TextMeshProUGUI popupDescText;

    [Header("Setup VFX Koin")] 
    public GameObject floatingCoinPrefab;
    public Transform vfxContainer; 
    
    private Sprite pendingSprite;
    private int pendingReward;
    private ItemDataSO pendingDataSO;   

    private Dictionary<string, Image> silhouetteDictionary = new Dictionary<string, Image>();

    private int totalItemsToFind;
    private int itemsFoundCounter = 0;
    private int koinLevelIni = 0;
    public bool isLevelSelesai = false;
    
    private float currentTime;
    private bool isTimerRunning = false;

    [Header("Pengaturan Penalti")]
    public float penaltyTime = 3f; 
    public int maxMissedClicks = 3; 
    private int currentMissedClicks = 0;

    [Header("Pengaturan AFK / Petunjuk")]
    public float waktuBatasAFK = 5f;
    private float timerAFK = 0f;

    // --- VARIABEL BARU: Penyimpan Sementara ---
    private List<ItemDataSO> pusakaDiamankanLevelIni = new List<ItemDataSO>();
    private bool dapatSecretLevelIni = false;

    void Start()
    {
        if (ActiveLevelData != null) currentLevelData = ActiveLevelData; 

        if (currentLevelData == null || currentLevelData.environmentPrefab == null)
        {
            Debug.LogError("ERROR: Level Data atau Environment Prefab kosong!");
            return; 
        }

        if(secretPopupPanel != null) secretPopupPanel.SetActive(false);

        // Reset penyimpan sementara setiap kali main
        pusakaDiamankanLevelIni.Clear();
        dapatSecretLevelIni = false;

        InventoryPemain.pusakaTerkumpul.Clear();

        LoadLevel();
    }

    private void FitToSpawnPoint(GameObject itemObj, Transform point)
    {
        RectTransform itemRect = itemObj.GetComponent<RectTransform>();
        RectTransform pointRect = point.GetComponent<RectTransform>();

        if (itemRect != null)
        {
            itemRect.localScale = Vector3.one;
            itemRect.anchorMin = new Vector2(0.5f, 0.5f);
            itemRect.anchorMax = new Vector2(0.5f, 0.5f);
            itemRect.pivot = new Vector2(0.5f, 0.5f);
            itemRect.anchoredPosition = Vector2.zero;

            if (pointRect != null)
            {
                itemRect.sizeDelta = pointRect.rect.size;
            }

            Image img = itemObj.GetComponent<Image>();
            if (img != null) img.preserveAspect = true;
        }
    }

    void LoadLevel()
    {
        if (AudioManager.Instance != null && currentLevelData != null && currentLevelData.bgmDaerah != null)
        {
            AudioManager.Instance.GantiBGM(currentLevelData.bgmDaerah);
        }

        GameObject spawnedEnv = Instantiate(currentLevelData.environmentPrefab, environmentContainer);
        
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

        foreach (GameObject itemPrefab in currentLevelData.itemPrefabs)
        {
            if (availablePoints.Count == 0) break;

            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform selectedPoint = availablePoints[randomIndex];

            GameObject spawnedItem = Instantiate(itemPrefab, selectedPoint, false);
            FitToSpawnPoint(spawnedItem, selectedPoint); 

            HiddenItem itemScript = spawnedItem.GetComponent<HiddenItem>();
            itemScript.gameManager = this;

            availablePoints.RemoveAt(randomIndex);

            GameObject newSilhouette = Instantiate(silhouetteSlotPrefab, bottomPanelContainer);
            Image silhouetteImage = newSilhouette.GetComponent<Image>();
            silhouetteImage.sprite = itemPrefab.GetComponent<Image>().sprite;
            silhouetteImage.color = new Color(0, 0, 0, 0.8f); 
            
            UISlotPusaka slotScript = newSilhouette.AddComponent<UISlotPusaka>();
            
            if (itemScript != null)
            {
                if (itemScript.dataPusaka != null && !string.IsNullOrEmpty(itemScript.dataPusaka.namaItem))
                {
                    slotScript.namaPusaka = itemScript.dataPusaka.namaItem; 
                }
                else
                {
                    slotScript.namaPusaka = itemScript.itemID; 
                }
            }

            silhouetteDictionary.Add(itemScript.itemID, silhouetteImage);
        }

        if (currentLevelData.secretItemPrefabs != null)
        {
            foreach (GameObject secretPrefab in currentLevelData.secretItemPrefabs)
            {
                if (availablePoints.Count == 0) break;
                
                int randomIndex = Random.Range(0, availablePoints.Count);
                Transform selectedPoint = availablePoints[randomIndex];
                
                GameObject spawnedSecret = Instantiate(secretPrefab, selectedPoint, false);
                FitToSpawnPoint(spawnedSecret, selectedPoint);

                SecretItem secretScript = spawnedSecret.GetComponent<SecretItem>();
                if(secretScript != null) secretScript.gameManager = this;

                availablePoints.RemoveAt(randomIndex);
            }
        }

        foreach (GameObject decoyPrefab in currentLevelData.decoyPrefabs)
        {
            if (availablePoints.Count == 0) break;
            
            int randomIndex = Random.Range(0, availablePoints.Count);
            Transform selectedPoint = availablePoints[randomIndex];
            
            GameObject spawnedDecoy = Instantiate(decoyPrefab, selectedPoint, false);
            FitToSpawnPoint(spawnedDecoy, selectedPoint);

            DecoyItem decoyScript = spawnedDecoy.GetComponent<DecoyItem>();
            if(decoyScript != null) decoyScript.gameManager = this;

            availablePoints.RemoveAt(randomIndex);
        }
    }

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

            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                timerAFK = 0f; 
            }
            else
            {
                timerAFK += Time.deltaTime;
                if (timerAFK >= waktuBatasAFK)
                {
                    BerikanPetunjukAFK();
                    timerAFK = 0f; 
                }
            }
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
        Time.timeScale = 0f; 
        if (panelFail != null) panelFail.SetActive(true);

        if (AudioManager.Instance != null && AudioManager.Instance.sfxKalah != null)
        {
            AudioManager.Instance.MainkanSFX(AudioManager.Instance.sfxKalah);
        }
    }

    public void AddCoins(int amount)
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AddCoins(amount);
            koinLevelIni += amount; 
        }
    }

    public void RegisterMissedClick()
    {
        if (!isTimerRunning || isLevelSelesai) return;

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

    public void SpawnFloatingCoin(int amount, Vector3 spawnPosition)
    {
        if (floatingCoinPrefab == null || vfxContainer == null) return;
        
        GameObject vfx = Instantiate(floatingCoinPrefab, vfxContainer);
        vfx.transform.position = spawnPosition; 
        
        FloatingCoinVFX vfxScript = vfx.GetComponent<FloatingCoinVFX>();
        if (vfxScript != null)
        {
            vfxScript.Setup(amount);
        }
    }

    public void ItemFound(string id, Sprite coloredSprite, ItemDataSO dataSO, Vector3 posisiBarang)
    {
        if (isLevelSelesai) return;

        if (silhouetteDictionary.ContainsKey(id))
        {
            if (dataSO != null)
            {
                // LOGIKA BARU: Simpan sementara, jangan langsung masuk PlayerPrefs
                pusakaDiamankanLevelIni.Add(dataSO);
            }

            if (AudioManager.Instance != null && AudioManager.Instance.sfxDapatBarang != null)
            {
                AudioManager.Instance.MainkanSFX(AudioManager.Instance.sfxDapatBarang);
            }

            StartCoroutine(UpdateSilhouetteAnim(silhouetteDictionary[id], coloredSprite));
            AddCoins(3);
            SpawnFloatingCoin(3, posisiBarang);

            itemsFoundCounter++;
            CheckWinCondition();
        }
    }

    public void ShowSecretPopup(Sprite img, string name, string desc, int reward, ItemDataSO dataSO)
    {
        if (isLevelSelesai) return;
        
        isTimerRunning = false; 

        // --- PERBAIKAN DI SINI ---
        if (AudioManager.Instance != null && AudioManager.Instance.sfxPopupSecret != null)
        {
            AudioManager.Instance.MainkanSFX(AudioManager.Instance.sfxPopupSecret);
        }
        else if (AudioManager.Instance != null && AudioManager.Instance.sfxDapatBarang != null)
        {
            // Fallback (cadangan) kalau slot sfxPopupSecret lupa diisi
            AudioManager.Instance.MainkanSFX(AudioManager.Instance.sfxDapatBarang);
        }
        // -------------------------
        
        pendingSprite = img;
        pendingReward = reward;
        pendingDataSO = dataSO; 

        if(popupItemImage != null) popupItemImage.sprite = img;
        if(popupMaskImage != null) popupMaskImage.sprite = img; 
        if(popupNameText != null) popupNameText.text = name;
        if(popupDescText != null) popupDescText.text = desc;
        
        if(secretPopupPanel != null) 
        {
            secretPopupPanel.SetActive(true);
            if(secretPopupAnimator != null) secretPopupAnimator.MulaiAnimasi();
        }
    }

    public void ClaimSecretItem()
    {
        if(secretPopupPanel != null) secretPopupPanel.SetActive(false);
        isTimerRunning = true;
        SecretItemFound(pendingSprite, pendingReward, pendingDataSO); 
    }

    public void SecretItemFound(Sprite secretSprite, int reward, ItemDataSO dataSO)
    {
        GameObject secretUIObj = Instantiate(secretSlotPrefab, secretPanelContainer);
        Image secretImg = secretUIObj.GetComponent<Image>();
        secretImg.sprite = secretSprite;
        secretImg.color = Color.white;

        if (dataSO != null)
        {
            // LOGIKA BARU: Simpan sementara dan tandai bahwa secret sudah diambil
            pusakaDiamankanLevelIni.Add(dataSO);
            dapatSecretLevelIni = true;
        }

        AddCoins(reward); 
        SpawnFloatingCoin(reward, secretUIObj.transform.position);

        StartCoroutine(PopAnimation(secretImg.transform)); 
        
        Transform shineObj = secretUIObj.transform.Find("Shine");
        if (shineObj != null) 
        {
            StartCoroutine(ShineSweepRoutine(shineObj.GetComponent<RectTransform>())); 
        }
    }

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

    private void CheckWinCondition()
    {
        if (itemsFoundCounter >= totalItemsToFind && !isLevelSelesai)
        {
            isLevelSelesai = true; 
            isTimerRunning = false; 
            
            if (CoinManager.Instance != null)
            {
                CoinManager.Instance.koinLevelTerakhir = koinLevelIni;
            }

            // --- EKSEKUSI SIMPAN PERMANEN (HANYA SAAT MENANG) ---
            foreach (ItemDataSO data in pusakaDiamankanLevelIni)
            {
                InventoryPemain.pusakaTerkumpul.Add(data);
                PlayerPrefs.SetInt("Koleksi_" + data.namaItem, 1);
            }

            // Kalau ada secret yang diambil di level ini, nyalakan notif Jurnal
            if (dapatSecretLevelIni)
            {
                PlayerPrefs.SetInt("AdaPusakaBaru", 1);
            }
            
            PlayerPrefs.Save();
            // ---------------------------------------------------

            StartCoroutine(MunculkanMaskotSmooth());
        }
    }

    private IEnumerator MunculkanMaskotSmooth()
    {
        yield return new WaitForSeconds(1.5f);
        if (MascotPopupManager.Instance != null)
        {
            MascotPopupManager.Instance.ShowWinPopup();
        }
    }

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

    private void BerikanPetunjukAFK()
    {
        HiddenItem[] barangSisa = FindObjectsOfType<HiddenItem>();
        
        if (barangSisa.Length > 0)
        {
            int acak = Random.Range(0, barangSisa.Length);
            StartCoroutine(AnimasiPetunjukDenyut(barangSisa[acak].transform));
        }
    }

    private IEnumerator AnimasiPetunjukDenyut(Transform t)
    {
        if (t == null) yield break;
        
        Vector3 skalaAwal = t.localScale;
        float elapsed = 0f;
        float durasi = 1.2f; 

        while (elapsed < durasi)
        {
            if (t == null) yield break; 

            elapsed += Time.deltaTime;
            
            float persentaseWaktu = elapsed / durasi;
            float kurvaSmooth = Mathf.Pow(Mathf.Sin(persentaseWaktu * Mathf.PI * 2f), 2);
            
            float efekSkala = 1f + (kurvaSmooth * 0.2f); 
            t.localScale = skalaAwal * efekSkala;
            
            yield return null;
        }
        
        if (t != null) t.localScale = skalaAwal;
    }
}