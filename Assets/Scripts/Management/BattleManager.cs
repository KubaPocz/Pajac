using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public PlayerController player;
    public EnemyController enemy;
    public ButtonsVisibility buttonsVisibility;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Start walki po chwili
        Invoke("StartPlayerTurn", 1.0f);
    }

    public void StartEnemyTurn()
    {
        enemy.Move();
    }

    public void StartPlayerTurn()
    {
        player.Move();
    }

    // --- LOGOWANIE STANU GRY ---
    public void LogBattleState()
    {
        if (player == null || enemy == null) return;

        var p = player.PlayerStats;
        var e = enemy.EnemyStats;

        string log = "\n<color=white>================ RAPORT STANU ================</color>\n";

        // GRACZ
        log += $"<color=green>GRACZ ({p.CharacterName}):</color> HP: {p.CurrentHealth}/{p.MaxHealth} | STA: {p.CurrentStamina}/{p.MaxStamina}";
        if (p.isBlocking) log += " <color=cyan>[BLOKUJE]</color>";
        log += "\n";

        // WRÓG
        log += $"<color=red>WRÓG  ({e.CharacterName}):</color> HP: {e.CurrentHealth}/{e.MaxHealth} | STA: {e.CurrentStamina}/{e.MaxStamina}";
        if (e.isBlocking) log += " <color=cyan>[BLOKUJE]</color>";

        log += "\n<color=white>==============================================</color>";

        Debug.Log(log);
    }
    public void SleepButton()
    {
        buttonsVisibility.HideAttacks();
        buttonsVisibility.HideMoves();

        //reszta
    }
    public void AttackButton()
    {
        buttonsVisibility.HideAll();
        buttonsVisibility.ShowAttacks();

        //reszta
    }
    public void LightAttackButton()
    {
        buttonsVisibility.HideAll();

        //reszta
    }
    public void NormalAttackButton()
    {
        buttonsVisibility.HideAll();

        //reszta
    }
    public void HardAttackButton()
    {
        buttonsVisibility.HideAll();

        //reszta
    }
    public void BlockButton()
    {
        buttonsVisibility.HideAll();

        //reszta
    }
    public void MoveButton()
    {
        buttonsVisibility.HideAll();
        buttonsVisibility.ShowMoves();

        //reszta
    }
    public void MoveLeftButton()
    {
        buttonsVisibility.HideAll();

        //reszta
    }
    public void MoveRightButton()
    {
        buttonsVisibility.HideAll();

        //reszta
    }
}