using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public static UpgradeSystem Instance;

    public int slingLevel = 1;
    public int planeLevel = 1;
    public int moneyLevel = 1;

    void Awake()
    {
        Instance = this;
    }

    public float SlingPower => 1f + slingLevel * 0.3f;

    public float LiftBonus => 1f + planeLevel * 0.1f;

    public float MoneyMultiplier => 1f + moneyLevel * 0.5f;
}
