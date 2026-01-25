using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(VideoPlayer))]
public class ChangeSceneOnVideoEnd : MonoBehaviour
{
    public string nextSceneName = "Statistics"; // tu wpisz nazwę sceny

    private VideoPlayer videoPlayer;

    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += OnVideoFinished; // wywoła się gdy film się skończy
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        CurtainManager.Instance.ChangeScene("Statistics", "StartingCutscene", true);
    }
}
