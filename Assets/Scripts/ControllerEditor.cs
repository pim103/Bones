using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Controller))]
public class ControllerEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Controller controller = (Controller) target;

        if (GUILayout.Button("Générer les bones"))
        {
            controller.GenerateBones();
        }

        if (GUILayout.Button("Some Test"))
        {
            controller.SomeTest();
        }

        if (GUILayout.Button("Clear la scene"))
        {
            controller.ClearScene();
        }
    }
}