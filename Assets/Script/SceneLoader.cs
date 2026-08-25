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
        SceneManager.LoadSceneAsync(sceneName);
    }
    
    public void Onclick_mainMenu()
    {
        SceneManager.LoadSceneAsync(mainMenuScene);
    }

    public void Onclick_map()
    {
        SceneManager.LoadSceneAsync(mapScene);
    }

    public void Onclick_gameplay()
    {
        SceneManager.LoadSceneAsync(gameplayScene);
    }

    public void Onclick_story()
    {
        SceneManager.LoadSceneAsync(storyScene);
    }

    public void Onclick_exit()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    /// <summary>
    /// Memuat scene gameplay dan menyuntikkan data level yang dipilih.
    /// </summary>
    /// <param name="selectedLevelData">Scriptable Object dari level yang dituju</param>
    public void LoadGameplayWithLevel(LevelData selectedLevelData)
    {
        HiddenObjectManager.ActiveLevelData = selectedLevelData;
        
        SceneManager.LoadSceneAsync(gameplayScene);
    }
}