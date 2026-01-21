using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour, CharacterController
{
    public CharacterStats PlayerStats;
    public float moveSpeed = 5f;
    public TMP_Text info;
    private bool isMyTurn = false;
    private EnemyController currentEnemy;

    private void Start()
    {
        if (GameManager.Instance != null) PlayerStats = GameManager.Instance.Player;
        currentEnemy = FindObjectOfType<EnemyController>();
    }

    public void Move()
    {
        isMyTurn = true;
        PlayerStats.NewTurnRegen();
        Debug.Log("\n<color=blue>--- TURA GRACZA ---</color>");
    }

    private void Update()
    {
        if (!isMyTurn) return;

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
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.LogBattleState();
            BattleManager.Instance.StartEnemyTurn();
        }
    }

    // --- AKCJE ---
    public void MoveRight()
    {
        if (PlayerStats.UseStamina(5))
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime * 20f);
            Debug.Log("Gracz: Ruch w prawo.");
            ShowInfo("Moving right");
            EndTurn();
        }
    }
    public void MoveLeft()
    {
        if (PlayerStats.UseStamina(5))
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime * 20f);
            Debug.Log("Gracz: Ruch w lewo.");
            ShowInfo("Moving left");
            EndTurn();
        }
    }
    public void Sleep()
    {
        Debug.Log("Gracz: Idzie spać.");
        ShowInfo("Sleeping...");

        PlayerStats.RestoreStamina(60);
        EndTurn();
    }
    public void Block()
    {
        if (PlayerStats.UseStamina(15))
        {
            PlayerStats.isBlocking = true;
            Debug.Log("<color=green>Gracz: Postawa obronna (BLOK).</color>");
            ShowInfo("Defending...");
            EndTurn();
        }
    }
    public void Dodge() { }

    public void AttackLight() { TryPlayerAttack(10, 1.0f, "Light"); }
    public void AttackMedium() { TryPlayerAttack(20, 1.5f, "Medium"); }
    public void AttackStrong() { TryPlayerAttack(30, 2.0f, "Strong"); }

    // --- MATEMATYKA ATAKU (Tu są logi o przeciwniku) ---
    private void TryPlayerAttack(float cost, float multiplier, string name)
    {
        if (!PlayerStats.UseStamina(cost)) return;
        if (currentEnemy == null) return;

        CharacterStats target = currentEnemy.EnemyStats;
        Debug.Log($"<color=green>Gracz: Wykonuje {name} atak!</color>");
        ShowInfo($"{name} attack");
        // 1. Czy trafiłeś?
        float hitChance = 80f + (PlayerStats.Precision - target.Precision);
        float hitRoll = Random.Range(0f, 100f);

        if (hitRoll > hitChance)
        {
            Debug.Log($"<color=grey>... PUDŁO! (Szansa: {hitChance}%, Wylosowano: {hitRoll})</color>");
            EndTurn(); return;
        }

        // 2. CZY PRZECIWNIK ROBI UNIK?
        float dodgeChance = 10f + (target.Agility - PlayerStats.Agility);
        dodgeChance = Mathf.Clamp(dodgeChance, 5f, 50f);
        float dodgeRoll = Random.Range(0f, 100f);

        if (dodgeRoll < dodgeChance)
        {
            Debug.Log($"<color=orange>... PRZECIWNIK ZROBIŁ UNIK! (Szansa uniku: {dodgeChance}%)</color>");
            ShowInfo("Enemy dodged");
            EndTurn(); return;
        }

        // 3. Obliczenie DMG
        float damage = PlayerStats.Strenght * multiplier;

        // 4. CZY PRZECIWNIK BLOKUJE?
        if (target.isBlocking)
        {
            float reductionPercent = 50f + (target.Agility * 0.5f);
            if (reductionPercent > 80f) reductionPercent = 80f;

            float reductionAmount = damage * (reductionPercent / 100f);
            damage -= reductionAmount;

            Debug.Log($"<color=orange>... PRZECIWNIK BLOKUJE! Zredukował obrażenia o {reductionPercent}% (-{reductionAmount} dmg).</color>");
        }

        Debug.Log($"<color=red>... SUKCES! Przeciwnik otrzymuje {damage} obrażeń.</color>");
        target.GetDamage(damage);
        EndTurn();
    }
    private IEnumerator ShowInfo(string infoMessage)
    {
        info.text = infoMessage;
        yield return new WaitForSeconds(1f);
        info.text = "";
    }
}