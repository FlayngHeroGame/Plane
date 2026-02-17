using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public static UpgradeSystem Instance;

    public int slingLevel = 1;
    public int planeLevel = 1;
    public int moneyLevel = 1;

    public GameObject[] wingParts; // Визуальные части самолёта (крылья)

    void Awake()
    {
        Instance = this;
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
            float bonus = 1f + planeLevel * 0.1f;          // базовый бонус за каждый уровень
            int wingCount = planeLevel / 5;                 // кол-во крыльев
            bonus += wingCount * 0.5f;                      // скачок аэродинамики за крылья
            return bonus;
        }
    }

    public float MoneyMultiplier => 1f + moneyLevel * 0.5f;

    public int PlaneVisualLevel => planeLevel / 5; // Сколько крыльев активно

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
