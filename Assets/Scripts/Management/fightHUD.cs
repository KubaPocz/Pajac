using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FightHUD : MonoBehaviour
{
    [Header("Auto-find (optional)")]
    public PlayerController player;
    public EnemyController enemy;

    [Header("PLAYER UI (HP left bottom / STA right bottom)")]
    public Image playerHpFill;      // Image Type = Filled
    public Image playerStaFill;     // Image Type = Filled
    public TMP_Text playerHpText;   // optional
    public TMP_Text playerStaText;  // optional

    [Header("ENEMY UI (optional)")]
    public Image enemyHpFill;
    public Image enemyStaFill;
    public TMP_Text enemyHpText;
    public TMP_Text enemyStaText;

    [Header("Animation")]
    [Tooltip("Jak szybko paski doganiają wartość docelową (większe = szybciej)")]
    public float lerpSpeed = 10f;

    [Tooltip("Minimalna zmiana aby ruszyć animację (anty-jitter)")]
    public float epsilon = 0.001f;

    // aktualnie wyświetlane wartości (0..1)
    private float pHpShown, pStaShown, eHpShown, eStaShown;

    private void Start()
    {
        if (player == null) player = FindObjectOfType<PlayerController>();
        if (enemy == null) enemy = FindObjectOfType<EnemyController>();

        ForceRefreshImmediate();
    }

    private void Update()
    {
        if (player == null || player.PlayerStats == null) return;

        // cele (0..1)
        float pHpTarget = Safe01(player.PlayerStats.CurrentHealth, player.PlayerStats.MaxHealth);
        float pStaTarget = Safe01(player.PlayerStats.CurrentStamina, player.PlayerStats.MaxStamina);

        // płynne doganianie
        pHpShown = Smooth(pHpShown, pHpTarget);
        pStaShown = Smooth(pStaShown, pStaTarget);

        if (enemy != null && enemy.EnemyStats != null)
        {
            float eHpTarget = Safe01(enemy.EnemyStats.CurrentHealth, enemy.EnemyStats.MaxHealth);
            float eStaTarget = Safe01(enemy.EnemyStats.CurrentStamina, enemy.EnemyStats.MaxStamina);

            eHpShown = Smooth(eHpShown, eHpTarget);
            eStaShown = Smooth(eStaShown, eStaTarget);
        }

        ApplyUI();
    }

    public void ForceRefreshImmediate()
    {
        if (player != null && player.PlayerStats != null)
        {
            pHpShown = Safe01(player.PlayerStats.CurrentHealth, player.PlayerStats.MaxHealth);
            pStaShown = Safe01(player.PlayerStats.CurrentStamina, player.PlayerStats.MaxStamina);
        }

        if (enemy != null && enemy.EnemyStats != null)
        {
            eHpShown = Safe01(enemy.EnemyStats.CurrentHealth, enemy.EnemyStats.MaxHealth);
            eStaShown = Safe01(enemy.EnemyStats.CurrentStamina, enemy.EnemyStats.MaxStamina);
        }

        ApplyUI(immediate: true);
    }

    private float Smooth(float current, float target)
    {
        if (Mathf.Abs(current - target) < epsilon)
            return target;

        // Lerp z prędkością niezależną od FPS
        return Mathf.Lerp(current, target, Time.deltaTime * Mathf.Max(0.01f, lerpSpeed));
    }

    private void ApplyUI(bool immediate = false)
    {
        // PLAYER
        if (playerHpFill != null) playerHpFill.fillAmount = pHpShown;
        if (playerStaFill != null) playerStaFill.fillAmount = pStaShown;

        if (playerHpText != null && player?.PlayerStats != null)
            playerHpText.text = $"{player.PlayerStats.CurrentHealth:0}/{player.PlayerStats.MaxHealth:0}";

        if (playerStaText != null && player?.PlayerStats != null)
            playerStaText.text = $"{player.PlayerStats.CurrentStamina:0}/{player.PlayerStats.MaxStamina:0}";

        // ENEMY (opcjonalnie)
        if (enemyHpFill != null) enemyHpFill.fillAmount = eHpShown;
        if (enemyStaFill != null) enemyStaFill.fillAmount = eStaShown;

        if (enemyHpText != null && enemy?.EnemyStats != null)
            enemyHpText.text = $"{enemy.EnemyStats.CurrentHealth:0}/{enemy.EnemyStats.MaxHealth:0}";

        if (enemyStaText != null && enemy?.EnemyStats != null)
            enemyStaText.text = $"{enemy.EnemyStats.CurrentStamina:0}/{enemy.EnemyStats.MaxStamina:0}";
    }

    private float Safe01(float current, float max)
    {
        if (max <= 0.0001f) return 0f;
        return Mathf.Clamp01(current / max);
    }
}
