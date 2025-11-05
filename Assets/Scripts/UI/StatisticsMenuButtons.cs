using UnityEngine;
using UnityEngine.SceneManagement;
public class StatisticsMenuButtons : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartFight()
    {
        SceneManager.LoadScene("Fight");
    }
}
