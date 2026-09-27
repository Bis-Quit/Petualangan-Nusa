using UnityEngine;
using System.Collections;

public class MapManager : MonoBehaviour
{
    [Header("Daftar Node Pulau")]
    public MapNode[] semuaNode; 

    [Header("BOM NUKLIR RUNTIME (CENTANG UNTUK RESET)")]
    [Tooltip("Centang ini, lalu Play game untuk mereset paksa semua data di Mac")]
    public bool HAPUS_DATA_SAAT_PLAY = false;

    IEnumerator Start()
    {
        if (HAPUS_DATA_SAAT_PLAY)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            
            foreach (MapNode node in semuaNode)
            {
                node.currentState = MapNode.NodeState.Locked;
                PlayerPrefs.SetInt(node.nodeID, 0); 
                node.UpdateNodeVisuals();
            }
            PlayerPrefs.Save();
            Debug.Log("BOM NUKLIR RUNTIME MELEDAK! Semua data bersih total.");
        }

        yield return null; 

        if (semuaNode.Length > 0 && semuaNode[0].currentState == MapNode.NodeState.Locked)
        {
            semuaNode[0].UnlockThisNode();
        }

        string levelSelesai = PlayerPrefs.GetString("LevelBaruSelesai", "");

        if (!string.IsNullOrEmpty(levelSelesai))
        {
            yield return new WaitForSeconds(1.0f);

            foreach (MapNode node in semuaNode)
            {
                if (node.levelData != null && node.levelData.levelName == levelSelesai)
                {
                    node.CompleteThisNode(); 
                    break;
                }
            }

            PlayerPrefs.DeleteKey("LevelBaruSelesai");
        }

        if (AudioManager.Instance != null && AudioManager.Instance.bgmPetaUtama != null)
        {
            AudioManager.Instance.GantiBGM(AudioManager.Instance.bgmPetaUtama);
        }
    }
    
    [ContextMenu("Reset Progres Peta")]
    public void ResetProgresMap()
    {
        PlayerPrefs.DeleteAll(); 
        PlayerPrefs.Save(); 
        
        foreach (MapNode node in semuaNode)
        {
            node.currentState = MapNode.NodeState.Locked;
            node.UpdateNodeVisuals();
        }
    }
}