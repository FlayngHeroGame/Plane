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

    int currentRewardCoins;

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
        int coins = dr != null ? dr.coins : 0;
        float multiplier = UpgradeSystem.Instance != null ? UpgradeSystem.Instance.MoneyMultiplier : 1f;

        currentRewardCoins = coins;

        if (distanceText != null) distanceText.text = $"{distance:F0}м";
        if (multiplierText != null) multiplierText.text = $"x{multiplier:F1}";
        if (rewardText != null) rewardText.text = currentRewardCoins.ToString();

        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0f;
    }

    void ClaimReward()
    {
        if (CoinManager.Instance != null)
            CoinManager.Instance.AddCoins(currentRewardCoins);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void DoubleClaimReward()
    {
        if (CoinManager.Instance != null)
            CoinManager.Instance.AddCoins(currentRewardCoins * 2);

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
