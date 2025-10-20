using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class EnemyDaggerControler : MonoBehaviour
{
    public Image PlayerHp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (PlayerHp.fillAmount != 0f)
                PlayerHp.fillAmount -= 0.2f;
            if (PlayerHp.fillAmount <= 0f)
                SceneManager.LoadScene("Stats");
        }
    }
}
