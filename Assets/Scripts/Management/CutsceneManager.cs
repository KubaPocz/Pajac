using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CutsceneManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CutSceneEnd();
        }
    }

    private void CutSceneEnd()
    {
        if (GameManager.Instance.CurrentEnemy == 0)
        {
            CurtainManager.Instance.ChangeScene("Statistics", "StartingCutscene", true);
        }
        else
            SceneManager.LoadScene("Credits");
    }
}
