using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class CurtainManager : MonoBehaviour
{
    public static CurtainManager Instance;
    [Header("Curtain Settings")]
    public Animator CurtainAnimator;
    public VideoPlayer CurtainVideoPlayer;
    public VideoClip OpeningCurtainClip;
    public VideoClip ClosingCurtainClip;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        Instance = this;
    }

    private void Start()
    {
        CurtainVideoPlayer.clip = null;
    }
    public void ShowCurtainAndChangeScene(string sceneToLoad, string sceneToUnload)
    {
        StartCoroutine(ShowCurtainAndChangeSceneRoutine(sceneToLoad, sceneToUnload));
    }

    private IEnumerator ShowCurtainAndChangeSceneRoutine(string sceneToLoad, string sceneToUnload)
    {
        yield return StartCoroutine(ShowCurtainRoutine());

        ChangeSceneWithCurtain(sceneToLoad, sceneToUnload);
    }
    public void ChangeSceneWithCurtain(string sceneToLoad, string sceneToUnload)
    {
        CurtainVideoPlayer.clip = ClosingCurtainClip;
        StartCoroutine(ChangeSceneWithCurtainRoutine(sceneToLoad, sceneToUnload));
    }

    private IEnumerator ChangeSceneWithCurtainRoutine(string sceneToLoad, string sceneToUnload)
    {
        yield return StartCoroutine(CloseCurtainRoutine());

        if (!string.IsNullOrEmpty(sceneToUnload))
        {
            SceneManager.UnloadSceneAsync(sceneToUnload);
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        yield return StartCoroutine(OpenCurtainRoutine());
    }

    public void ShowCurtain()
    {
        StartCoroutine(ShowCurtainRoutine());
    }

    public void CloseCurtain()
    {
        StartCoroutine(CloseCurtainRoutine());
    }

    public void OpenCurtain()
    {
        StartCoroutine(OpenCurtainRoutine());
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    public void UnloadScene(string sceneName)
    {
        StartCoroutine(UnloadSceneRoutine(sceneName));
    }

    private IEnumerator ShowCurtainRoutine()
    {
        CurtainAnimator.SetTrigger("ShowCurtain");
        AnimatorStateInfo state = CurtainAnimator.GetCurrentAnimatorStateInfo(0);
        while (state.normalizedTime < 1f)
        {
            yield return null;
            state = CurtainAnimator.GetCurrentAnimatorStateInfo(0);
        }
    }

    private IEnumerator CloseCurtainRoutine()
    {
        CurtainVideoPlayer.clip = ClosingCurtainClip;
        CurtainVideoPlayer.Play();
        while (CurtainVideoPlayer.isPlaying)
            yield return null;
    }

    private IEnumerator OpenCurtainRoutine()
    {
        CurtainVideoPlayer.clip = OpeningCurtainClip;
        CurtainVideoPlayer.Play();
        while (CurtainVideoPlayer.isPlaying)
            yield return null;
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        yield return StartCoroutine(CloseCurtainRoutine());

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
            yield return null;

        asyncLoad.allowSceneActivation = true;

        yield return StartCoroutine(OpenCurtainRoutine());

    }

    private IEnumerator UnloadSceneRoutine(string sceneName)
    {
        yield return StartCoroutine(CloseCurtainRoutine());

        SceneManager.UnloadSceneAsync(sceneName);

        yield return StartCoroutine(OpenCurtainRoutine());
    }
}

