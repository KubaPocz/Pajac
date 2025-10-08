using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5.0f;
    [SerializeField] Animator animator;
    public void PlayerMoveLeft()
    {
        transform.position += new Vector3(-playerSpeed, 0f);
    }
    public void PlayerMoveRight()
    {
        transform.position += new Vector3(playerSpeed, 0f);
    }
    public void PlayerAttack()
    {
        animator.SetTrigger("Attack");
    }
}