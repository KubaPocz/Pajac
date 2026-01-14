using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour, CharacterController
{
    [Header("Stats")]
    public CharacterStats PlayerStats;

    [Header("Movement")]
    public float moveStep = 120f;      // ile jednostek UI/świata przesunąć za 1 akcję
    public float moveDuration = 0.15f; // czas animacji ruchu
    public float moveStaminaCost = 5f;

    [Header("Combat costs")]
    public float lightCost = 10f;
    public float mediumCost = 20f;
    public float strongCost = 30f;
    public float blockCost = 15f;

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
        if (!CanAct()) return;
        if (!PlayerStats.UseStamina(moveStaminaCost)) return;

        actionLocked = true;
        StartCoroutine(MoveRoutine(Vector3.right * moveStep));
        Report("Ruch w prawo (-STA)");

    }

    public void MoveLeft()
    {
        if (!CanAct()) return;
        if (!PlayerStats.UseStamina(moveStaminaCost)) return;

        actionLocked = true;
        StartCoroutine(MoveRoutine(Vector3.left * moveStep));
        Report("Ruch w lewo (-STA)");

    }

    private IEnumerator MoveRoutine(Vector3 delta)
    {
        Debug.Log($"Gracz: Ruch {(delta.x > 0 ? "w prawo" : "w lewo")}.");

        Vector3 start = transform.position;
        Vector3 end = start + delta;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, moveDuration);
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        transform.position = end;

        actionLocked = false;
        EndTurn();
    }

    public void Sleep()
    {
        if (!CanAct()) return;

        Debug.Log("Gracz: Idzie spać.");
        PlayerStats.RestoreStamina(40);
        Report("Sen (+STA)");
        EndTurn();
    }

    public void Block()
    {
        if (!CanAct()) return;
        if (!PlayerStats.UseStamina(blockCost)) return;

        PlayerStats.isBlocking = true;
        Debug.Log("<color=green>Gracz: Postawa obronna (BLOK).</color>");
        Report("Blok");

        EndTurn();
    }

    public void Dodge()
    {
        // zostawione jako placeholder
    }

    public void AttackLight()
    {
        if (!CanAct()) return;
        TryPlayerAttack(lightCost, 1.0f, "Lekki");
        Report("Atak lekki (-STA)");
    }

    public void AttackMedium()
    {
        if (!CanAct()) return;
        TryPlayerAttack(mediumCost, 1.5f, "Średni");
        Report("Atak średni (-STA)");
    }

    public void AttackStrong()
    {
        if (!CanAct()) return;
        TryPlayerAttack(strongCost, 2.0f, "Ciężki");
        Report("Atak ciężki (-STA)");
    }

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

        // 1. Trafienie
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
            EndTurn();
            return;
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
    private void Report(string label)
    {
        if (BattleManager.Instance != null)
            BattleManager.Instance.SetLastAction(label);
    }

}
