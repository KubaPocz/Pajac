using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class PlayFromMainMenu
{
    static PlayFromMainMenu()
    {
        var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/MainMenu.unity");
        if (scene != null)
            EditorSceneManager.playModeStartScene = scene;
    }
}