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

        float totalDistance = Vector3.Distance(startPoint.position, endPoint.position);
        if (totalDistance < 0.01f) return;

        float currentDistance = Vector3.Distance(startPoint.position, plane.position);
        progressSlider.value = Mathf.Clamp01(currentDistance / totalDistance);

        if (distanceLabel != null)
            distanceLabel.text = $"{currentDistance:F0} м";
    }
}
