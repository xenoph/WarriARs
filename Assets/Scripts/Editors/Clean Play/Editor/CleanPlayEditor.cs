using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections.Generic;

public class CleanPlayEditor : EditorWindow {

    private bool lastPlay = false;

    private Dictionary<string, bool> enabledScenes = new Dictionary<string, bool>();
    private static CleanPlayEditor window;

    [MenuItem("Window/Clean Play")]
    static void Init() {
        window = (CleanPlayEditor) EditorWindow.GetWindow(typeof(CleanPlayEditor));
        window.Show();
    }

    void OnGUI() {
        GUILayout.Label("Clean Play", EditorStyles.boldLabel);
        /*
        if(EditorApplication.isPlaying) {
            GUILayout.Label("Cannot be changed in Play Mode");
        } else {
            for(int i = 0; i < SceneManager.sceneCount; i++) {
                string name = SceneManager.GetSceneAt(i).name;
                bool enabled = true;
                if(enabledScenes.ContainsKey(name)) {
                    enabledScenes.TryGetValue(name, out enabled);
                } else {
                    enabledScenes.Add(name, enabled);
                }
                enabled = EditorGUILayout.Toggle(name, enabled);
                enabledScenes[name] = enabled;
            }
        }
        */
        //EditorGUILayout.EndToggleGroup();
    }

    void Update() {
        if(EditorApplication.isPlaying) {
            if(lastPlay != EditorApplication.isPlaying) {
                for(int i = 0; i < SceneManager.sceneCount; i++) {
                    string name = SceneManager.GetSceneAt(i).name;
                    if(name != "main") {
                        SceneManager.UnloadSceneAsync(name);
                    }
                }
                lastPlay = EditorApplication.isPlaying;
            }
        } else {
            lastPlay = false;
        }
    }
}
