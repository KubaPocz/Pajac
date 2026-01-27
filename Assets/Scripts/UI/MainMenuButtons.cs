using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public void NewGame()
    {
        CurtainManager.Instance.ChangeScene("StartingCutscene", "MainMenu",false);
    }
    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
