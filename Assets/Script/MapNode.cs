using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class MapNode : MonoBehaviour
{
    public enum NodeState { Locked, Unlocked, Completed }

    [Header("Status Node")]
    public NodeState currentState = NodeState.Locked;
    public string nodeID; // Unique identifier for the node

    [Header("Navigation Settings")]
    public MapNode nextNode;// Reference to the next node in the sequence

    [Header("UI Elements")]
    public Button nodeButton;
    public Image nodeImage;
    public GameObject pinObject;

    [Header("Color Settings")]
    public Color lockedColor = new Color(0.5f, 0.5f, 0.5f); // Gray
    public Color unlockedColor = Color.white; // White
    public Color completedColor = Color.green; // Green

    [Header("Animasi Pin")]
    public bool useFloatingAnimation = true;
    public float floatSpeed = 3f;
    public float floatAmount = 10f;
    private Vector3 pinStartPos;

    [Header("Click Event")]
    public UnityEvent onNodeActiveClicked;

    void Start()
    {
        LoadNodeState();
        UpdateNodeVisuals();

        if (pinObject != null)
        {
            pinStartPos = pinObject.transform.localPosition;
            
            if (useFloatingAnimation && currentState != NodeState.Locked)
            {
                StartCoroutine(FloatingAnimation());
            }
        }
    }

    public void UpdateNodeVisuals()
    {
        switch (currentState)
        {
            case NodeState.Locked:
                nodeButton.interactable = false;
                nodeImage.color = lockedColor;
                if (pinObject != null) pinObject.SetActive(false);
                break;
            
            case NodeState.Unlocked:
                nodeButton.interactable = true;
                nodeImage.color = unlockedColor;
                if (pinObject != null) pinObject.SetActive(true);
                break;

            case NodeState.Completed:
                nodeButton.interactable = true;
                nodeImage.color = completedColor;
                if (pinObject != null) pinObject.SetActive(true);
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
        SaveNodeState(2); // 2 represents Completed state
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
            SaveNodeState(1); // 1 represents Unlocked state
            UpdateNodeVisuals();
            
            // Nyalain animasi kalau baru di-unlock
            if (useFloatingAnimation && pinObject != null)
            {
                StartCoroutine(FloatingAnimation());
            }
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
        while (currentState != NodeState.Locked)
        {
            float newY = pinStartPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
            pinObject.transform.localPosition = new Vector3(pinStartPos.x, newY, pinStartPos.z);
            yield return null;
        }
    }
}