using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float enemySpeed = 1.0f;
    [SerializeField] Animator animator;
    public GameObject Player;
    private PlayerController turn;
    private EnemyAttackAreaDetect AreaDetect;
    public Image PlayerHp;

    void Start()
    {
        // Find and assign the enemy reference
        turn = FindAnyObjectByType<PlayerController>();
        AreaDetect = GetComponentInChildren<EnemyAttackAreaDetect>();
        // Safety check
        if (turn == null)
        {
            Debug.LogError("EnemyController script not found in the scene!");
        }
    }



    public async void EnemyAction()
    {
        await Task.Delay(2000);

        if (AreaDetect.playerInArea == false)
        {
            transform.position += new Vector3(-enemySpeed, 0f, 0f);
        }
        else
        {
            animator.SetTrigger("EnemyAttack");
            if (PlayerHp.fillAmount != 0f)
                PlayerHp.fillAmount -= 0.2f;
            if (PlayerHp.fillAmount <= 0f)
                SceneManager.LoadScene("Stats");
        }

        await Task.Delay(1000);
        turn.isPlayerTurn = true;
    }
}