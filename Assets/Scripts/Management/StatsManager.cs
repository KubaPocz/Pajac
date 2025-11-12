using System;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;
    public CharacterInfo player;
    private int StatPoints = 10;
    private int[] StatisticsToPass = new int[5];
    // Array containing statistics to pass towards CharacterInfo Methods
    // Following this format: 0-Health 1-Stamina 2-Agility 3-Strenght 4-Precision
    [SerializeField] public CharacterStats PlayerStats;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    public void AddHealthStatistic()
    { 
        if (StatPoints > 0) 
        {
            StatPoints--;
            StatisticsToPass[0]++;
        }
    }
    public void AddStaminaStatistic()
    {
        if (StatPoints > 0)
        {
            StatPoints--;
            StatisticsToPass[1]++;
        }
    }
    public void AddAgilityStatistic()
    {
        if (StatPoints > 0)
        {
            StatPoints--;
            StatisticsToPass[2]++;
        }
    }
    public void AddStrenghtStatistic()
    {
        if (StatPoints > 0)
        {
            StatPoints--;
            StatisticsToPass[3]++;
        }
    }
    public void AddPrecisionStatistic()
    {

        if (StatPoints > 0)
        {
            StatPoints--;
            StatisticsToPass[4]++;
        }
    }

    public void ResetPoints()
    {
        StatPoints = 10;
        Array.Clear(StatisticsToPass, 0, StatisticsToPass.Length);
    }

    public void ConfirmStatistics() 
    {
        for (int i = StatisticsToPass[0]; i-- > 0;) PlayerStats.IncreaseHealth();
        for (int i = StatisticsToPass[0]; i-- > 0;) PlayerStats.IncreaseStamina();
        for (int i = StatisticsToPass[0]; i-- > 0;) PlayerStats.IncreaseAgility();
        for (int i = StatisticsToPass[0]; i-- > 0;) PlayerStats.IncreaseStrenght();
        for (int i = StatisticsToPass[0]; i-- > 0;) PlayerStats.IncreasePrecision();
    }
}
