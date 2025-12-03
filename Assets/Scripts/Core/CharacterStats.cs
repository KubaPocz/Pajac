using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "RPG System/Character Stats")]
public class CharacterStats : ScriptableObject
{
    public string CharacterName;

    [Header("Status Walki")]
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
    public void Initialize(string characterName, int levelPoints, float maxHealth, float maxStamina, int agility, int strenght, int precision)
    {
        CharacterName = characterName;
        LevelPoints = levelPoints;
        MaxHealth = maxHealth; CurrentHealth = MaxHealth;
        MaxStamina = maxStamina; CurrentStamina = MaxStamina;
        Agility = agility; Strenght = strenght; Precision = precision;
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
        // ZMIANA: Tylko resetujemy blok. NIE dodajemy staminy.
        isBlocking = false;
        Debug.Log($"<color=cyan>[TURA]</color> {CharacterName}: Nowa tura (Reset bloku). Stan: {CurrentStamina}/{MaxStamina}");
    }

    public void GetDamage(float amount)
    {
        float previousHP = CurrentHealth;
        if (CurrentHealth - amount > 0f) CurrentHealth -= amount;
        else CurrentHealth = 0f;

        Debug.Log($"<color=red>[OBRAŻENIA]</color> {CharacterName} otrzymał {amount} dmg. HP: {previousHP} -> {CurrentHealth}");
    }

    public bool UseStamina(float amount)
    {
        if (CurrentStamina >= amount)
        {
            float previousSta = CurrentStamina;
            CurrentStamina -= amount;
            Debug.Log($"<color=orange>[KOSZT]</color> {CharacterName} zużywa {amount} staminy. ({previousSta} -> {CurrentStamina})");
            return true;
        }
        else
        {
            Debug.Log($"<color=grey>[BRAK SIŁ]</color> {CharacterName} chciał zużyć {amount}, ale ma tylko {CurrentStamina}!");
            return false;
        }
    }

    public void RestoreStamina(float amount = 40f)
    {
        float previousSta = CurrentStamina;
        CurrentStamina += amount;
        if (CurrentStamina > MaxStamina) CurrentStamina = MaxStamina;

        Debug.Log($"<color=green>[ODPOCZYNEK]</color> {CharacterName} odpoczywa (+{amount}). Sta: {previousSta} -> {CurrentStamina}");
    }

    // --- ROZWÓJ POSTACI ---
    public void IncreaseHealth() { MaxHealth += 10f; CurrentHealth = MaxHealth; }
    public void IncreaseStamina() { MaxStamina += 10f; CurrentStamina = MaxStamina; }
    public void IncreaseAgility() { Agility++; }
    public void IncreaseStrenght() { Strenght++; }
    public void IncreasePrecision() { Precision++; }
}