using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Collections.Generic;

public class CleanPlayEditor : EditorWindow {
    private static CleanPlayEditor window;
	private bool lastPlay = false;

    [MenuItem("Window/Clean Play")]
    static void Init() {
        window = (CleanPlayEditor) EditorWindow.GetWindow(typeof(CleanPlayEditor));
        window.Show();
    }

    void OnGUI() {
        GUILayout.Label("Clean Play", EditorStyles.boldLabel);
    }

    void Update() {
        if(EditorApplication.isPlaying) {
            if(lastPlay != EditorApplication.isPlaying) {
				SceneManager.LoadSceneAsync("main");
                lastPlay = EditorApplication.isPlaying;
            }
        } else {
            lastPlay = false;
        }
    }
}
