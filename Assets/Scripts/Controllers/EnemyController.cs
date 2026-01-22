using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour, CharacterController
{
    public CharacterStats EnemyStats;
    public CharacterStats TargetStats;
    public Transform TargetTransform;

    [Header("AI Settings")]
    public float attackRange = 2.0f;
    public float moveSpeed = 4.0f;

    [Header("Banana projectile (dla Monkey)")]
    public GameObject bananaPrefab;
    public Transform bananaSpawnPoint;

    // === STATYSTYKI AI DO LOGOWANIA ===
    private int turnCount = 0;
    private int totalAttackAttempts = 0;
    private int totalHits = 0;
    private int totalMisses = 0;
    private int totalDodgesAgainstPlayer = 0;
    private int totalBlocksByPlayer = 0;
    private int totalMovesAttempted = 0;
    private int totalSleepTurns = 0;
    private float totalDamageDealt = 0f;
    private int lightAttacksCount = 0;
    private int mediumAttacksCount = 0;
    private int heavyAttacksCount = 0;

    private void Start()
    {
        if (EnemyStats == null && GameManager.Instance != null)
        {
            EnemyStats = GameManager.Instance.Enemies[GameManager.Instance.CurrentEnemy];
        }

        if (EnemyStats != null)
            EnemyStats.Initialize();

        if (TargetStats != null)
            TargetStats.Initialize();

        if (TargetTransform == null)
        {
            var p = GameObject.Find("Player");
            if (p != null) TargetTransform = p.transform;
        }
    }

    public void Move()
    {
        StartCoroutine(AI_Logic());
    }

    private IEnumerator AI_Logic()
    {
        turnCount++;
        EnemyStats.NewTurnRegen();

        yield return new WaitForSeconds(1.0f);

        if (EnemyStats.CurrentHealth <= 0 || TargetStats.CurrentHealth <= 0)
        {
            if (EnemyStats.CurrentHealth <= 0) Debug.Log("Wróg POKONANY!");
            if (TargetStats.CurrentHealth <= 0) Debug.Log("Gracz POKONANY!");
            EndTurn();
            yield break;
        }

        float dist = Vector3.Distance(transform.position, TargetTransform.position);

        if (EnemyStats.CurrentStamina < 20)
        {
            PerformSleep();
        }
        else if (dist <= attackRange + 0.5f)
        {
            float stamAvailable = EnemyStats.CurrentStamina;

            if (stamAvailable >= 30 && Random.value > 0.6f)
            {
                PerformAttack(30, 2.0f, "CIĘŻKI ATAK");
                heavyAttacksCount++;
            }
            else if (stamAvailable >= 20 && Random.value > 0.4f)
            {
                PerformAttack(20, 1.5f, "ŚREDNI ATAK");
                mediumAttacksCount++;
            }
            else
            {
                PerformAttack(10, 1.0f, "LEKKI ATAK");
                lightAttacksCount++;
            }
        }
        else
        {
            if (EnemyStats.UseStamina(5))
            {
                totalMovesAttempted++;
                yield return StartCoroutine(MoveRoutine(dist));
            }
            else
            {
                PerformSleep();
            }
        }

        yield return new WaitForSeconds(0.5f);
        EndTurn();
    }

    private void PerformAttack(float cost, float multiplier, string attackName)
    {
        BattleManager.Instance?.SetLastAction(attackName);
        totalAttackAttempts++;

        if (!EnemyStats.UseStamina(cost))
        {
            PerformSleep();
            return;
        }

        float hitChance = 80f + (EnemyStats.Precision - TargetStats.Precision);
        float hitRoll = Random.Range(0f, 100f);
        bool isHit = (hitRoll <= hitChance);
        if (!isHit)
        {
            totalMisses++;
            return;
        }

        totalHits++;

        float dodgeChance = 10f + (TargetStats.Agility - EnemyStats.Agility);
        dodgeChance = Mathf.Clamp(dodgeChance, 5f, 50f);
        float dodgeRoll = Random.Range(0f, 100f);
        bool isDodged = (dodgeRoll < dodgeChance);
        if (isDodged)
        {
            totalDodgesAgainstPlayer++;
            return;
        }

        if (EnemyStats.CharacterName == "Monkey" || EnemyStats.CharacterName == "Małpa")
        {
            FireBanana();
        }

        float baseDamage = EnemyStats.Strenght * multiplier;
        float finalDamage = baseDamage;

        if (TargetStats.isBlocking)
        {
            float reductionPercent = 50f + (TargetStats.Agility * 0.5f);
            if (reductionPercent > 80f) reductionPercent = 80f;

            float reductionAmount = baseDamage * (reductionPercent / 100f);
            finalDamage -= reductionAmount;
            totalBlocksByPlayer++;
        }

        TargetStats.GetDamage(finalDamage);
        totalDamageDealt += finalDamage;

        // ODRZUT GRACZA robi BananaProjectile, przeciwnik odskakuje tylko przy ciosie od gracza (w PlayerController).
    }

    private void FireBanana()
    {
        if (bananaPrefab == null || TargetTransform == null) return;

        Vector3 startPos = bananaSpawnPoint != null
            ? bananaSpawnPoint.position
            : transform.position;

        GameObject banana = Instantiate(bananaPrefab, startPos, Quaternion.identity, transform);

        BananaProjectile proj = banana.GetComponent<BananaProjectile>();
        if (proj != null)
        {
            proj.Init(TargetTransform);
        }
        else
        {
            Debug.LogWarning("BananaProjectile nie znaleziony na prefabie banana.");
        }
    }

    private IEnumerator MoveRoutine(float currentDist)
    {
        BattleManager.Instance?.SetLastAction("Ruch do gracza (-STA)");

        Vector3 start = transform.position;
        Vector3 target = TargetTransform.position;
        Vector3 dir = (target - start).normalized;
        float travel = Mathf.Min(currentDist - attackRange, moveSpeed);

        if (travel > 0.1f)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / 0.3f;
                transform.position = Vector3.Lerp(start, start + dir * travel, t);
                yield return null;
            }

            transform.position = start + dir * travel;
        }
    }

    private void PerformSleep()
    {
        BattleManager.Instance?.SetLastAction("Sen (+STA)");
        totalSleepTurns++;
        EnemyStats.RestoreStamina(40);
    }

    private void EndTurn()
    {
        if (BattleManager.Instance != null)
            BattleManager.Instance.EndEnemyTurn();
    }
}
