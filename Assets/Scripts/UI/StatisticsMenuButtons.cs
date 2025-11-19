using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class StatisticsMenuButtons : MonoBehaviour
{
    
   
    public TMP_Text HealthPointsText;
    public TMP_Text StaminaPointsText;
    public TMP_Text AgilityPointsText;
    public TMP_Text StrenghtPointsText;
    public TMP_Text PrecisionPointsText;
    public TMP_Text PointsLeftText;
    public int PointsLeft=10;

    public void StartFight()
    {
        StatsManager.Instance.ConfirmStatistics();
        SceneManager.LoadScene("Fight");
    }

    public void OnHealthButtonClicked()
    {
        StatsManager.Instance.AddHealthStatistic();
        if (PointsLeft > 0)
        {
            HealthPointsText.text = (int.Parse(HealthPointsText.text) + 1).ToString();
            PointsLeft--;
            PointsLeftText.text = PointsLeft.ToString();
        }
    }

    public void OnStaminaButtonClicked()
    {
        StatsManager.Instance.AddStaminaStatistic();
        if (PointsLeft > 0)
        {
            StaminaPointsText.text = (int.Parse(StaminaPointsText.text) + 1).ToString();
            PointsLeft--;
            PointsLeftText.text = PointsLeft.ToString();
        }
    }

    public void OnAgilityButtonClicked()
    {
        StatsManager.Instance.AddAgilityStatistic();
        if (PointsLeft > 0)
        {
            AgilityPointsText.text = (int.Parse(AgilityPointsText.text) + 1).ToString();
            PointsLeft--;
            PointsLeftText.text = PointsLeft.ToString();
        }
    }

    public void OnStrenghtButtonClicked()
    {
        StatsManager.Instance.AddStrenghtStatistic();
        if (PointsLeft > 0)
        {
            StrenghtPointsText.text = (int.Parse(StrenghtPointsText.text) + 1).ToString();
            PointsLeft--;
            PointsLeftText.text = PointsLeft.ToString();
        }
    }

    public void OnPrecisionButtonClicked()
    {
        StatsManager.Instance.AddPrecisionStatistic();
        if (PointsLeft > 0)
        {
            PrecisionPointsText.text = (int.Parse(PrecisionPointsText.text) + 1).ToString();
            PointsLeft--;
            PointsLeftText.text = PointsLeft.ToString();
        }
    }

    public void OnResetButtonClicked() 
    {
        StatsManager.Instance.ResetPoints();
        PointsLeft=10;
        PointsLeftText.text = PointsLeft.ToString();

    }
}
