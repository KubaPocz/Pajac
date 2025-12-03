using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "RPG System/Character Stats")]
public class CharacterStats : ScriptableObject
{
    public string CharacterName;

    [Header("Status")]
    public float CurrentHealth;
    public float MaxHealth;
    public float CurrentStamina;
    public float MaxStamina;
    public bool isBlocking = false;

    [Header("Atrybuty")]
    public int Agility;
    public int Strenght;
    public int Precision;
    public int LevelPoints;

    // --- INICJALIZACJA ---
    public void Initialize(string name, int levels, float hp, float sta, int agi, int str, int prec)
    {
        CharacterName = name; LevelPoints = levels;
        MaxHealth = hp; CurrentHealth = hp;
        MaxStamina = sta; CurrentStamina = sta;
        Agility = agi; Strenght = str; Precision = prec;
        isBlocking = false;
    }

    public void Initialize()
    {
        CurrentHealth = MaxHealth;
        CurrentStamina = MaxStamina;
        isBlocking = false;
    }

    // --- LOGIKA WALKI ---

    public void NewTurnRegen()
    {
        // TYLKO RESET BLOKU - BRAK DODAWANIA STAMINY
        isBlocking = false;
        Debug.Log($"<color=grey>[TURA]</color> {CharacterName}: Rozpoczyna turę. Blok zdjęty.");
    }

    public void GetDamage(float amount)
    {
        float before = CurrentHealth;
        CurrentHealth -= amount;
        if (CurrentHealth < 0) CurrentHealth = 0;

        Debug.Log($"<color=red>[OBRAŻENIA]</color> {CharacterName} traci {amount} HP. ({before} -> {CurrentHealth})");
    }

    public bool UseStamina(float amount)
    {
        if (CurrentStamina >= amount)
        {
            float before = CurrentStamina;
            CurrentStamina -= amount;
            Debug.Log($"<color=orange>[KOSZT]</color> {CharacterName} zużył {amount} Stam. ({before} -> {CurrentStamina})");
            return true;
        }

        Debug.Log($"<color=grey>[BRAK SIŁ]</color> {CharacterName} potrzebuje {amount}, ale ma {CurrentStamina} Stam.");
        return false;
    }

    public void RestoreStamina(float amount = 40f)
    {
        float before = CurrentStamina;
        CurrentStamina += amount;
        if (CurrentStamina > MaxStamina) CurrentStamina = MaxStamina;
        Debug.Log($"<color=green>[SEN]</color> {CharacterName} odnowił {amount} Stam. ({before} -> {CurrentStamina})");
    }

    // --- ROZWÓJ POSTACI ---
    public void IncreaseHealth() { MaxHealth += 10f; CurrentHealth = MaxHealth; }
    public void IncreaseStamina() { MaxStamina += 10f; CurrentStamina = MaxStamina; }
    public void IncreaseAgility() { Agility++; }
    public void IncreaseStrenght() { Strenght++; }
    public void IncreasePrecision() { Precision++; }
}