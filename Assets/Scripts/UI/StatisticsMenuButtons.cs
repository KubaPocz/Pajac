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
    public TMP_Text PointsText;
    private int PointsLeft=10;


    public void Awake()
    {
        PointsText.text = (PointsLeft.ToString());
    }
    public void StartFight()
    {
        CurtainManager.Instance.ChangeScene("Fight", "Statistics", false);
        StatsManager.Instance.ConfirmStatistics();
    }

    public void OnHealthButtonClicked()
    {
        StatsManager.Instance.AddHealthStatistic();
        if (PointsLeft > 0)
        {
            HealthPointsText.text = (int.Parse(HealthPointsText.text) + 1).ToString();
            PointsLeft--;
            PointsText.text = (PointsLeft.ToString());
        }
    }

    public void OnStaminaButtonClicked()
    {
        StatsManager.Instance.AddStaminaStatistic();
        if (PointsLeft > 0)
        {
            StaminaPointsText.text = (int.Parse(StaminaPointsText.text) + 1).ToString();
            PointsLeft--;
            PointsText.text = (PointsLeft.ToString());
        }
    }

    public void OnAgilityButtonClicked()
    {
        StatsManager.Instance.AddAgilityStatistic();
        if (PointsLeft > 0)
        {
            AgilityPointsText.text = (int.Parse(AgilityPointsText.text) + 1).ToString();
            PointsLeft--;
            PointsText.text = (PointsLeft.ToString());
        }
    }

    public void OnStrenghtButtonClicked()
    {
        StatsManager.Instance.AddStrenghtStatistic();
        if (PointsLeft > 0)
        {
            StrenghtPointsText.text = (int.Parse(StrenghtPointsText.text) + 1).ToString();
            PointsLeft--;
            PointsText.text = (PointsLeft.ToString());
        }
    }

    public void OnPrecisionButtonClicked()
    {
        StatsManager.Instance.AddPrecisionStatistic();
        if (PointsLeft > 0)
        {
            PrecisionPointsText.text = (int.Parse(PrecisionPointsText.text) + 1).ToString();
            PointsLeft--;
            PointsText.text = (PointsLeft.ToString());
        }
    }

    public void OnResetButtonClicked() 
    {
        PointsLeft = 10;
        HealthPointsText.text = "0";
        StaminaPointsText.text = "0";
        AgilityPointsText.text = "0";
        StrenghtPointsText.text = "0";
        PrecisionPointsText.text = "0";
        PointsText.text = (PointsLeft.ToString());
    }
}
