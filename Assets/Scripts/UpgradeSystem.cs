using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public static UpgradeSystem Instance;

    [Header("Уровни прокачки")]
    public int slingLevel = 1;
    public int planeLevel = 1;
    public int moneyLevel = 1;

    [Header("Стоимость прокачки")]
    public int slingUpgradeCost = 50;
    public int planeUpgradeCost = 80;
    public int moneyUpgradeCost = 100;
    public float costMultiplier = 1.5f;

    public GameObject[] wingParts;

    const string KEY_SLING = "SlingLevel";
    const string KEY_PLANE = "PlaneLevel";
    const string KEY_MONEY = "MoneyLevel";

    void Awake()
    {
        Instance = this;
        LoadLevels();
    }

    void Start()
    {
        UpdatePlaneVisuals();
    }

    public float SlingPower => 1f + slingLevel * 0.3f;

    public float LiftBonus
    {
        get
        {
            float bonus = 1f + planeLevel * 0.1f;
            int wingCount = planeLevel / 5;
            bonus += wingCount * 0.5f;
            return bonus;
        }
    }

    public float MoneyMultiplier => 1f + moneyLevel * 0.5f;

    public int PlaneVisualLevel => planeLevel / 5;

    public int GetSlingCost()
    {
        return Mathf.RoundToInt(slingUpgradeCost * Mathf.Pow(costMultiplier, slingLevel - 1));
    }

    public int GetPlaneCost()
    {
        return Mathf.RoundToInt(planeUpgradeCost * Mathf.Pow(costMultiplier, planeLevel - 1));
    }

    public int GetMoneyCost()
    {
        return Mathf.RoundToInt(moneyUpgradeCost * Mathf.Pow(costMultiplier, moneyLevel - 1));
    }

    public bool UpgradeSling()
    {
        int cost = GetSlingCost();
        if (CoinManager.Instance == null || !CoinManager.Instance.SpendCoins(cost))
            return false;

        slingLevel++;
        SaveLevels();
        return true;
    }

    public bool UpgradePlane()
    {
        int cost = GetPlaneCost();
        if (CoinManager.Instance == null || !CoinManager.Instance.SpendCoins(cost))
            return false;

        planeLevel++;
        SaveLevels();
        UpdatePlaneVisuals();
        return true;
    }

    public bool UpgradeMoney()
    {
        int cost = GetMoneyCost();
        if (CoinManager.Instance == null || !CoinManager.Instance.SpendCoins(cost))
            return false;

        moneyLevel++;
        SaveLevels();
        return true;
    }

    void SaveLevels()
    {
        PlayerPrefs.SetInt(KEY_SLING, slingLevel);
        PlayerPrefs.SetInt(KEY_PLANE, planeLevel);
        PlayerPrefs.SetInt(KEY_MONEY, moneyLevel);
        PlayerPrefs.Save();
    }

    void LoadLevels()
    {
        slingLevel = PlayerPrefs.GetInt(KEY_SLING, 1);
        planeLevel = PlayerPrefs.GetInt(KEY_PLANE, 1);
        moneyLevel = PlayerPrefs.GetInt(KEY_MONEY, 1);
    }

    public void UpdatePlaneVisuals()
    {
        if (wingParts == null) return;

        int visualLevel = PlaneVisualLevel;

        for (int i = 0; i < wingParts.Length; i++)
        {
            if (wingParts[i] != null)
            {
                wingParts[i].SetActive(i < visualLevel);
            }
        }
    }
}

