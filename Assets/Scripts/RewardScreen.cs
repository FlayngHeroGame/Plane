using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class RewardScreen : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI multiplierText;
    public TextMeshProUGUI rewardText;
    public Button claimButton;
    public Button doubleClaimButton;

    float currentReward;

    void Start()
    {
        if (panel != null) panel.SetActive(false);

        if (claimButton != null) claimButton.onClick.AddListener(ClaimReward);
        if (doubleClaimButton != null) doubleClaimButton.onClick.AddListener(DoubleClaimReward);
    }

    public void Show()
    {
        var dr = FindObjectOfType<DistanceReward>();
        float distance = dr != null ? dr.distance : 0f;
        float multiplier = UpgradeSystem.Instance != null ? UpgradeSystem.Instance.MoneyMultiplier : 1f;
        currentReward = distance * multiplier;

        if (distanceText != null) distanceText.text = $"Расстояние: {distance:F1} м";
        if (multiplierText != null) multiplierText.text = $"Множитель: x{multiplier:F1}";
        if (rewardText != null) rewardText.text = $"Награда: {currentReward:F0} монет";

        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0f;
    }

    void ClaimReward()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void DoubleClaimReward()
    {
        currentReward *= 2f;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
