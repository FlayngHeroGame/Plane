using UnityEngine;
using TMPro;

public class Speedometer : MonoBehaviour
{
    public Rigidbody planeRb;
    public TextMeshProUGUI speedText;
    public float displayMultiplier = 3.6f; // м/с -> км/ч

    void Update()
    {
        if (planeRb == null || speedText == null) return;
        
        float speed = planeRb.linearVelocity.magnitude * displayMultiplier;
        speedText.text = speed < 0.1f ? "0 км/ч" : $"{speed:F0} км/ч";
    }
}
