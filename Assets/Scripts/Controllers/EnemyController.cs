using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("PLIKI STATYSTYK")]
    public CharacterStats EnemyStats;
    public CharacterStats TargetStats;

    [Header("CEL")]
    public Transform TargetTransform;

    [Header("Ustawienia AI")]
    public float attackRange = 2.0f;
    public float moveSpeed = 4.0f;
    public float actionDelay = 1.0f;

    private void Start()
    {
        if (EnemyStats != null) EnemyStats.Initialize();
        if (TargetStats != null) TargetStats.Initialize();

        if (TargetTransform == null)
        {
            GameObject player = GameObject.Find("Player");
            if (player != null) TargetTransform = player.transform;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T)) StartTurn();
    }

    public void StartTurn()
    {
        StartCoroutine(MakeDecision());
    }

    private IEnumerator MakeDecision()
    {
        if (TargetTransform == null || TargetStats == null || EnemyStats == null) yield break;

        // Reset flagi bloku (bez dodawania staminy)
        EnemyStats.NewTurnRegen();

        yield return new WaitForSeconds(actionDelay);

        if (EnemyStats.CurrentHealth <= 0 || TargetStats.CurrentHealth <= 0)
        {
            Debug.Log("Walka zakończona.");
            yield break;
        }

        float dist = Vector3.Distance(transform.position, TargetTransform.position);
        float currentStamina = EnemyStats.CurrentStamina;

        // --- NOWA LOGIKA DECYZYJNA ---

        // Sytuacja 1: Jestem w zasięgu ataku
        if (dist <= attackRange + 0.2f)
        {
            // Czy mam siłę na chociaż najsłabszy atak (koszt 10)?
            if (currentStamina >= 10)
            {
                PerformAttack();
            }
            else
            {
                // Jestem blisko, ale nie mam siły machnąć mieczem -> ŚPIJ
                Debug.Log("AI: Jest blisko, ale brak sił na atak -> Śpi.");
                Sleep();
            }
        }
        // Sytuacja 2: Jestem za daleko, muszę podejść
        else
        {
            // Czy mam siłę na ruch (koszt 5)?
            if (currentStamina >= 5)
            {
                // Jeśli mam dużo życia, to po prostu idę
                // Ale jeśli mam mało życia (<30%) i mało staminy, to może lepiej spać z daleka?
                // Tutaj zrobimy prosto: jak stać nas na ruch, to idziemy.
                yield return StartCoroutine(MoveTowardsPlayerRoutine(dist));
            }
            else
            {
                // Nie mam siły nawet chodzić -> ŚPIJ
                Debug.Log("AI: Za daleko i brak sił na ruch -> Śpi.");
                Sleep();
            }
        }

        Debug.Log("--> KONIEC TURY WROGA.");
    }

    private IEnumerator MoveTowardsPlayerRoutine(float currentDistance)
    {
        // Próba zużycia 5 staminy
        if (!EnemyStats.UseStamina(5))
        {
            // To zabezpieczenie, teoretycznie sprawdzone wyżej, ale dla pewności:
            Sleep();
            yield break;
        }

        Debug.Log("AI: Wykonuje ruch.");

        Vector3 start = transform.position;
        Vector3 target = TargetTransform.position;
        Vector3 dir = (target - start).normalized;
        float distToTravel = currentDistance - attackRange;
        float step = Mathf.Min(distToTravel, moveSpeed);

        if (step > 0.1f)
        {
            float timer = 0;
            while (timer < 1.0f)
            {
                transform.position = Vector3.Lerp(start, start + (dir * step), timer);
                timer += Time.deltaTime;
                yield return null;
            }
            transform.position = start + (dir * step);
        }
    }

    private void PerformAttack()
    {
        float st = EnemyStats.CurrentStamina;
        float roll = Random.value;

        // AI decyduje jaki atak wykonać na podstawie dostępnej staminy
        if (st >= 30 && roll > 0.6f) AttackHeavy();
        else if (st >= 20 && roll > 0.3f) AttackMedium();
        else AttackLight();
    }

    public void Sleep()
    {
        EnemyStats.RestoreStamina(40);
    }

    public void Block() // Opcjonalnie AI może blokować, jeśli dodasz warunek w logice
    {
        if (EnemyStats.UseStamina(15))
        {
            EnemyStats.isBlocking = true;
            Debug.Log("AI: Podnosi gardę.");
        }
    }

    public void AttackLight()
    {
        if (EnemyStats.UseStamina(10))
            TryDealDamage(EnemyStats.Strenght * 1.0f, "Lekki Atak");
    }

    public void AttackMedium()
    {
        if (EnemyStats.UseStamina(20))
            TryDealDamage(EnemyStats.Strenght * 1.5f, "Średni Atak");
    }

    public void AttackHeavy()
    {
        if (EnemyStats.UseStamina(30))
            TryDealDamage(EnemyStats.Strenght * 2.0f, "Ciężki Atak");
    }

    private void TryDealDamage(float baseDamage, string attackName)
    {
        float hitChance = 80f + (EnemyStats.Precision - TargetStats.Precision);
        if (Random.Range(0f, 100f) > hitChance)
        {
            Debug.Log($"AI: {attackName} PUDŁUJE!");
            return;
        }

        float dodgeChance = 10f + (TargetStats.Agility - EnemyStats.Agility);
        dodgeChance = Mathf.Clamp(dodgeChance, 5f, 50f);
        if (Random.Range(0f, 100f) < dodgeChance)
        {
            Debug.Log($"AI: {attackName} UNIKNIĘTY przez gracza!");
            return;
        }

        float finalDamage = baseDamage;
        if (TargetStats.isBlocking)
        {
            float reduction = 50f + (TargetStats.Agility * 0.5f);
            if (reduction > 80f) reduction = 80f;
            finalDamage -= finalDamage * (reduction / 100f);
            Debug.Log($"AI: Atak częściowo zablokowany (-{reduction}%).");
        }

        TargetStats.GetDamage(finalDamage);
    }
}