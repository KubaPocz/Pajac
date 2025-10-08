using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    public string SceneToLoadAfterCutscene;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene(SceneToLoadAfterCutscene, LoadSceneMode.Single);

    }
}
