using Accenture.eviola;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FPSCounter))]
public class FPSCounterEditor : Editor
{

    private FPSCounter GetTarget() { return (FPSCounter)target; }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUI.Header("Controls");
        EditorUI.Button("start", () => { GetTarget().StartCounting(); });
        EditorUI.Button("stop", () => { GetTarget().StopCounting(); });
        if (GetTarget().IsRunning())
        {
            EditorUI.Header("Output");
            GUILayout.Label("Fps: " + GetTarget().GetFps().ToString("#.##"));
        }
    }
}
