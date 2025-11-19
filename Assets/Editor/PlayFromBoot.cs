using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class PlayFromBoot
{
    static PlayFromBoot()
    {
        var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Boot.unity");
        if (scene != null)
            EditorSceneManager.playModeStartScene = scene;
    }
}