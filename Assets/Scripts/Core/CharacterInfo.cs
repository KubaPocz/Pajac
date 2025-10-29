public class CharacterStats
{
    public string CharacterName;
    public float CurrentHealth;
    public float MaxHealth;
    public float CurrentStamina;
    public float MaxStamina;
    public int Agility;
    public int Strenght;
    public int Precision;
    public int LevelPoints;

    public CharacterStats(  string characterName, 
                            int levelPoints, 
                            float maxHealth, 
                            float maxStamina, 
                            int agility, 
                            int strenght, 
                            int precision)
    {
        CharacterName = characterName;
        LevelPoints = levelPoints;

        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;

        MaxStamina = maxStamina;
        CurrentStamina = MaxStamina;

        Agility = agility;
        Strenght = strenght;
        Precision = precision;
    }
    
    public void GetDamage(float ammount)
    {
        if(CurrentHealth - ammount>0f)
            CurrentHealth -= ammount;
        else
        {
            CurrentHealth = 0f;
            throw new System.Exception($"{CharacterName} died!");
        }
    }
    public void UseStamina(float ammount)
    {
        if (CurrentStamina - ammount > 0f)
            CurrentStamina -= ammount;
        else
        {
            CurrentStamina = 0f;
            RestoreStamina();
        }
    }
    public void IncreaseHealth()
    {
        MaxHealth += 10f;
        LevelPoints--;
    }
    public void IncreaseStamina()
    {
        MaxStamina += 10f;
        LevelPoints--;
    }
    public void IncreaseAgility()
    {
        if (LevelPoints > 0)
        {
            Agility++;
            LevelPoints--;
        }
    }
    public void IncreaseStrenght()
    {
        if (LevelPoints > 0)
        {
            Strenght++;
            LevelPoints--;
        }
    }
    public void IncreasePrecision()
    {
        if (LevelPoints > 0)
        {
            Precision++;
            LevelPoints--;
        }
    }
    public void RestoreStamina()
    {
        if (CurrentStamina + 40f <= MaxStamina)
            CurrentStamina += 40f;
        else
            CurrentStamina = MaxStamina;
    }
    public void GetLevelPoints(int ammount)
    {
        LevelPoints += ammount;
    }
}
