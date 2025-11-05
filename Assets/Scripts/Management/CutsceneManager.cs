using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CutsceneManager : MonoBehaviour
{
    [SerializeField] private string targetSceneName;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CutSceneEnd();
        }
    }

    private void CutSceneEnd()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
