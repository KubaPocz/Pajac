using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5.0f;
    [SerializeField] Animator animator;
    public bool isPlayerTurn = true;
    EnemyBehav enemy;

    void Start()
    {
        // Find and assign the enemy reference
        enemy = FindAnyObjectByType<EnemyBehav>();

        // Safety check
        if (enemy == null)
        {
            Debug.LogError("EnemyBehav script not found in the scene!");
        }
    }

    public void PlayerMoveLeft()
    {
        if (isPlayerTurn) {
            transform.position += new Vector3(-playerSpeed, 0f);
            isPlayerTurn = false;
            enemy.EnemyAction();
        }
    }
    public void PlayerMoveRight()
    {
        if (isPlayerTurn)
        {
            transform.position += new Vector3(playerSpeed, 0f);
            isPlayerTurn = false;
            enemy.EnemyAction();
        }
    }
    public void PlayerAttack()
    {
        if (isPlayerTurn)
        {
            animator.SetTrigger("Attack");
            isPlayerTurn = false;
            enemy.EnemyAction();
        }
    }
}