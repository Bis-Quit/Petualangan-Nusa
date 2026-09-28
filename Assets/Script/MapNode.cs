using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class MapNode : MonoBehaviour
{
    public enum NodeState { Locked, Unlocked, Completed }

    [Header("Status Node")]
    public NodeState currentState = NodeState.Locked;
    public string nodeID;

    [Header("Data Level")]
    public LevelData levelData; 

    [Header("Navigation Settings")]
    public MapNode nextNode;

    [Header("UI Pulau (Warna)")]
    public Button nodeButton;
    public Image nodeImage;
    public Color lockedColor = new Color(0.5f, 0.5f, 0.5f);
    public Color unlockedColor = Color.white;
    public Color completedColor = Color.white;

    [Header("Visual Pin (Sistem 3 Fase)")]
    public GameObject lockedPinObj;      
    public GameObject activePinObj;      
    public GameObject completedPinObj;   

    [Header("Animasi Pin Aktif (Ngambang)")]
    public bool useFloatingAnimation = true;
    public float floatSpeed = 3f;
    public float floatAmount = 10f;
    private Vector3 activePinStartPos;
    private Coroutine floatCoroutine;

    [Header("Animasi Pin Selesai (Denyut)")]
    public bool useCompletedAnimation = true;
    public float pulseSpeed = 2f;
    public float pulseAmount = 0.05f;
    private Vector3 completedPinStartScale;
    private Coroutine completedCoroutine;

    [Header("Click Event")]
    public UnityEvent onNodeActiveClicked;

    void Start()
    {
        LoadNodeState();
        
        if (activePinObj != null)
        {
            activePinStartPos = activePinObj.transform.localPosition;
        }

        if (completedPinObj != null)
        {
            completedPinStartScale = completedPinObj.transform.localScale;
        }

        UpdateNodeVisuals();
    }

    public void UpdateNodeVisuals()
    {
        if (lockedPinObj != null) lockedPinObj.SetActive(false);
        if (activePinObj != null) activePinObj.SetActive(false);
        if (completedPinObj != null) completedPinObj.SetActive(false);
        
        if (floatCoroutine != null) StopCoroutine(floatCoroutine);
        if (completedCoroutine != null) StopCoroutine(completedCoroutine);

        switch (currentState)
        {
            case NodeState.Locked:
                if (nodeButton != null) nodeButton.interactable = false;
                if (nodeImage != null) nodeImage.color = lockedColor;
                if (lockedPinObj != null) lockedPinObj.SetActive(true);
                break;
            
            case NodeState.Unlocked:
                if (nodeButton != null) nodeButton.interactable = true;
                if (nodeImage != null) nodeImage.color = unlockedColor;
                if (activePinObj != null) 
                {
                    activePinObj.SetActive(true);
                    if (useFloatingAnimation) floatCoroutine = StartCoroutine(FloatingAnimation());
                }
                break;

            case NodeState.Completed:
                if (nodeButton != null) nodeButton.interactable = true;
                if (nodeImage != null) nodeImage.color = completedColor; 
                
                if (completedPinObj != null) 
                {
                    completedPinObj.SetActive(true);
                    if (useCompletedAnimation) completedCoroutine = StartCoroutine(CompletedAnimation());
                }
                break;
        }
    }

    public void OnNodeClicked()
    {
        if (currentState != NodeState.Locked)
        {
            if (AudioManager.Instance != null && AudioManager.Instance.sfxPilihPulau != null)
            {
                AudioManager.Instance.MainkanSFX(AudioManager.Instance.sfxPilihPulau);
            }
            
            Debug.Log("Node clicked: " + nodeID);
            onNodeActiveClicked?.Invoke();

            if (MascotPopupManager.Instance != null && levelData != null)
            {
                MascotPopupManager.Instance.ShowMascotPopup(levelData);
            }
            else if (levelData == null)
            {
                Debug.LogError("BRO! LevelData di MapNode " + gameObject.name + " belum diisi!");
            }
        }
    }

    public void CompleteThisNode()
    {
        StartCoroutine(AnimasiSelebrasiKomplit());
    }

    // --- ANIMASI 1: SWELL & WIGGLE UNTUK LEVEL SELESAI ---
    private IEnumerator AnimasiSelebrasiKomplit()
    {
        currentState = NodeState.Completed;
        SaveNodeState(2);
        
        if (lockedPinObj != null) lockedPinObj.SetActive(false);
        if (activePinObj != null) activePinObj.SetActive(false);
        
        if (nodeImage != null) nodeImage.color = completedColor;

        if (completedPinObj != null)
        {
            completedPinObj.SetActive(true);
            
            float waktu = 0f;
            float durasi = 0.6f; // Lama animasi goyang kegirangan
            
            while (waktu < durasi)
            {
                waktu += Time.deltaTime;
                float persen = waktu / durasi;
                
                // Efek membesar membulat
                float scaleT = Mathf.Sin(persen * Mathf.PI); 
                completedPinObj.transform.localScale = completedPinStartScale + (Vector3.one * scaleT * 0.7f);
                
                // Efek goyang rotasi kiri-kanan
                float sudutWiggle = Mathf.Sin(persen * Mathf.PI * 5f) * 20f; 
                completedPinObj.transform.localRotation = Quaternion.Euler(0, 0, sudutWiggle);
                
                yield return null;
            }
            
            completedPinObj.transform.localScale = completedPinStartScale;
            completedPinObj.transform.localRotation = Quaternion.identity;

            if (useCompletedAnimation) completedCoroutine = StartCoroutine(CompletedAnimation());
        }

        // Jeda dramatis sebelum pulau selanjutnya terbuka
        yield return new WaitForSeconds(0.8f);

        if (nextNode != null)
        {
            nextNode.UnlockThisNode();
        }
    }

    public void UnlockThisNode()
    {
        if (currentState == NodeState.Locked)
        {
            currentState = NodeState.Unlocked;
            SaveNodeState(1);
            
            if (lockedPinObj != null) lockedPinObj.SetActive(false);
            if (completedPinObj != null) completedPinObj.SetActive(false);
            
            if (nodeButton != null) nodeButton.interactable = true;
            
            // Catatan: nodeImage.color = unlockedColor; DIHAPUS dari sini 
            // agar warnanya tidak ganti secara instan, melainkan lewat animasi di bawah

            if (activePinObj != null) 
            {
                activePinObj.SetActive(true);
            }

            StartCoroutine(AnimasiPopUnlock());
        }
    }

    // --- ANIMASI 2: BOUNCY POP UNTUK LEVEL BARU ---
    private IEnumerator AnimasiPopUnlock()
    {
        if (floatCoroutine != null) StopCoroutine(floatCoroutine);
        
        float waktu = 0f;
        float durasi = 0.6f; // Durasi pas agar transisi warnanya terasa mulus
        Vector3 skalaPinTarget = Vector3.one; 
        Vector3 skalaPulauAwal = nodeImage != null ? nodeImage.transform.localScale : Vector3.one;
        
        while (waktu < durasi)
        {
            waktu += Time.deltaTime;
            float persen = waktu / durasi;
            
            // 1. Animasi Pin Koper (Mantul/Overshoot)
            if (activePinObj != null)
            {
                float scalePin = Mathf.Clamp01(persen) + Mathf.Sin(persen * Mathf.PI) * 0.4f;
                activePinObj.transform.localScale = skalaPinTarget * scalePin;
            }

            // 2. Animasi Pulau (Transisi Warna Smooth & Efek Nafas)
            if (nodeImage != null)
            {
                // Perubahan warna dari abu-abu ke putih secara perlahan
                nodeImage.color = Color.Lerp(lockedColor, unlockedColor, persen);
                
                // Pulaunya sedikit mengembang (5%) lalu kembali normal
                float efekScalePulau = 1f + Mathf.Sin(persen * Mathf.PI) * 0.05f; 
                nodeImage.transform.localScale = skalaPulauAwal * efekScalePulau;
            }
            
            yield return null;
        }
        
        // Memastikan parameter kembali presisi 100% di detik terakhir
        if (activePinObj != null) activePinObj.transform.localScale = skalaPinTarget;
        if (nodeImage != null)
        {
            nodeImage.color = unlockedColor;
            nodeImage.transform.localScale = skalaPulauAwal;
        }

        if (useFloatingAnimation) floatCoroutine = StartCoroutine(FloatingAnimation());
    }

    private void SaveNodeState(int stateValue)
    {
        PlayerPrefs.SetInt(nodeID, stateValue);
        PlayerPrefs.Save();
    }

    private void LoadNodeState()
    {
        int defaultState = (int)currentState; 
        int saveState = PlayerPrefs.GetInt(nodeID, defaultState);
        currentState = (NodeState) saveState;
    }

    private IEnumerator FloatingAnimation()
    {
        while (true) 
        {
            float newY = activePinStartPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
            activePinObj.transform.localPosition = new Vector3(activePinStartPos.x, newY, activePinStartPos.z);
            yield return null;
        }
    }

    private IEnumerator CompletedAnimation()
    {
        while (true)
        {
            float scaleMultiplier = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            completedPinObj.transform.localScale = completedPinStartScale * scaleMultiplier;
            yield return null;
        }
    }
}