using UnityEngine;

public class BladeController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Enemy got hit");
        if (collision.tag == "Enemy")
        {
            Debug.Log("Enemy got hit");
        }
    }
}
