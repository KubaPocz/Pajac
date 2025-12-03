using UnityEngine;

public class PlayerController : MonoBehaviour, CharacterController
{
    public CharacterStats PlayerStats;
    public float moveSpeed = 5f;
    private bool isMyTurn = false;
    private EnemyController currentEnemy;

    private void Start()
    {
        if (GameManager.Instance != null) PlayerStats = GameManager.Instance.Player;
        currentEnemy = FindObjectOfType<EnemyController>();
    }

    public void TakeTurn()
    {
        isMyTurn = true;
        PlayerStats.NewTurnRegen();
        Debug.Log("\n<color=blue>--- TURA GRACZA ---</color> (Wybierz akcję)");
    }

    private void Update()
    {
        if (!isMyTurn) return;

        // Skip (K)
        if (Input.GetKeyDown(KeyCode.K)) { Debug.Log("SKIP"); EndTurn(); }

        // Sterowanie
        if (Input.GetKeyDown(KeyCode.A)) AttackLight();
        if (Input.GetKeyDown(KeyCode.S)) AttackMedium();
        if (Input.GetKeyDown(KeyCode.D)) AttackStrong();
        if (Input.GetKeyDown(KeyCode.Space)) Sleep();
        if (Input.GetKeyDown(KeyCode.B)) Block();
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveRight();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveLeft();
    }

    private void EndTurn()
    {
        isMyTurn = false;
        // Pokaż tabelkę i oddaj turę
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.LogBattleState();
            BattleManager.Instance.StartEnemyTurn();
        }
    }

    // --- AKCJE ---
    public void MoveRight() { if (PlayerStats.UseStamina(5)) { transform.Translate(Vector3.right * moveSpeed * Time.deltaTime * 20f); EndTurn(); } }
    public void MoveLeft() { if (PlayerStats.UseStamina(5)) { transform.Translate(Vector3.left * moveSpeed * Time.deltaTime * 20f); EndTurn(); } }
    public void Sleep() { PlayerStats.RestoreStamina(40); EndTurn(); }
    public void Block() { if (PlayerStats.UseStamina(15)) { PlayerStats.isBlocking = true; Debug.Log("Gracz: Postawa obronna."); EndTurn(); } }
    public void Dodge() { }

    public void AttackLight() { TryPlayerAttack(10, 1.0f, "Lekki"); }
    public void AttackMedium() { TryPlayerAttack(20, 1.5f, "Średni"); }
    public void AttackStrong() { TryPlayerAttack(30, 2.0f, "Ciężki"); }

    // --- MATEMATYKA ATAKU ---
    private void TryPlayerAttack(float cost, float multiplier, string name)
    {
        if (!PlayerStats.UseStamina(cost)) return;
        if (currentEnemy == null) return;

        CharacterStats target = currentEnemy.EnemyStats;

        // 1. Logika Trafienia
        float hitChance = 80f + (PlayerStats.Precision - target.Precision);
        float hitRoll = Random.Range(0f, 100f);
        Debug.Log($"[MATH] Gracz trafienie: Szansa {hitChance}% vs Wylosowano {hitRoll}");

        if (hitRoll > hitChance)
        {
            Debug.Log($"<color=grey>Gracz: {name} atak PUDŁUJE!</color>");
            EndTurn(); return;
        }

        // 2. Logika Uniku
        float dodgeChance = 10f + (target.Agility - PlayerStats.Agility);
        dodgeChance = Mathf.Clamp(dodgeChance, 5f, 50f);
        float dodgeRoll = Random.Range(0f, 100f);
        Debug.Log($"[MATH] Wróg unik: Szansa {dodgeChance}% vs Wylosowano {dodgeRoll}");

        if (dodgeRoll < dodgeChance)
        {
            Debug.Log($"<color=yellow>Gracz: Wróg zrobił UNIK!</color>");
            EndTurn(); return;
        }

        // 3. Obliczenie DMG
        float damage = PlayerStats.Strenght * multiplier;
        Debug.Log($"[MATH] Dmg bazowy: {damage}");

        // 4. Blok
        if (target.isBlocking)
        {
            float reductionPercent = 50f + (target.Agility * 0.5f);
            if (reductionPercent > 80f) reductionPercent = 80f;

            float reductionAmount = damage * (reductionPercent / 100f);
            damage -= reductionAmount;
            Debug.Log($"[MATH] Wróg blokuje! Redukcja: {reductionPercent}% (-{reductionAmount} dmg)");
        }

        target.GetDamage(damage);
        EndTurn();
    }
}