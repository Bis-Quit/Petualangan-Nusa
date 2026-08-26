using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SecretItem : MonoBehaviour
{
    [Header("Data Item Rahasia")]
    public string secretName;
    [TextArea] public string secretDescription;
    public int coinReward = 0;

    [HideInInspector] public HiddenObjectManager gameManager;
    private bool isCollected = false;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnSecretClicked);
    }

    public void OnSecretClicked()
    {
        if (!isCollected && gameManager != null)
        {
            isCollected = true;
            
            gameManager.ShowSecretPopup(GetComponent<Image>().sprite, secretName, secretDescription, coinReward);
            
            gameObject.SetActive(false);
        }
    }
}