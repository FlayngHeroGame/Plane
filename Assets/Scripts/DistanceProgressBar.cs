using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DistanceProgressBar : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public Transform plane;
    public Slider progressSlider;
    public TextMeshProUGUI distanceLabel;

    void Update()
    {
        if (startPoint == null || endPoint == null || plane == null || progressSlider == null) return;

        Vector3 startToEnd = endPoint.position - startPoint.position;
        float totalDistance = startToEnd.magnitude;
        if (totalDistance < 0.01f) return;

        // Direction "forward" from start to finish
        Vector3 forwardDir = startToEnd / totalDistance;

        // Project the plane's position onto the forward axis.
        // Negative values (behind the start) are clamped to zero.
        float forwardDistance = Mathf.Max(0f, Vector3.Dot(plane.position - startPoint.position, forwardDir));

        progressSlider.value = Mathf.Clamp01(forwardDistance / totalDistance);

        if (distanceLabel != null)
            distanceLabel.text = $"{forwardDistance:F0} м";
    }
}
