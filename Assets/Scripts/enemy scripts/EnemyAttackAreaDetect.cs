using UnityEngine;

public class EnemyAttackAreaDetect : MonoBehaviour
{
    public bool playerInArea = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInArea = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInArea = false;
        }
    }
}
