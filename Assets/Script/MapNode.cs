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

    // --- UBAH STRING JADI SO LEVEL DATA ---
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
                if (nodeImage != null) nodeImage.color = Color.white; 
                
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
            Debug.Log("Node clicked: " + nodeID);
            onNodeActiveClicked?.Invoke();

            // --- KIRIM SO KE MASCOT POPUP ---
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
        currentState = NodeState.Completed;
        SaveNodeState(2);
        UpdateNodeVisuals();

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
            UpdateNodeVisuals();
        }
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