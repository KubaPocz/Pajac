using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public void NewGame()
    {
        CurtainManager.Instance.ChangeScene("StartingCutscene", "MainMenu",true);
    }
    public void ContinueGame()
    {

    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
