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

        LogAIInitialization();
    }

    // === LOGOWANIE INICJALIZACJI ===
    private void LogAIInitialization()
    {
        Debug.Log("\n╔════════════════════════════════════════════════╗");
        Debug.Log("║         AI WROGA ZAINICJALIZOWANY              ║");
        Debug.Log("╚════════════════════════════════════════════════╝");
        Debug.Log($"[STATS] Nazwa wroga: {EnemyStats.CharacterName}");
        Debug.Log($"[STATS] HP: {EnemyStats.CurrentHealth}/{EnemyStats.MaxHealth}");
        Debug.Log($"[STATS] Stamina: {EnemyStats.CurrentStamina}/{EnemyStats.MaxStamina}");
        Debug.Log($"[STATS] Siła: {EnemyStats.Strenght} | Zwinność: {EnemyStats.Agility} | Precyzja: {EnemyStats.Precision}");
        Debug.Log($"[SETTINGS] Zasięg ataku: {attackRange}m | Prędkość ruchu: {moveSpeed}");
        Debug.Log("");
    }

    public void TakeTurn()
    {
        StartCoroutine(AI_Logic());
    }

    private IEnumerator AI_Logic()
    {
        turnCount++;

        // NAGŁÓWEK TURY
        Debug.Log("\n╔════════════════════════════════════════════════╗");
        Debug.Log($"║        >>> TURA WROGA #{turnCount} ({EnemyStats.CharacterName}) <<<        ║");
        Debug.Log("╚════════════════════════════════════════════════╝");

        EnemyStats.NewTurnRegen();
        Debug.Log($"[POCZĄTEK TURY] Blok zdjęty. HP: {EnemyStats.CurrentHealth}/{EnemyStats.MaxHealth} | STA: {EnemyStats.CurrentStamina}/{EnemyStats.MaxStamina}");

        yield return new WaitForSeconds(1.0f);

        // WARUNEK KOŃCA GRY
        if (EnemyStats.CurrentHealth <= 0 || TargetStats.CurrentHealth <= 0)
        {
            Debug.Log("[KONIEC GRY] Walka zakończona - ktoś nie żyje!");
            if (EnemyStats.CurrentHealth <= 0) Debug.Log("  → Wróg POKONANY!");
            if (TargetStats.CurrentHealth <= 0) Debug.Log("  → Gracz POKONANY!");
            EndTurn();
            yield break;
        }

        // RAPORT DYSTANSU I STAMINY
        float dist = Vector3.Distance(transform.position, TargetTransform.position);
        Debug.Log($"\n[OBSERWACJA] Dystans do gracza: {dist:F2}m (Wymagany: {attackRange}m)");
        Debug.Log($"[OBSERWACJA] Stamina wroga: {EnemyStats.CurrentStamina}/{EnemyStats.MaxStamina}");
        Debug.Log($"[OBSERWACJA] HP gracza: {TargetStats.CurrentHealth}/{TargetStats.MaxHealth}");

        // --- DRZEWO DECYZYJNE AI ---

        // 1. CZY WRÓG MA WYSTARCZAJĄCO SIŁY? (< 20 = Sen)
        if (EnemyStats.CurrentStamina < 20)
        {
            Debug.Log("\n[DECYZJA] ❌ Zbyt mało staminy!");
            Debug.Log($"         Wymagane minimum: 20 | Dostępne: {EnemyStats.CurrentStamina}");
            Debug.Log("         → AKCJA: SEN (Regeneracja +40 STA)");
            PerformSleep();
        }
        // 2. CZY WRÓG JEST W ZASIĘGU ATAKU?
        else if (dist <= attackRange + 0.5f)
        {
            Debug.Log("\n[DECYZJA] ✓ Gracz w zasięgu ataku!");
            float stamAvailable = EnemyStats.CurrentStamina;
            Debug.Log($"         Dostępna stamina: {stamAvailable}");

            // Wybór ataku bazując na ilości staminy
            if (stamAvailable >= 30 && Random.value > 0.6f)
            {
                Debug.Log("         → WYBÓR: Ciężki atak (random: 0.6 szansa)");
                PerformAttack(30, 2.0f, "CIĘŻKI ATAK");
                heavyAttacksCount++;
            }
            else if (stamAvailable >= 20 && Random.value > 0.4f)
            {
                Debug.Log("         → WYBÓR: Średni atak (random: 0.4 szansa)");
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
        // 3. CZY GRACZ JE ZA DALEKO? (Ruch w stronę gracza)
        else
        {
            Debug.Log("\n[DECYZJA] ⚠ Gracz za daleko!");
            Debug.Log($"         Dystans: {dist:F2}m > Zasięg: {attackRange}m");
            Debug.Log($"         → AKCJA: RUCH NAPRZÓD (Koszt: 5 STA)");

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

    // === SZCZEGÓŁOWY RAPORT Z ATAKU ===
    private void PerformAttack(float cost, float multiplier, string attackName)
    {
        totalAttackAttempts++;

        Debug.Log($"\n┌─────────────────────────────────────────────────┐");
        Debug.Log($"│ ⚔️  ATAK #{totalAttackAttempts}: {attackName,-30} ⚔️ │");
        Debug.Log($"└─────────────────────────────────────────────────┘");

        // 1. SPRAWDZENIE KOSZTU
        if (!EnemyStats.UseStamina(cost))
        {
            Debug.Log($"[❌ BŁĄD ATAKU] Wróg chciał użyć {attackName}");
            Debug.Log($"              Wymagane: {cost} STA | Dostępne: {EnemyStats.CurrentStamina}");
            Debug.Log("              → Atak ANULOWANY! Wróg idzie spać.");
            PerformSleep();
            return;
        }

        Debug.Log($"[KOSZT] Stamina: -{cost} (Pozostało: {EnemyStats.CurrentStamina})");
        Debug.Log($"[MNOŻNIK OBRAŻEŃ] x{multiplier} | Typ: {attackName}");

        // 2. SZANSA TRAFIENIA (80% + różnica precyzji)
        float hitChance = 80f + (EnemyStats.Precision - TargetStats.Precision);
        float hitRoll = Random.Range(0f, 100f);
        bool isHit = (hitRoll <= hitChance);

        Debug.Log($"\n[TRAFIENIE]");
        Debug.Log($"  Siła Wroga: {EnemyStats.Precision} | Obrona Gracza: {TargetStats.Precision}");
        Debug.Log($"  Szansa trafienia: {hitChance:F1}%");
        Debug.Log($"  Wylosowano: {hitRoll:F2}%");
        Debug.Log($"  → WYNIK: {(isHit ? "✓ TRAFIENIE" : "✗ PUDŁO")}");

        if (!isHit)
        {
            totalMisses++;
            Debug.Log($"\n[STATYSTYKA] Pudła wroga: {totalMisses}");
            return;
        }

        totalHits++;

        // 3. UNIK GRACZA (10% + różnica zwinności, min 5, max 50)
        float dodgeChance = 10f + (TargetStats.Agility - EnemyStats.Agility);
        dodgeChance = Mathf.Clamp(dodgeChance, 5f, 50f);
        float dodgeRoll = Random.Range(0f, 100f);
        bool isDodged = (dodgeRoll < dodgeChance);

        Debug.Log($"\n[SZANSA UNIKU GRACZA]");
        Debug.Log($"  Zwinność Gracza: {TargetStats.Agility} | Zwinność Wroga: {EnemyStats.Agility}");
        Debug.Log($"  Szansa uniku gracza: {dodgeChance:F1}%");
        Debug.Log($"  Wylosowano: {dodgeRoll:F2}%");
        Debug.Log($"  → WYNIK: {(isDodged ? "✓✓✓ GRACZ UNIKNĄŁ ATAKU! ✓✓✓" : "✗ UNIK NIEUDANY - TRAFIA W CEL!")}");

        if (isDodged)
        {
            totalDodgesAgainstPlayer++;
            Debug.Log($"\n[STATYSTYKA] Udane uniki gracza w całej walce: {totalDodgesAgainstPlayer}");
            return;
        }

        // 4. OBLICZENIE OBRAŻEŃ
        float baseDamage = EnemyStats.Strenght * multiplier;
        Debug.Log($"\n[OBRAŻENIA]");
        Debug.Log($"  Siła Wroga: {EnemyStats.Strenght}");
        Debug.Log($"  Bazowe obrażenia: {EnemyStats.Strenght} × {multiplier} = {baseDamage}");

        // 5. BLOK GRACZA
        float finalDamage = baseDamage;
        if (TargetStats.isBlocking)
        {
            float reductionPercent = 50f + (TargetStats.Agility * 0.5f);
            if (reductionPercent > 80f) reductionPercent = 80f;

            float reductionAmount = baseDamage * (reductionPercent / 100f);
            finalDamage -= reductionAmount;

            Debug.Log($"\n[BLOK GRACZA] 🛡");
            Debug.Log($"  Zwinność gracza: {TargetStats.Agility}");
            Debug.Log($"  Procent redukcji: {reductionPercent:F1}%");
            Debug.Log($"  Zmniejszone obrażenia: {baseDamage} - {reductionAmount:F1} = {finalDamage:F1}");
            totalBlocksByPlayer++;
        }
        else
        {
            Debug.Log($"[BLOK] Gracz NIE BLOKUJE");
        }

        // 6. FINALNE OBRAŻENIA
        Debug.Log($"\n[FINAŁ] ─────────────────────────────────────────────");
        Debug.Log($"       💥 FINALNE OBRAŻENIA: {finalDamage:F1}");
        Debug.Log($"       Typ ataku: {attackName} | Mnożnik: x{multiplier}");
        Debug.Log($"       HP Gracza: {TargetStats.CurrentHealth:F1} → {TargetStats.CurrentHealth - finalDamage:F1}");

        TargetStats.GetDamage(finalDamage);
        totalDamageDealt += finalDamage;

        Debug.Log($"\n[STATYSTYKA] Całkowite obrażenia wroga: {totalDamageDealt:F1}");
        Debug.Log($"             Trafienia: {totalHits} | Pudła: {totalMisses}");
    }

    // === LOGOWANIE RUCHU ===
    private IEnumerator MoveRoutine(float currentDist)
    {
        Debug.Log($"\n[RUCH] ─────────────────────────────────────────────");

        Vector3 start = transform.position;
        Vector3 target = TargetTransform.position;
        Vector3 dir = (target - start).normalized;
        float travel = Mathf.Min(currentDist - attackRange, moveSpeed);

        Debug.Log($"  Pozycja początkowa: ({start.x:F2}, {start.y:F2})");
        Debug.Log($"  Pozycja gracza: ({target.x:F2}, {target.y:F2})");
        Debug.Log($"  Kierunek: {dir.ToString("F2")}");
        Debug.Log($"  Dystans do pokonania: {travel:F2}m");

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

    // === LOGOWANIE SNU ===
    private void PerformSleep()
    {
        totalSleepTurns++;
        Debug.Log($"\n[SEN] ─────────────────────────────────────────────");
        Debug.Log($"  Wróg regeneruje siły...");
        Debug.Log($"  Stamina przed: {EnemyStats.CurrentStamina}");
        EnemyStats.RestoreStamina(40);
        Debug.Log($"  Stamina po: {EnemyStats.CurrentStamina}");
        Debug.Log($"  [STATYSTYKA] Całkowite tury snu: {totalSleepTurns}");
    }

    // === LOGOWANIE KOŃCA TURY ===
    private void EndTurn()
    {
        Debug.Log($"\n╔════════════════════════════════════════════════╗");
        Debug.Log($"║        KONIEC TURY WROGA #{turnCount,-28} ║");
        Debug.Log($"╚════════════════════════════════════════════════╝");

        // PODSUMOWANIE TURY
        Debug.Log($"\n[PODSUMOWANIE TURY]");
        Debug.Log($"  Liczba tur: {turnCount}");
        Debug.Log($"  Akcja: Atak/Ruch/Sen");
        Debug.Log($"  HP Wroga: {EnemyStats.CurrentHealth:F1}/{EnemyStats.MaxHealth}");
        Debug.Log($"  STA Wroga: {EnemyStats.CurrentStamina:F1}/{EnemyStats.MaxStamina}");
        Debug.Log($"  HP Gracza: {TargetStats.CurrentHealth:F1}/{TargetStats.MaxHealth}");

        // STATYSTYKI GLOBALNE
        Debug.Log($"\n[STATYSTYKI GLOBALNE AI]");
        Debug.Log($"  └─ Ataki Lekkie: {lightAttacksCount}");
        Debug.Log($"  └─ Ataki Średnie: {mediumAttacksCount}");
        Debug.Log($"  └─ Ataki Ciężkie: {heavyAttacksCount}");
        Debug.Log($"  └─ Razem ataków: {totalAttackAttempts} (Trafienia: {totalHits}, Pudła: {totalMisses})");
        Debug.Log($"  └─ Celne Ataki: {totalHits}/{totalAttackAttempts} ({(totalAttackAttempts > 0 ? (totalHits * 100f / totalAttackAttempts) : 0):F1}%)");
        Debug.Log($"  └─ Obrony: Uniki gracza: {totalDodgesAgainstPlayer}, Bloki: {totalBlocksByPlayer}");
        Debug.Log($"  └─ Ruchy: {totalMovesAttempted}");
        Debug.Log($"  └─ Sny: {totalSleepTurns}");
        Debug.Log($"  └─ Całkowite obrażenia: {totalDamageDealt:F1}");

        Debug.Log("\n");

        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.LogBattleState();
            BattleManager.Instance.StartPlayerTurn();
        }
    }

    // Puste implementacje interfejsu (wymagane)
    public void MoveRight() { }
    public void MoveLeft() { }
    public void Dodge() { }
    public void AttackLight() { }
    public void AttackMedium() { }
    public void AttackStrong() { }
    public void Sleep() { }
    public void Block() { }
}