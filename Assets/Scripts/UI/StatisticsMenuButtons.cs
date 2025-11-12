using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class StatisticsMenuButtons : MonoBehaviour
{
     
    public Text HealthPointsText;
    public Text StaminaPointsText;
    public Text AgilityPointsText;
    public Text StrenghtPointsText;
    public Text PrecisionPointsText;
    private int PointsLeft=10;

    public void StartFight()
    {
        StatsManager.Instance.ConfirmStatistics();
        SceneManager.LoadScene("Fight");
    }

    private void OnHealthButtonClicked()
    {
        StatsManager.Instance.AddHealthStatistic();
        

    }

    private void OnStaminaButtonClicked()
    {
        StatsManager.Instance.AddStaminaStatistic();
    }

    private void OnAgilityButtonClicked()
    {
        StatsManager.Instance.AddAgilityStatistic();
    }

    private void OnStrenghtButtonClicked()
    {
        StatsManager.Instance.AddStrenghtStatistic();
    }

    private void OnPrecisionButtonClicked()
    {
        StatsManager.Instance.AddPrecisionStatistic();
    }

    private void OnResetButtonClicked() 
    {
        StatsManager.Instance.ResetPoints();
    }
}
