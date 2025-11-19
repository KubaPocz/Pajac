using System;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    private CharacterStats player;

    public CharacterStats playerStats;
    private int[] StatisticsToPass = new int[5];
    // Array containing statistics to pass towards CharacterInfo Methods
    // Following this format: 0-Health 1-Stamina 2-Agility 3-Strenght 4-Precision
    [SerializeField] public CharacterStats PlayerStats;
    private int StatPoints = 10;
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
        StatPoints = PlayerStats.LevelPoints;
    }
    private void OnEnable()
    {
        ResetPoints();
    }

    public void AddHealthStatistic()
    {
        if (StatPoints > 0)
        {
            Debug.Log("Health clicked");
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
        StatPoints = PlayerStats.LevelPoints;
        Array.Clear(StatisticsToPass, 0, StatisticsToPass.Length);
    }

    public void ConfirmStatistics()
    {
        for (int i = 0; i < StatisticsToPass[0]; i++) { PlayerStats.IncreaseHealth();}
        for (int i = 0; i < StatisticsToPass[1]; i++) PlayerStats.IncreaseStamina();
        for (int i = 0; i < StatisticsToPass[2]; i++) PlayerStats.IncreaseAgility();
        for (int i = 0; i < StatisticsToPass[3]; i++) PlayerStats.IncreaseStrenght();
        for (int i = 0; i < StatisticsToPass[4]; i++) PlayerStats.IncreasePrecision();
    }
}