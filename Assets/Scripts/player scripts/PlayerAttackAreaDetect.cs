using UnityEngine;

public class PlayerAttackAreaDetect : MonoBehaviour
{
    public bool enemyInArea = false;
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyInArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyInArea = false;
        }
    }
}
