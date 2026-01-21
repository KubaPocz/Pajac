using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour, CharacterController
{
    [Header("Stats")]
    public CharacterStats PlayerStats;
    public float moveSpeed = 5f;
    public TMP_Text info;
    private bool isMyTurn = false;
    private bool actionLocked = false;
    private EnemyController currentEnemy;

    private void Start()
    {
        if (GameManager.Instance != null)
            PlayerStats = GameManager.Instance.Player;

        if (PlayerStats != null)
            PlayerStats.Initialize();

        currentEnemy = FindObjectOfType<EnemyController>();
    }

    // Start tury gracza (BattleManager.StartPlayerTurn -> player.Move())
    public void Move()
    {
        isMyTurn = true;
        actionLocked = false;

        if (PlayerStats != null)
            PlayerStats.NewTurnRegen();

        Debug.Log("\n<color=blue>--- TURA GRACZA ---</color>");
    }

    private void Update()
    {
        if (!isMyTurn || actionLocked) return;

        // opcjonalnie: skróty klawiszowe do testów
        if (Input.GetKeyDown(KeyCode.K)) EndTurn();

        if (Input.GetKeyDown(KeyCode.A)) AttackLight();
        if (Input.GetKeyDown(KeyCode.S)) AttackMedium();
        if (Input.GetKeyDown(KeyCode.D)) AttackStrong();

        if (Input.GetKeyDown(KeyCode.Space)) Sleep();
        if (Input.GetKeyDown(KeyCode.B)) Block();

        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveRight();
    }

    private bool CanAct()
    {
        if (!isMyTurn) return false;
        if (actionLocked) return false;

        if (PlayerStats == null)
        {
            Debug.LogError("[PlayerController] PlayerStats is NULL.");
            return false;
        }

        if (currentEnemy == null) currentEnemy = FindObjectOfType<EnemyController>();
        if (currentEnemy == null)
        {
            Debug.LogError("[PlayerController] currentEnemy is NULL.");
            return false;
        }

        return true;
    }

    private void EndTurn()
    {
        isMyTurn = false;
        actionLocked = false;

        if (BattleManager.Instance != null)
            BattleManager.Instance.EndPlayerTurn();
    }

    // --- AKCJE (UI + keyboard) ---

    public void MoveRight()
    {
        if (PlayerStats.UseStamina(5))
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime * 200f);
            Debug.Log("Gracz: Ruch w prawo.");
            ShowInfo("Moving right");
            EndTurn();
        }
    }

    public void MoveLeft()
    {
        if (PlayerStats.UseStamina(5))
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime * 200f);
            Debug.Log("Gracz: Ruch w prawo.");
            ShowInfo("Moving right");
            EndTurn();
        }
    }

    public void Sleep()
    {
        if (!CanAct()) return;

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

    public void AttackLight() { TryPlayerAttack(10, 1.0f, "Light"); }
    public void AttackMedium() { TryPlayerAttack(20, 1.5f, "Medium"); }
    public void AttackStrong() { TryPlayerAttack(30, 2.0f, "Strong"); }

    // --- MATEMATYKA ATAKU ---
    private void TryPlayerAttack(float cost, float multiplier, string name)
    {
        if (!PlayerStats.UseStamina(cost)) return;

        CharacterStats target = currentEnemy.EnemyStats;
        if (target == null)
        {
            Debug.LogError("[PlayerController] EnemyStats is NULL.");
            return;
        }

        Debug.Log($"<color=green>Gracz: Wykonuje {name} atak!</color>");
        ShowInfo($"{name} attack");
        // 1. Czy trafiłeś?
        float hitChance = 80f + (PlayerStats.Precision - target.Precision);
        float hitRoll = Random.Range(0f, 100f);

        if (hitRoll > hitChance)
        {
            Debug.Log($"<color=grey>... PUDŁO! (Szansa: {hitChance}%, Wylosowano: {hitRoll})</color>");
            EndTurn();
            return;
        }

        // 2. Unik celu
        float dodgeChance = 10f + (target.Agility - PlayerStats.Agility);
        dodgeChance = Mathf.Clamp(dodgeChance, 5f, 50f);
        float dodgeRoll = Random.Range(0f, 100f);

        if (dodgeRoll < dodgeChance)
        {
            Debug.Log($"<color=orange>... PRZECIWNIK ZROBIŁ UNIK! (Szansa uniku: {dodgeChance}%)</color>");
            ShowInfo("Enemy dodged");
            EndTurn(); return;
        }

        // 3. DMG
        float damage = PlayerStats.Strenght * multiplier;

        // 4. Blok celu
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
