using UnityEngine;
using TMPro;

public class UICoinDisplay : MonoBehaviour
{
    [Header("UI Komponen")]
    public TextMeshProUGUI coinText;

    void Start()
    {
        UpdateCoinUI();
    }

    public void UpdateCoinUI()
    {
        // Cek apakah Bank Pusat (CoinManager) ada di game
        if (CoinManager.Instance != null)
        {
            // Ambil total koin, ubah jadi teks
            coinText.text = CoinManager.Instance.GetTotalCoins().ToString();
        }
        else
        {
            Debug.LogWarning("Bro, CoinManager belum ke-load di scene ini!");
        }
    }
}