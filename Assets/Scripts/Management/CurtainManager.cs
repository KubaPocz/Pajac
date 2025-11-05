using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using System.Collections;

public class CurtainManager : MonoBehaviour
{
    [Header("Curtain Settings")]
    public Animator CurtainAnimator;
    public VideoPlayer CurtainVideoPlayer;
    public VideoClip OpeningCurtainClip;
    public VideoClip ClosingCurtainClip;

    private void Start()
    {
        LoadSceneSequenceAfterAnimation("Statistics");
    }
    public void LoadSceneSequence(string sceneName)
    {
        StartCoroutine(LoadSceneSequenceRoutine(sceneName, false));
    }

    public void LoadSceneSequenceAfterAnimation(string sceneName)
    {
        StartCoroutine(LoadSceneSequenceRoutine(sceneName, true));
    }

    private IEnumerator LoadSceneSequenceRoutine(string sceneName, bool waitForAnimation)
    {
        if (waitForAnimation)
        {
            yield return StartCoroutine(WaitForCurtainAnimation());
        }

        CurtainVideoPlayer.clip = ClosingCurtainClip;
        CurtainVideoPlayer.Play();
        while (CurtainVideoPlayer.isPlaying)
            yield return null;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false;

        // Czekamy a¿ scena siê za³aduje do 90–100%
        while (asyncLoad.progress < 0.9f)
            yield return null;

        CurtainVideoPlayer.clip = OpeningCurtainClip;
        CurtainVideoPlayer.Play();
        while (CurtainVideoPlayer.isPlaying)
            yield return null;
        asyncLoad.allowSceneActivation = true;

    }

    private IEnumerator WaitForCurtainAnimation()
    {
        CurtainAnimator.SetTrigger("ShowCurtain");
        AnimatorStateInfo state = CurtainAnimator.GetCurrentAnimatorStateInfo(0);

        while (state.normalizedTime < 1f)
        {
            yield return null;
            state = CurtainAnimator.GetCurrentAnimatorStateInfo(0);
        }
    }
}
