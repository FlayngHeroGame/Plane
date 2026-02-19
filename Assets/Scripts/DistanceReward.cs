using UnityEngine;

public class DistanceReward : MonoBehaviour
{
    public Transform startPoint;
    public Transform forwardReference;  // Forward direction reference (e.g. endPoint)

    public float distance;
    public int coins;

    void Update()
    {
        if (startPoint == null) return;

        // Use forwardReference direction if available and not coincident with startPoint,
        // otherwise fall back to startPoint.forward
        Vector3 forwardDir;
        Vector3 toRef = forwardReference != null ? forwardReference.position - startPoint.position : Vector3.zero;
        if (toRef.magnitude > 0.01f)
        {
            forwardDir = toRef.normalized;
        }
        else
        {
            forwardDir = startPoint.forward;
        }

        // Only count forward distance; clamp backward movement to zero
        distance = Mathf.Max(0f, Vector3.Dot(transform.position - startPoint.position, forwardDir));
        coins = Mathf.FloorToInt(distance / 5f * UpgradeSystem.Instance.MoneyMultiplier);
    }
}
