using UnityEngine;
using TMPro;
using System;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI")]
    public TextMeshProUGUI coinText;

    const string PREFS_KEY = "PlayerCoins";

    int coins;

    public int Coins => coins;

    public event Action<int> OnCoinsChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        coins = PlayerPrefs.GetInt(PREFS_KEY, 0);
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        coins += amount;
        Save();
        UpdateUI();
        OnCoinsChanged?.Invoke(coins);
    }

    public bool SpendCoins(int amount)
    {
        if (amount <= 0 || coins < amount) return false;
        coins -= amount;
        Save();
        UpdateUI();
        OnCoinsChanged?.Invoke(coins);
        return true;
    }

    public bool CanAfford(int amount)
    {
        return coins >= amount;
    }

    void Save()
    {
        PlayerPrefs.SetInt(PREFS_KEY, coins);
        PlayerPrefs.Save();
    }

    void UpdateUI()
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }
}
