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

    [Header("Animasi Pin Aktif")]
    public bool useFloatingAnimation = true;
    public float floatSpeed = 3f;
    public float floatAmount = 10f;
    private Vector3 activePinStartPos;
    private Coroutine floatCoroutine;

    [Header("Click Event")]
    public UnityEvent onNodeActiveClicked;

    void Start()
    {
        LoadNodeState();
        
        // Simpan posisi awal pin aktif untuk dianimasikan
        if (activePinObj != null)
        {
            activePinStartPos = activePinObj.transform.localPosition;
        }

        UpdateNodeVisuals();
    }

    public void UpdateNodeVisuals()
    {
        // 1. Matikan semua pin terlebih dahulu
        if (lockedPinObj != null) lockedPinObj.SetActive(false);
        if (activePinObj != null) activePinObj.SetActive(false);
        if (completedPinObj != null) completedPinObj.SetActive(false);
        
        // 2. Hentikan animasi sebelumnya
        if (floatCoroutine != null) StopCoroutine(floatCoroutine);

        // 3. Nyalakan pin sesuai status
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
                if (completedPinObj != null) completedPinObj.SetActive(true);
                break;
        }
    }

    public void OnNodeClicked()
    {
        if (currentState != NodeState.Locked)
        {
            Debug.Log("Node clicked: " + nodeID);
            onNodeActiveClicked?.Invoke();
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
        int defaultState = (currentState == NodeState.Unlocked) ? 1 : 0;
        int saveState = PlayerPrefs.GetInt(nodeID, defaultState);
        currentState = (NodeState) saveState;
    }

    private IEnumerator FloatingAnimation()
    {
        while (true) // Terus mengambang selama objeknya nyala
        {
            float newY = activePinStartPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
            activePinObj.transform.localPosition = new Vector3(activePinStartPos.x, newY, activePinStartPos.z);
            yield return null;
        }
    }
}