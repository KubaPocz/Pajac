using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CutsceneManager : MonoBehaviour
{
    public string currentScene;
    public string sceneToLoad;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CutSceneEnd();
        }
    }

    private void CutSceneEnd()
    {
        if(sceneToLoad=="Credits")
            SceneManager.LoadScene("Credits");
        else if(sceneToLoad=="Boot")
            SceneManager.LoadScene("Boot");
        else
            CurtainManager.Instance.ChangeScene(sceneToLoad, currentScene, true);
        
    }
}
