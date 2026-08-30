using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;
    
    private int totalCoins;
    private const string COIN_KEY = "PlayerTotalCoins";

    private void Awake()
    {
        // Sistem Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCoins();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadCoins()
    {
        // Ambil data koin yang tersimpan, kalau belum ada set 0
        totalCoins = PlayerPrefs.GetInt(COIN_KEY, 0);
    }

    public void AddCoins(int amount)
    {
        totalCoins += amount;
        SaveCoins();
    }

    public bool SpendCoins(int amount)
    {
        // Cek apakah koin cukup untuk dibelanjakan
        if (totalCoins >= amount)
        {
            totalCoins -= amount;
            SaveCoins();
            return true; // Transaksi berhasil
        }
        return false; // Transaksi gagal (koin kurang)
    }

    public int GetTotalCoins()
    {
        return totalCoins;
    }

    private void SaveCoins()
    {
        PlayerPrefs.SetInt(COIN_KEY, totalCoins);
        PlayerPrefs.Save();
    }
}