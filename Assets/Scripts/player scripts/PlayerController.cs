using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5.0f;
    [SerializeField] Animator animator;
    public bool isPlayerTurn = true;
    private EnemyController enemy;
    public Image EnemyHp;
    private PlayerAttackAreaDetect areaDetect;
    void Start()
    {
        // Find and assign the enemy reference
        enemy = FindAnyObjectByType<EnemyController>();
        areaDetect = FindAnyObjectByType<PlayerAttackAreaDetect>();
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
            if (areaDetect.enemyInArea)
            {
                if (EnemyHp.fillAmount != 0f)
                    EnemyHp.fillAmount -= 0.2f;
                if (EnemyHp.fillAmount <= 0f)
                    SceneManager.LoadScene("Stats");
            }
            isPlayerTurn = false;
            enemy.EnemyAction();
        }
    }
}