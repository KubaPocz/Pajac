using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [Header("Scene refs")]
    public PlayerController player;
    public EnemyController enemy;
    public ButtonsVisibility buttonsVisibility;

    public Image enemyImage;
    public Sprite[] enemySprites;

    private enum BattleState { None, PlayerTurn, EnemyTurn, Finished }
    private BattleState state = BattleState.None;

    // --- turn snapshots / action labels ---
    private TurnSnapshot snapshot;
    private string lastActionLabel = "-";

    private struct TurnSnapshot
    {
        public float pHP, pSTA;
        public float eHP, eSTA;
    }
    private void updateEnemySprite(int id)
    {
        enemyImage.sprite = enemySprites[id];
    }
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (player == null) player = FindObjectOfType<PlayerController>();
        if (enemy == null) enemy = FindObjectOfType<EnemyController>();
        if (buttonsVisibility == null) buttonsVisibility = FindObjectOfType<ButtonsVisibility>();

        Invoke(nameof(StartPlayerTurn), 0.25f);
        updateEnemySprite(0);
    }

    private bool CanRunTurn() =>
        state != BattleState.Finished &&
        player != null && enemy != null &&
        player.PlayerStats != null && enemy.EnemyStats != null;

    // --- Public API for controllers (set action name) ---
    public void SetLastAction(string label)
    {
        lastActionLabel = string.IsNullOrWhiteSpace(label) ? "-" : label;
    }

    private void CaptureSnapshot()
    {
        snapshot = new TurnSnapshot
        {
            pHP = player.PlayerStats.CurrentHealth,
            pSTA = player.PlayerStats.CurrentStamina,
            eHP = enemy.EnemyStats.CurrentHealth,
            eSTA = enemy.EnemyStats.CurrentStamina
        };
    }

    public void StartPlayerTurn()
    {
        if (!CanRunTurn()) return;
        state = BattleState.PlayerTurn;

        lastActionLabel = "-";
        CaptureSnapshot();

        buttonsVisibility?.HideAttacks();
        buttonsVisibility?.HideMoves();

        Debug.Log("\n<color=blue>--- TURA GRACZA ---</color>");
        player.Move(); // begin player turn (regen etc)
    }

    public void StartEnemyTurn()
    {
        if (!CanRunTurn()) return;
        state = BattleState.EnemyTurn;

        lastActionLabel = "-";
        CaptureSnapshot();

        buttonsVisibility?.HideAll();

        Debug.Log("\n<color=red>--- TURA WROGA ---</color>");
        enemy.Move(); // begin enemy AI coroutine
    }

    public void EndPlayerTurn()
    {
        if (state != BattleState.PlayerTurn) return;

        LogTurnReport(
            side: "GRACZ",
            action: lastActionLabel,
            pBeforeHP: snapshot.pHP, pBeforeSTA: snapshot.pSTA,
            eBeforeHP: snapshot.eHP, eBeforeSTA: snapshot.eSTA
        );

        StartEnemyTurn();
    }

    public void EndEnemyTurn()
    {
        if (state != BattleState.EnemyTurn) return;

        LogTurnReport(
            side: "WRÓG",
            action: lastActionLabel,
            pBeforeHP: snapshot.pHP, pBeforeSTA: snapshot.pSTA,
            eBeforeHP: snapshot.eHP, eBeforeSTA: snapshot.eSTA
        );

        StartPlayerTurn();
    }

    private void LogTurnReport(
        string side,
        string action,
        float pBeforeHP, float pBeforeSTA,
        float eBeforeHP, float eBeforeSTA
    )
    {
        var p = player.PlayerStats;
        var e = enemy.EnemyStats;

        float pAfterHP = p.CurrentHealth;
        float pAfterSTA = p.CurrentStamina;
        float eAfterHP = e.CurrentHealth;
        float eAfterSTA = e.CurrentStamina;

        float dmgToEnemy = Mathf.Max(0f, eBeforeHP - eAfterHP);
        float dmgToPlayer = Mathf.Max(0f, pBeforeHP - pAfterHP);

        float pStaSpent = Mathf.Max(0f, pBeforeSTA - pAfterSTA);
        float eStaSpent = Mathf.Max(0f, eBeforeSTA - eAfterSTA);

        string line =
            "\n<color=white>================= RAPORT TURY =================</color>\n" +
            $"<b>Strona:</b> {side}\n" +
            $"<b>Akcja:</b> {action}\n\n" +
            $"<color=green><b>GRACZ</b></color>  HP: {pBeforeHP:0.0} → {pAfterHP:0.0}  (Δ {(pAfterHP - pBeforeHP):+0.0;-0.0;0.0})" +
            $" | STA: {pBeforeSTA:0.0} → {pAfterSTA:0.0}  (Δ {(pAfterSTA - pBeforeSTA):+0.0;-0.0;0.0})\n" +
            $"<color=red><b>WRÓG</b></color>   HP: {eBeforeHP:0.0} → {eAfterHP:0.0}  (Δ {(eAfterHP - eBeforeHP):+0.0;-0.0;0.0})" +
            $" | STA: {eBeforeSTA:0.0} → {eAfterSTA:0.0}  (Δ {(eAfterSTA - eBeforeSTA):+0.0;-0.0;0.0})\n\n" +
            $"<b>Zadane obrażenia:</b>  na wroga: {dmgToEnemy:0.0} | na gracza: {dmgToPlayer:0.0}\n" +
            $"<b>Zużyta stamina:</b>    gracz: {pStaSpent:0.0} | wróg: {eStaSpent:0.0}\n" +
            "<color=white>================================================</color>";

        Debug.Log(line);
    }

    // =========================
    // UI BUTTONS (Unity UI)
    // =========================
    public void SleepButton()
    {
        if (state != BattleState.PlayerTurn) return;
        buttonsVisibility?.HideAll();
        player.Sleep();
    }

    public void AttackButton()
    {
        if (state != BattleState.PlayerTurn) return;
        buttonsVisibility?.HideAll();
        buttonsVisibility?.ShowAttacks();
    }

    public void LightAttackButton()
    {
        if (state != BattleState.PlayerTurn) return;
        buttonsVisibility?.HideAll();
        player.AttackLight();
    }

    public void NormalAttackButton()
    {
        if (state != BattleState.PlayerTurn) return;
        buttonsVisibility?.HideAll();
        player.AttackMedium();
    }

    public void HardAttackButton()
    {
        if (state != BattleState.PlayerTurn) return;
        buttonsVisibility?.HideAll();
        player.AttackStrong();
    }

    public void BlockButton()
    {
        if (state != BattleState.PlayerTurn) return;
        buttonsVisibility?.HideAll();
        player.Block();
    }

    public void MoveButton()
    {
        if (state != BattleState.PlayerTurn) return;
        buttonsVisibility?.HideAll();
        buttonsVisibility?.ShowMoves();
    }

    public void MoveLeftButton()
    {
        if (state != BattleState.PlayerTurn) return;
        buttonsVisibility?.HideAll();
        player.MoveLeft();
    }

    public void MoveRightButton()
    {
        //if (state != BattleState.PlayerTurn) return;
        buttonsVisibility?.HideAll();
        player.MoveRight();
    }
    private void EndBattle()
    {
        GameManager.Instance.CurrentEnemy++;
        updateEnemySprite(GameManager.Instance.CurrentEnemy);
        CurtainManager.Instance.ChangeScene("Statistics", "Fight", true);
    }
}
