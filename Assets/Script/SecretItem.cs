using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SecretItem : MonoBehaviour
{
    [Header("Secret Item Coin Reward " )]
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

            gameManager.SecretItemFound(GetComponent<Image>().sprite, coinReward);
            gameObject.SetActive(false);
        }
    }
}
