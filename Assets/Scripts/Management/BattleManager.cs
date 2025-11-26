using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public CharacterController player;
    public CharacterController enemy;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartEnemyTurn()
    {
        enemy.TakeTurn();
    }

    public void StartPlayerTurn()
    {
        player.TakeTurn();
    }
}
