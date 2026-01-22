using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;


public class CurtainManager : MonoBehaviour
{
    public static CurtainManager Instance;

    [Header("Curtain Settings")]
    public Animator CurtainAnimator;
    public RawImage curtainImageDisplay;  // U¿ywamy RawImage do wyœwietlania obrazów
    public string openImageFolder = "Image/opening"; // Folder w Resources do obrazów otwierania
    public string closeImageFolder = "Image/closing"; // Folder w Resources do obrazów zamykania
    public float frameRate = 32f; // Liczba klatek na sekundê
    private Texture[] openingCurtainFrames;  // Tablica z obrazami do otwierania
    private Texture[] closingCurtainFrames;  // Tablica z obrazami do zamykania
    private int currentFrame = 0;
    private float timePerFrame;

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
        // Za³aduj obrazy z obu folderów
        openingCurtainFrames = Resources.LoadAll<Texture>(openImageFolder);
        closingCurtainFrames = Resources.LoadAll<Texture>(closeImageFolder);
        timePerFrame = 1f / frameRate;  // Czas trwania jednej klatki
        curtainImageDisplay.texture = closingCurtainFrames[0];
    }

    public void ChangeScene(string sceneToLoad, string sceneToUnload,bool showCurtain)
    {
        StartCoroutine(ShowCurtainAndChangeSceneRoutine(sceneToLoad, sceneToUnload, showCurtain));
    }

    private IEnumerator ShowCurtainAndChangeSceneRoutine(string sceneToLoad, string sceneToUnload, bool showCurtain)
    {
        // 1. Pokazanie zas³ony (animacja)
        if(showCurtain) yield return StartCoroutine(ShowCurtainRoutine());
        yield return new WaitForSeconds(0.5f);
        // 2. Odtwarzanie sekwencji obrazów (zamykanie zas³ony)
        yield return StartCoroutine(PlayCurtainClosingSequence());
        // 3. Zmiana sceny po zakoñczeniu sekwencji zamykania
        yield return StartCoroutine(ChangeSceneWithCurtainRoutine(sceneToLoad, sceneToUnload));
        // 4. Otwieranie zas³ony po za³adowaniu nowej sceny
        yield return StartCoroutine(OpenCurtainRoutine());
        
    }

    private IEnumerator ChangeSceneWithCurtainRoutine(string sceneToLoad, string sceneToUnload)
    {
        // 3. Zmiana sceny po zakoñczeniu zamkniêcia zas³ony
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
    }

    private IEnumerator ShowCurtainRoutine()
    {
        // 1. Pokazanie zas³ony (animacja)
        CurtainAnimator.SetTrigger("ShowCurtain");
        AnimatorStateInfo state = CurtainAnimator.GetCurrentAnimatorStateInfo(0);
        while (state.normalizedTime < 1f)
        {
            yield return null;
            state = CurtainAnimator.GetCurrentAnimatorStateInfo(0);
        }
    }

    private IEnumerator PlayCurtainClosingSequence()
    {
        // 2. Odtwarzanie sekwencji obrazów (zamykanie zas³ony)
        AudioManager.PlayCurtainCloseSound(); //zagranie dzwi?ku zamykanej zas?ony
        for (int i = 0; i < closingCurtainFrames.Length; i++)
        {
            curtainImageDisplay.texture = closingCurtainFrames[i];
            yield return new WaitForSeconds(timePerFrame);  // Czas na ka¿d¹ klatkê
        }
    }

    private IEnumerator OpenCurtainRoutine()
    {
        // 4. Otwieranie zas³ony po za³adowaniu nowej sceny
        AudioManager.PlayCurtainOpenSound();//zagranie dzwi?ku otwieranej zas?ony
        for (int i = 0; i < openingCurtainFrames.Length; i++)
        {
            curtainImageDisplay.texture = openingCurtainFrames[i];
            yield return new WaitForSeconds(timePerFrame);  // Czas na ka¿d¹ klatkê
        }
    }

}
