using UnityEngine;
using UnityEngine.SceneManagement;

public class BootManager : MonoBehaviour
{
    private async void Start()
    {
        await SceneManager.LoadSceneAsync("LoadingScene");

        await SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
    }
}
