using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SecretItem : MonoBehaviour
{
    [Header("Data Utama")]
    public ItemDataSO dataPusaka;

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
        if (gameManager != null && gameManager.isLevelSelesai)
        {
            return;
        }

        if (!isCollected && gameManager != null)
        {
            isCollected = true;
            
            gameManager.ShowSecretPopup(GetComponent<Image>().sprite, secretName, secretDescription, coinReward, dataPusaka); 
            
            gameObject.SetActive(false);
        }
    }

    private void OnValidate()
    {
        if (dataPusaka != null && dataPusaka.gambarItem != null)
        {
            Image img = GetComponent<Image>();
            if (img != null)
            {
                img.sprite = dataPusaka.gambarItem;
                img.preserveAspect = true;
            }

            // Otomatis isi Nama popup pakai data SO
            if (string.IsNullOrEmpty(secretName))
            {
                secretName = dataPusaka.namaItem;
            }
        }
    }
}