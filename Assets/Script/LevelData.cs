using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "HiddenObject/Level Data")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public Sprite backgroundSprite;

    [Header("Main Items")]
    public List<GameObject> itemPrefabs;

    [Header("Decoy Items")]
    public List<GameObject> decoyPrefabs;

    [Header("Secret Items")]
    public List<GameObject> secretItemPrefabs;
}