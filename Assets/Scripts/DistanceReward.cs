using UnityEngine;

public class DistanceReward : MonoBehaviour
{
    public Transform startPoint;

    public float distance;
    public int coins;

    void Update()
    {
        distance = Vector3.Distance(startPoint.position, transform.position);
        coins = Mathf.FloorToInt(distance / 5f * UpgradeSystem.Instance.MoneyMultiplier);
    }
}
