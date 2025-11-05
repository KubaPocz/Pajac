using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public void NewGame()
    {
        SceneManager.LoadSceneAsync("LoadingScene");
    }
    public void ContinueGame()
    {

    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
