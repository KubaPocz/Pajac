using System.Threading.Tasks;
using UnityEngine;

public class EnemyBehav : MonoBehaviour
{
    [SerializeField] float enemySpeed = 1.0f;
    [SerializeField] Animator animator;
    public GameObject Player;
    public GameObject Enemy;
    PlayerController turn;
    [SerializeField]public float distanceBetweenActors { get; private set; }

    void Start()
    {
        // Find and assign the enemy reference
        turn = FindAnyObjectByType<PlayerController>();

        // Safety check
        if (turn == null)
        {
            Debug.LogError("EnemyBehav script not found in the scene!");
        }
    }


    public async void EnemyAction()
    {
        
        if (Player != null && Enemy != null)
        {
            distanceBetweenActors = Vector2.Distance(
                Player.transform.position,
                Enemy.transform.position
            );
        }
        await Task.Delay(2000);
        if (distanceBetweenActors >= 1.5f) 
        {
            transform.position += new Vector3(-enemySpeed, 0f);
        }
        else 
        {
            animator.SetTrigger("EnemyAttack"); 
        }
        await Task.Delay(1000);
        turn.isPlayerTurn = true;
    }
}
