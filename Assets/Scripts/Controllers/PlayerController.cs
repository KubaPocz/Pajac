using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterStats PlayerStats;
    public float moveSpeed = 5f;

    // Potrzebujemy wroga, żeby go bić
    private EnemyController enemyRef;

    private void Start()
    {
        if (GameManager.Instance != null) PlayerStats = GameManager.Instance.Player;
        enemyRef = FindObjectOfType<EnemyController>();
    }

    // Wywołaj to przyciskiem "End Turn" lub automatycznie na starcie tury gracza
    public void StartPlayerTurn()
    {
        PlayerStats.NewTurnRegen();
        Debug.Log("<color=green>--- TURA GRACZA ---</color> (+20 Staminy)");
    }

    // --- AKCJE GRACZA (Podpięte pod przyciski) ---

    public void MoveRight()
    {
        if (PlayerStats.UseStamina(5)) // Koszt 5
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
    }

    public void MoveLeft()
    {
        if (PlayerStats.UseStamina(5)) // Koszt 5
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
    }

    public void Sleep()
    {
        PlayerStats.RestoreStamina(40); // Odnawia 40
        Debug.Log("Gracz śpi (+40 Staminy).");
    }

    public void Block()
    {
        if (PlayerStats.UseStamina(15)) // Koszt 15
        {
            PlayerStats.isBlocking = true;
            Debug.Log("Gracz blokuje.");
        }
    }

    // --- ATAKI GRACZA ---

    public void AttackLight()
    {
        // Koszt 10, Dmg * 1
        if (PlayerStats.UseStamina(10))
            DealDamageToEnemy(PlayerStats.Strenght * 1.0f, "Lekki Atak");
        else
            Debug.Log("Za mało staminy!");
    }

    public void AttackMedium()
    {
        // Koszt 20, Dmg * 1.5
        if (PlayerStats.UseStamina(20))
            DealDamageToEnemy(PlayerStats.Strenght * 1.5f, "Średni Atak");
        else
            Debug.Log("Za mało staminy!");
    }

    public void AttackHeavy()
    {
        // Koszt 30, Dmg * 2
        if (PlayerStats.UseStamina(30))
            DealDamageToEnemy(PlayerStats.Strenght * 2.0f, "Ciężki Atak");
        else
            Debug.Log("Za mało staminy!");
    }

    // --- KALKULATOR OBRAŻEŃ (Taki sam jak u wroga) ---
    private void DealDamageToEnemy(float baseDamage, string attackName)
    {
        if (enemyRef == null) return;
        CharacterStats target = enemyRef.EnemyStats;

        // 1. Trafienie
        float hitChance = 80f + (PlayerStats.Precision - target.Precision);
        if (Random.Range(0f, 100f) > hitChance)
        {
            Debug.Log($"Gracz: {attackName} PUDŁUJE!");
            return;
        }

        // 2. Unik Wroga
        float dodgeChance = 10f + (target.Agility - PlayerStats.Agility);
        dodgeChance = Mathf.Clamp(dodgeChance, 5f, 50f);
        if (Random.Range(0f, 100f) < dodgeChance)
        {
            Debug.Log($"Gracz: Wróg zrobił UNIK!");
            return;
        }

        // 3. Blok Wroga
        float finalDamage = baseDamage;
        if (target.isBlocking)
        {
            float reduction = 50f + (target.Agility * 0.5f);
            if (reduction > 80f) reduction = 80f;
            finalDamage -= finalDamage * (reduction / 100f);
            Debug.Log("Gracz: Wróg zablokował część obrażeń.");
        }

        target.GetDamage(finalDamage);
        Debug.Log($"Gracz: {attackName} trafia za {finalDamage} hp!");
    }
}