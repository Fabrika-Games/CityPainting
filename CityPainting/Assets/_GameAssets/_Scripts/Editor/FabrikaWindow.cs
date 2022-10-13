using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

public class FabrikaWindow : EditorWindow
{
    [MenuItem("CustomWindows/FabrikaWindow")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FabrikaWindow));
    }

    void OnGUI()
    {
        if (GUILayout.Button("AddCube"))
        {
            AddCube();
        }
    }


    public void AddCube()
    {
        if (Selection.activeGameObject != null)
        {
            Level _level = Selection.activeGameObject.GetComponentInParent<Level>();
            if (_level != null)
            {
                Cube _cube = Instantiate(Resources.Load<Cube>("Cube"), _level.Cubes.transform);
                _cube.TargetGameObjects = Selection.gameObjects.Where(qq => qq.GetComponent<Renderer>())
                    .Select(qq => qq.GetComponent<Renderer>()).ToList();
                Selection.activeGameObject = _cube.gameObject;
                for (int i = 0; i < _cube.TargetGameObjects.Count; i++)
                {
                    _cube.TargetGameObjects[i].enabled = false;
                }
            }
        }
    }
}