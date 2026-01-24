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

    [Header("Projectiles")]
    [Tooltip("Pocisk małpy (banan)")]
    public GameObject bananaPrefab;
    public Transform bananaSpawnPoint;

    [Tooltip("Pocisk słonia (piłka)")]
    public GameObject ballPrefab;
    public Transform ballSpawnPoint;

    [Tooltip("Pocisk klauna")]
    public GameObject clownBallPrefab;
    public Transform clownBallSpawnPoint;

    [Tooltip("Pocisk bossa")]
    public GameObject bossProjectilePrefab;
    public Transform bossProjectileSpawnPoint;

    // Statystyki walki (logowanie)
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
        // aktualny wróg z GameManagera
        EnemyStats = GameManager.Instance.Enemies[GameManager.Instance.CurrentEnemy];
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
            if (EnemyStats.CurrentHealth <= 0) Debug.Log(" → Wróg POKONANY!");
            if (TargetStats.CurrentHealth <= 0) Debug.Log(" → Gracz POKONANY!");
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
                Debug.Log(" → WYBÓR: Lekki atak (domyślnie)");
                PerformAttack(10, 1.0f, "LEKKI ATAK");
                lightAttacksCount++;
            }
        }
        else
        {
            if (EnemyStats.UseStamina(5))
            {
                totalMovesAttempted++;
                Debug.Log("\n[RUCH] WRÓG RUSZA SIĘ DO PRZODU!");
                Debug.Log($" Koszt: 5 STA | Pozostało: {EnemyStats.CurrentStamina}");
                yield return StartCoroutine(MoveRoutine(dist));
            }
            else
            {
                Debug.Log($"[RUCH] BRAK STAMINY NA RUCH! Wymagane: 5 | Dostępne: {EnemyStats.CurrentStamina}");
                Debug.Log(" → AKCJA ZASTĘPCZA: SEN");
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

        // 1. Trafienie
        float hitChance = 80f + (EnemyStats.Precision - TargetStats.Precision);
        float hitRoll = Random.Range(0f, 100f);
        bool isHit = (hitRoll <= hitChance);
        if (!isHit)
        {
            totalMisses++;
            Debug.Log($"[ATAK] WRÓG PUDŁUJE (szansa {hitChance}%, wylosowano {hitRoll})");
            return;
        }

        totalHits++;

        // 2. Unik gracza
        float dodgeChance = 10f + (TargetStats.Agility - EnemyStats.Agility);
        dodgeChance = Mathf.Clamp(dodgeChance, 5f, 50f);
        float dodgeRoll = Random.Range(0f, 100f);
        bool isDodged = (dodgeRoll < dodgeChance);
        if (isDodged)
        {
            totalDodgesAgainstPlayer++;
            Debug.Log($"[ATAK] GRACZ ZROBIŁ UNIK (szansa uniku {dodgeChance}%, wylosowano {dodgeRoll})");
            Debug.Log($"[STATYSTYKA] Udane uniki gracza w całej walce: {totalDodgesAgainstPlayer}");
            return;
        }

        // 3. Wybór rodzaju pocisku na podstawie CharacterName
        bool isMonkey = EnemyStats != null && EnemyStats.CharacterName == "Monkey";
        bool isElephant = EnemyStats != null && EnemyStats.CharacterName == "Elephant";
        bool isClown = EnemyStats != null && EnemyStats.CharacterName == "Clown";
        bool isBoss = EnemyStats != null && EnemyStats.CharacterName == "Boss";

        if (isMonkey)
        {
            FireBanana();
        }
        else if (isElephant)
        {
            FireBall();
        }
        else if (isClown)
        {
            FireClownBall();
        }
        else if (isBoss)
        {
            FireBossProjectile();
        }

        // 4. Obrażenia
        float baseDamage = EnemyStats.Strenght * multiplier;
        float finalDamage = baseDamage;

        if (TargetStats.isBlocking)
        {
            float reductionPercent = 50f + (TargetStats.Agility * 0.5f);
            if (reductionPercent > 80f) reductionPercent = 80f;

            float reductionAmount = baseDamage * (reductionPercent / 100f);
            finalDamage -= reductionAmount;
            totalBlocksByPlayer++;

            Debug.Log($"[BLOK] Gracz BLOKUJE. Redukcja {reductionPercent}% (-{reductionAmount} dmg).");
            Debug.Log($"[STATYSTYKA] Łączna liczba bloków gracza: {totalBlocksByPlayer}");
        }
        else
        {
            Debug.Log("[BLOK] Gracz NIE BLOKUJE");
        }

        TargetStats.GetDamage(finalDamage);
        totalDamageDealt += finalDamage;

        Debug.Log($"[OBRAŻENIA] Wróg zadaje {finalDamage} dmg (bazowo {baseDamage}).");
        Debug.Log($"[STATYSTYKA] Łączne obrażenia zadane przez wroga: {totalDamageDealt}");
    }

    // --- POCISKI ---

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

    private void FireBall()
    {
        if (ballPrefab == null || TargetTransform == null) return;

        Vector3 startPos = ballSpawnPoint != null
            ? ballSpawnPoint.position
            : transform.position;

        GameObject ball = Instantiate(ballPrefab, startPos, Quaternion.identity, transform);

        BallProjectile proj = ball.GetComponent<BallProjectile>();
        if (proj != null)
        {
            proj.Init(TargetTransform);
        }
        else
        {
            Debug.LogWarning("BallProjectile nie znaleziony na prefabie piłki.");
        }
    }

    private void FireClownBall()
    {
        if (clownBallPrefab == null || TargetTransform == null) return;

        Vector3 startPos = clownBallSpawnPoint != null
            ? clownBallSpawnPoint.position
            : transform.position;

        GameObject ball = Instantiate(clownBallPrefab, startPos, Quaternion.identity, transform);

        ClownBallProjectile proj = ball.GetComponent<ClownBallProjectile>();
        if (proj != null)
        {
            proj.Init(TargetTransform);
        }
        else
        {
            Debug.LogWarning("ClownBallProjectile nie znaleziony na prefabie klauna.");
        }
    }

    private void FireBossProjectile()
    {
        if (bossProjectilePrefab == null || TargetTransform == null) return;

        Vector3 startPos = bossProjectileSpawnPoint != null
            ? bossProjectileSpawnPoint.position
            : transform.position;

        GameObject projObj = Instantiate(
            bossProjectilePrefab,
            startPos,
            Quaternion.identity,
            transform
        );

        BossProjectile proj = projObj.GetComponent<BossProjectile>();
        if (proj != null)
        {
            proj.Init(TargetTransform);
        }
        else
        {
            Debug.LogWarning("BossProjectile nie znaleziony na prefabie bossa.");
        }
    }

    // --- RUCH / SEN / KONIEC TURY ---

    private IEnumerator MoveRoutine(float currentDist)
    {
        BattleManager.Instance?.SetLastAction("Ruch do gracza (-STA)");

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

        float newDist = Vector3.Distance(transform.position, TargetTransform.position);
        Debug.Log($"[RUCH] Nowy dystans: {newDist:F2}");
    }

    private void PerformSleep()
    {
        BattleManager.Instance?.SetLastAction("Sen (+STA)");
        totalSleepTurns++;
        EnemyStats.RestoreStamina(40);
        Debug.Log($"[SEN] Wróg regeneruje 40 STA. Łączna liczba tur snu: {totalSleepTurns}");
    }

    private void EndTurn()
    {
        if (BattleManager.Instance != null)
            BattleManager.Instance.EndEnemyTurn();
    }
}
