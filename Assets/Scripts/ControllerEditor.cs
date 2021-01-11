using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Controller))]
public class ControllerEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Controller controller = (Controller) target;
        if (GUILayout.Button("Générer un nuage de point"))
        {
            controller.GeneratePointCloud();
        }

        if (GUILayout.Button("Clear la scene"))
        {
            controller.ClearScene();
        }
    }
}