using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string mainMenuScene = "";
    [SerializeField] private string gameplayScene = "";
    [SerializeField] private string mapScene = "";
    [SerializeField] private string storyScene = "";

    public void LoadSceneByName(string sceneName)
    {
        TransisiScene.Instance.PindahScene(sceneName);
    }
    
    public void Onclick_mainMenu()
    {
        TransisiScene.Instance.PindahScene(mainMenuScene);
    }

    public void Onclick_map()
    {
        TransisiScene.Instance.PindahScene(mapScene);
    }

    public void Onclick_gameplay()
    {
        TransisiScene.Instance.PindahScene(gameplayScene);
    }

    public void Onclick_story()
    {
        TransisiScene.Instance.PindahScene(storyScene);
    }

    public void Onclick_exit()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void LoadGameplayWithLevel(LevelData selectedLevelData)
    {
        HiddenObjectManager.ActiveLevelData = selectedLevelData;
        TransisiScene.Instance.PindahScene(gameplayScene);
    }
}