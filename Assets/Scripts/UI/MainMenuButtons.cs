using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public void NewGame()
    {
        CurtainManager.Instance.ShowCurtainAndChangeScene("Statistics", "MainMenu");
    }
    public void ContinueGame()
    {

    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
