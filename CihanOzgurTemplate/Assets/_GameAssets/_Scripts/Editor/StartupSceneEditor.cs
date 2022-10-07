using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class StartupSceneEditor
{
    private const string _playFromFirstMenuText = "Edit/Always Start From Scene 0 &p";

    public static bool PlayFromFirstScene
    {
        get { return EditorPrefs.HasKey(_playFromFirstMenuText) && EditorPrefs.GetBool(_playFromFirstMenuText); }
        set { EditorPrefs.SetBool(_playFromFirstMenuText, value); }
    }

    [MenuItem(_playFromFirstMenuText, false, 150)]
    public static void PlayFromFirstSceneCheckMenu()
    {
        PlayFromFirstScene = !PlayFromFirstScene;
        Menu.SetChecked(_playFromFirstMenuText, PlayFromFirstScene);
    }

    [MenuItem(_playFromFirstMenuText, true)]
    public static bool PlayFromFirstSceneCheckMenuValidate()
    {
        Menu.SetChecked(_playFromFirstMenuText, PlayFromFirstScene);
        return true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void LoadFirstSceneAtGameBegins()
    {
        if (!PlayFromFirstScene)
            return;
        if (EditorBuildSettings.scenes.Length == 0)
        {
            Debug.LogWarning("The scene build list is empty. Can't play from first scene.");
            return;
        }

        foreach (GameObject go in UnityEngine.Object.FindObjectsOfType<GameObject>())
            go.SetActive(false);
        SceneManager.LoadScene(0);
    }
}