using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BladeController : MonoBehaviour
{
    public Image EnemyHp;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            if(EnemyHp.fillAmount != 0f)
                EnemyHp.fillAmount -= 0.2f;
            if (EnemyHp.fillAmount <= 0f)
                SceneManager.LoadScene("Stats");
        }
    }
}
