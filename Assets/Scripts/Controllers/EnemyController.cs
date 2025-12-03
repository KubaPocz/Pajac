using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour, CharacterController
{
    [Header("Statystyki")]
    public CharacterStats EnemyStats;
    public CharacterStats TargetStats;
    public Transform TargetTransform;

    [Header("AI Config")]
    public float attackRange = 2.0f;
    public float moveSpeed = 4.0f;

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

    // --- INTERFEJS CHARACTER CONTROLLER ---
    // Ta metoda jest wołana przez BattleManager!
    public void TakeTurn()
    {
        StartCoroutine(AI_Logic());
    }

    // Metody wymagane przez interfejs (AI używa ich wewnątrz swojej logiki)
    public void MoveRight() { }
    public void MoveLeft() { }
    public void Sleep() { EnemyStats.RestoreStamina(40); }
    public void Block()
    {
        if (EnemyStats.UseStamina(15)) EnemyStats.isBlocking = true;
    }
    public void AttackLight() { PerformSpecificAttack(10, 1.0f, "Lekki"); }
    public void AttackMedium() { PerformSpecificAttack(20, 1.5f, "Średni"); }
    public void AttackStrong() { PerformSpecificAttack(30, 2.0f, "Ciężki"); }
    public void Dodge() { } // AI robi uniki pasywnie (matematyka), nie aktywnie

    // --- LOGIKA AI ---

    private IEnumerator AI_Logic()
    {
        EnemyStats.NewTurnRegen(); // +20 Staminy na start tury (wg dokumentacji)
        yield return new WaitForSeconds(1.0f); // Czas na myślenie

        if (EnemyStats.CurrentHealth <= 0 || TargetStats.CurrentHealth <= 0)
        {
            // Koniec gry, nie oddajemy tury
            yield break;
        }

        float dist = Vector3.Distance(transform.position, TargetTransform.position);

        // DRZEWO DECYZYJNE WG KOSZTÓW I ZASAD
        // 1. Mało staminy (<20) -> Śpij (Odnawia 40)
        if (EnemyStats.CurrentStamina < 20)
        {
            Sleep();
            Debug.Log("AI: Odpoczywa.");
        }
        // 2. W zasięgu ataku?
        else if (dist <= attackRange + 0.2f)
        {
            // Decyzja jaki atak
            float st = EnemyStats.CurrentStamina;
            if (st >= 30 && Random.value > 0.6f) AttackStrong();
            else if (st >= 20 && Random.value > 0.4f) AttackMedium();
            else AttackLight();
        }
        // 3. Za daleko? Podejdź (Koszt 5)
        else
        {
            if (EnemyStats.UseStamina(5))
            {
                yield return StartCoroutine(MoveRoutine(dist));
            }
            else
            {
                Sleep(); // Brak siły na ruch -> Śpij
            }
        }

        yield return new WaitForSeconds(0.5f);

        // KONIEC TURY - Oddajemy sterowanie do Gracza przez BattleManager
        if (BattleManager.Instance != null)
            BattleManager.Instance.StartPlayerTurn();
    }

    // --- MATEMATYKA WALKI (Z DOKUMENTACJI) ---

    private void PerformSpecificAttack(float staminaCost, float dmgMultiplier, string name)
    {
        // 1. Sprawdź staminę
        if (!EnemyStats.UseStamina(staminaCost))
        {
            Sleep(); // Jak chciał zaatakować a nie ma siły, traci turę na sen
            return;
        }

        // 2. Szansa trafienia: 80% + (Prec Atakującego - Prec Obrońcy)
        float hitChance = 80f + (EnemyStats.Precision - TargetStats.Precision);
        if (Random.Range(0f, 100f) > hitChance)
        {
            Debug.Log($"AI: {name} atak PUDŁUJE! (Szansa: {hitChance}%)");
            return;
        }

        // 3. Szansa na unik: 10% + (Agi Obrońcy - Agi Atakującego), min 5 max 50
        float dodgeChance = 10f + (TargetStats.Agility - EnemyStats.Agility);
        dodgeChance = Mathf.Clamp(dodgeChance, 5f, 50f);
        if (Random.Range(0f, 100f) < dodgeChance)
        {
            Debug.Log($"AI: Gracz zrobił UNIK! (Szansa: {dodgeChance}%)");
            return;
        }

        // 4. Obliczenie obrażeń: Siła * Mnożnik
        float damage = EnemyStats.Strenght * dmgMultiplier;

        // 5. Redukcja blokiem: 50% + (Agi * 0.5%), max 80%
        if (TargetStats.isBlocking)
        {
            float reduction = 50f + (TargetStats.Agility * 0.5f);
            if (reduction > 80f) reduction = 80f;

            damage -= damage * (reduction / 100f);
            Debug.Log($"AI: Gracz zablokował atak. Zredukowano o {reduction}%.");
        }

        TargetStats.GetDamage(damage);
    }

    private IEnumerator MoveRoutine(float currentDist)
    {
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
    }
}