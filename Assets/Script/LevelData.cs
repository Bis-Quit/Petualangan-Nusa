using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "HiddenObject/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;

    [Header("Environment Setup")]
    public GameObject environmentPrefab; 

    [Header("Time Setting")]
    public float timeLimit = 60f;

    [Header("Main Items")]
    public List<GameObject> itemPrefabs;

    [Header("Decoy Items")]
    public List<GameObject> decoyPrefabs;

    [Header("Secret Items")]
    public List<GameObject> secretItemPrefabs;
}