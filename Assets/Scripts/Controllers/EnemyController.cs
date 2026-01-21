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
        if (EnemyStats != null) EnemyStats.Initialize();
        if (TargetStats != null) TargetStats.Initialize();

        // Auto-find gracza
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
            if (EnemyStats.CurrentHealth <= 0) Debug.Log("  → Wróg POKONANY!");
            if (TargetStats.CurrentHealth <= 0) Debug.Log("  → Gracz POKONANY!");
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
                Debug.Log("         → WYBÓR: Lekki atak (domyślnie)");
                PerformAttack(10, 1.0f, "LEKKI ATAK");
                lightAttacksCount++;
            }
        }
        else
        {

            if (EnemyStats.UseStamina(5))
            {
                totalMovesAttempted++;
                Debug.Log($"\n[RUCH] 🏃 WRÓG RUSZA SIĘ DO PRZODU!");
                Debug.Log($"       Koszt: 5 STA | Pozostało: {EnemyStats.CurrentStamina}");
                yield return StartCoroutine(MoveRoutine(dist));
            }
            else
            {
                Debug.Log($"[RUCH] ❌ BRAK STAMINY NA RUCH!");
                Debug.Log($"         Wymagane: 5 | Dostępne: {EnemyStats.CurrentStamina}");
                Debug.Log("         → AKCJA ZASTĘPCZA: SEN");
                PerformSleep();
            }
        }

        yield return new WaitForSeconds(0.5f);
        EndTurn();
    }

    private void PerformAttack(float cost, float multiplier, string attackName)
    {
        BattleManager.Instance?.SetLastAction(attackName); // np. "LEKKI ATAK"

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
            Debug.Log($"\n[STATYSTYKA] Udane uniki gracza w całej walce: {totalDodgesAgainstPlayer}");
            return;
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
        else
        {
            Debug.Log($"[BLOK] Gracz NIE BLOKUJE");
        }
        TargetStats.GetDamage(finalDamage);
        totalDamageDealt += finalDamage;
    }

    private IEnumerator MoveRoutine(float currentDist)
    {
        BattleManager.Instance?.SetLastAction("Ruch do gracza (-STA)");

        Debug.Log($"\n[RUCH] ─────────────────────────────────────────────");

        Vector3 start = transform.position;
        Vector3 target = TargetTransform.position;
        Vector3 dir = (target - start).normalized;
        float travel = Mathf.Min(currentDist - attackRange, moveSpeed);


        if (travel > 0.1f)
        {
            float t = 0;
            while (t < 1f)
            {
                transform.position = Vector3.Lerp(start, start + dir * travel, t);
                t += Time.deltaTime;
                yield return null;
            }
            transform.position = start + dir * travel;
        }

        Debug.Log($"  Nowa pozycja: ({transform.position.x:F2}, {transform.position.y:F2})");
        float newDist = Vector3.Distance(transform.position, TargetTransform.position);
        Debug.Log($"  Nowy dystans: {newDist:F2}m");
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
