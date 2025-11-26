using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    [Header("PLIKI STATYSTYK (Przeciągnij pliki .asset)")]
    public CharacterStats EnemyStats;   // Plik Clowna
    public CharacterStats TargetStats;  // Plik Gracza

    [Header("CEL FIZYCZNY (Przeciągnij obiekt ze sceny)")]
    public Transform TargetTransform;   // Obiekt Player ze sceny

    [Header("Ustawienia AI")]
    public float attackRange = 2.0f; // Jak blisko podejdzie (np. 2 metry). Zwiększ to, jeśli nadal wchodzi w gracza!
    public float moveSpeed = 4.0f;   // Ile metrów może przejść w jednej turze
    public float actionDelay = 1.0f; // Czas namysłu

    private void Start()
    {
        // 1. Inicjalizacja statystyk na start (HP na maksa)
        if (EnemyStats != null)
        {
            EnemyStats.CurrentHealth = EnemyStats.MaxHealth;
            EnemyStats.CurrentStamina = EnemyStats.MaxStamina;
        }

        if (TargetStats != null)
        {
            TargetStats.CurrentHealth = TargetStats.MaxHealth;
            TargetStats.CurrentStamina = TargetStats.MaxStamina;
        }

        // 2. Szukanie gracza z automatu (zabezpieczenie)
        if (TargetTransform == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null) TargetTransform = playerObj.transform;
        }
    }

    private void Update()
    {
        // Testowanie klawiszem T
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartTurn();
        }
    }

    public void StartTurn()
    {
        StartCoroutine(MakeDecision());
    }

    private IEnumerator MakeDecision()
    {
        if (TargetTransform == null || TargetStats == null || EnemyStats == null)
        {
            Debug.LogError("AI: BŁĄD! Nie przypisano statystyk lub celu w Inspektorze.");
            yield break;
        }

        yield return new WaitForSeconds(actionDelay);

        if (EnemyStats.CurrentHealth <= 0 || TargetStats.CurrentHealth <= 0)
        {
            Debug.Log("Koniec walki.");
            yield break;
        }

        // Obliczamy dystans do gracza
        float distanceToPlayer = Vector3.Distance(transform.position, TargetTransform.position);

        // --- DRZEWO DECYZYJNE ---

        // 1. Czy jesteśmy wystarczająco blisko, żeby zaatakować?
        // (Dodajemy mały margines błędu 0.2f, żeby nie musiał stać co do milimetra)
        if (distanceToPlayer <= attackRange + 0.2f)
        {
            // Jesteśmy blisko -> ATAKUJEMY
            Debug.Log("AI: Jestem w zasięgu, atakuję!");
            PerformAttack();
        }
        else
        {
            // Jesteśmy za daleko -> PODCHODZIMY
            // Ale tylko jeśli mamy siłę (opcjonalne)
            if (EnemyStats.CurrentStamina > 10)
            {
                yield return StartCoroutine(MoveTowardsPlayerRoutine(distanceToPlayer));
            }
            else
            {
                Sleep(); // Jak nie ma siły biegać, to śpi
            }
        }

        Debug.Log("--> KONIEC TURY PRZECIWNIKA.");
    }

    private IEnumerator MoveTowardsPlayerRoutine(float currentDistance)
    {
        Debug.Log("AI: Podchodzę do gracza...");

        Vector3 startPos = transform.position;
        Vector3 targetPos = TargetTransform.position;

        // Obliczamy kierunek do gracza
        Vector3 direction = (targetPos - startPos).normalized;

        // Kluczowa matematyka:
        // Chcemy stanąć w odległości 'attackRange' OD gracza.
        // Czyli musimy pokonać dystans = (ObecnyDystans - ZasięgAtaku).
        float distanceNeededToTravel = currentDistance - attackRange;

        // Ale nie możemy przejść więcej niż 'moveSpeed' w jednej turze.
        // Wybieramy mniejszą wartość: albo tyle ile brakuje, albo nasz limit ruchu.
        float actualStep = Mathf.Min(distanceNeededToTravel, moveSpeed);

        // Jeśli wyliczony ruch jest minimalny (jesteśmy prawie na miejscu), nie ruszamy się
        if (actualStep <= 0.1f)
        {
            yield break;
        }

        // Wyznaczamy punkt końcowy TEJ tury
        Vector3 finalDestination = startPos + (direction * actualStep);

        // Płynny ruch
        float timer = 0;
        float moveDuration = 1.0f; // Ruch trwa 1 sekundę

        while (timer < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, finalDestination, timer / moveDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        transform.position = finalDestination; // Dociągamy do punktu końcowego
    }

    private void PerformAttack()
    {
        float roll = Random.value;
        float stamina = EnemyStats.CurrentStamina;

        // Prosta logika wyboru ataku
        if (stamina >= 40 && roll > 0.6f) AttackStrong();
        else if (stamina >= 25 && roll > 0.3f) AttackMedium();
        else AttackLight();
    }

    // --- METODY ATAKU I REGENERACJI ---

    public void Sleep()
    {
        EnemyStats.RestoreStamina();
        Debug.Log("AI: Odpoczywa (Stamina regenerowana).");
    }

    public void Block() { Debug.Log("AI: Blokuje."); }

    public void AttackLight()
    {
        float cost = 10f;
        float dmg = EnemyStats.Strenght * 1.0f;

        EnemyStats.UseStamina(cost);
        DealDamageToPlayer(dmg);
        Debug.Log($"AI: Lekki atak ({dmg} dmg).");
    }

    public void AttackMedium()
    {
        float cost = 25f;
        float dmg = EnemyStats.Strenght * 1.5f;

        EnemyStats.UseStamina(cost);
        DealDamageToPlayer(dmg);
        Debug.Log($"AI: Średni atak ({dmg} dmg).");
    }

    public void AttackStrong()
    {
        float cost = 40f;
        float dmg = EnemyStats.Strenght * 2.0f;
        if (EnemyStats.Precision > 12) dmg += 5;

        EnemyStats.UseStamina(cost);
        DealDamageToPlayer(dmg);
        Debug.Log($"AI: MOCNY ATAK ({dmg} dmg)!");
    }

    private void DealDamageToPlayer(float amount)
    {
        try
        {
            // Wywołujemy Twoją metodę z CharacterStats
            TargetStats.GetDamage(amount);
            Debug.Log($"GRACZ OTRZYMAŁ OBRAŻENIA! HP: {TargetStats.CurrentHealth}");
        }
        catch (System.Exception e)
        {
            Debug.Log("GRACZ UMARŁ! " + e.Message);
        }
    }
}