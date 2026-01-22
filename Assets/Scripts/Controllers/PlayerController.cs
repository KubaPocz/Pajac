using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour, CharacterController
{
    [Header("Stats")]
    public CharacterStats PlayerStats;

    public float moveStep = 200f;
    public float minDistanceToEnemy = 100f;
    public TMP_Text info;

    [Header("Weapon")]
    public GameObject staffPrefab;          // PREFAB laski (z PlayerStaffProjectile)

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

    public void Move()
    {
        isMyTurn = true;
        actionLocked = false;

        if (PlayerStats != null)
            PlayerStats.NewTurnRegen();

        Debug.Log("\n--- TURA GRACZA ---");
    }

    private void Update()
    {
        if (!isMyTurn || actionLocked) return;

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

    // --- RUCH ---

    public void MoveRight()
    {
        if (!CanAct()) return;
        if (!PlayerStats.UseStamina(5)) return;

        Vector3 from = transform.position;
        Vector3 to = from + Vector3.right * moveStep;

        if (!CanMoveTo(to)) return;

        StartCoroutine(SmoothMove(from, to, 0.3f));
        Debug.Log("Gracz: Ruch w prawo.");
        StartCoroutine(ShowInfo("Moving right"));
        EndTurn();
    }

    public void MoveLeft()
    {
        if (!CanAct()) return;
        if (!PlayerStats.UseStamina(5)) return;

        Vector3 from = transform.position;
        Vector3 to = from + Vector3.left * moveStep;

        if (!CanMoveTo(to)) return;

        StartCoroutine(SmoothMove(from, to, 0.3f));
        Debug.Log("Gracz: Ruch w lewo.");
        StartCoroutine(ShowInfo("Moving left"));
        EndTurn();
    }

    private bool CanMoveTo(Vector3 targetPos)
    {
        if (currentEnemy == null) return true;

        float enemyX = currentEnemy.transform.position.x;
        float targetX = targetPos.x;
        float distanceX = Mathf.Abs(enemyX - targetX);

        if (distanceX < minDistanceToEnemy)
        {
            Debug.Log("Ruch zablokowany: za blisko przeciwnika.");
            return false;
        }

        return true;
    }

    // --- RESZTA AKCJI ---

    public void Sleep()
    {
        if (!CanAct()) return;

        Debug.Log("Gracz: Idzie spać.");
        StartCoroutine(ShowInfo("Sleeping..."));
        PlayerStats.RestoreStamina(60);
        EndTurn();
    }

    public void Block()
    {
        if (!CanAct()) return;
        if (!PlayerStats.UseStamina(15)) return;

        PlayerStats.isBlocking = true;
        Debug.Log("Gracz: Postawa obronna (BLOK).");
        StartCoroutine(ShowInfo("Defending..."));
        EndTurn();
    }

    public void AttackLight() { TryPlayerAttack(10, 1.0f, "Light"); }
    public void AttackMedium() { TryPlayerAttack(20, 1.5f, "Medium"); }
    public void AttackStrong() { TryPlayerAttack(30, 2.0f, "Strong"); }

    private void TryPlayerAttack(float cost, float multiplier, string name)
    {
        if (!CanAct()) return;
        if (!PlayerStats.UseStamina(cost)) return;

        CharacterStats target = currentEnemy.EnemyStats;
        if (target == null)
        {
            Debug.LogError("[PlayerController] EnemyStats is NULL.");
            return;
        }

        // --- OGRANICZENIE ZASIĘGU ATAKU ---
        // np. gracz może atakować tylko jeśli jest bliżej niż attackRangeX jednostek po osi X
        float attackRangeX = 200f; // DOSTOSUJ do swojego świata (na start daj tyle co minDistanceToEnemy lub trochę więcej)
        float distX = Mathf.Abs(currentEnemy.transform.position.x - transform.position.x);

        if (distX > attackRangeX)
        {
            Debug.Log($"Atak anulowany – za daleko. DystansX: {distX} > Zasięg: {attackRangeX}");
            StartCoroutine(ShowInfo("Too far to attack"));
            EndTurn();
            return;
        }
        // -----------------------------------

        Debug.Log($"Gracz: Wykonuje {name} atak!");
        StartCoroutine(ShowInfo($"{name} attack"));

        // jeśli używasz laski – zostaw swój kod tutaj (Instantiate staffPrefab itd.)

        float hitChance = 80f + (PlayerStats.Precision - target.Precision);
        float hitRoll = Random.Range(0f, 100f);
        if (hitRoll > hitChance)
        {
            Debug.Log($"... PUDŁO! (Szansa: {hitChance}%, Wylosowano: {hitRoll})");
            EndTurn();
            return;
        }

        float dodgeChance = 10f + (target.Agility - PlayerStats.Agility);
        dodgeChance = Mathf.Clamp(dodgeChance, 5f, 50f);
        float dodgeRoll = Random.Range(0f, 100f);
        if (dodgeRoll < dodgeChance)
        {
            Debug.Log($"... PRZECIWNIK ZROBIŁ UNIK! (Szansa uniku: {dodgeChance}%)");
            StartCoroutine(ShowInfo("Enemy dodged"));
            EndTurn();
            return;
        }

        float damage = PlayerStats.Strenght * multiplier;

        if (target.isBlocking)
        {
            float reductionPercent = 50f + (target.Agility * 0.5f);
            if (reductionPercent > 80f) reductionPercent = 80f;

            float reductionAmount = damage * (reductionPercent / 100f);
            damage -= reductionAmount;
            Debug.Log($"... PRZECIWNIK BLOKUJE! Zredukował obrażenia o {reductionPercent}% (-{reductionAmount} dmg).");
        }

        Debug.Log($"... SUKCES! Przeciwnik otrzymuje {damage} obrażeń.");
        target.GetDamage(damage);

        EndTurn();
    }


    private IEnumerator SmoothMove(Vector3 from, Vector3 to, float duration)
    {
        actionLocked = true;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }

        transform.position = to;
        actionLocked = false;
    }

    private IEnumerator ShowInfo(string infoMessage)
    {
        if (info != null)
            info.text = infoMessage;

        Debug.Log(infoMessage);
        yield return new WaitForSeconds(1f);

        if (info != null)
            info.text = "";
    }
}
