using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SocketIO;

public class ServerSelector : EditorWindow {
	private static ServerSelector window;
	private SocketIOComponent socketio;

	private int selected = 0;
	private List<string> options = new List<string>();
	private List<string> urls = new List<string>();

	[MenuItem("Window/Server Select")]
    static void Init() {
        window = (ServerSelector) EditorWindow.GetWindow(typeof(ServerSelector));
		window.titleContent = new GUIContent("Server Select");
        window.Show();
    }

	void OnEnable() {
		LoadServers();
	}
	
	private void LoadServers() {
        string filePath = Path.Combine(Application.streamingAssetsPath, "../servers.json");
        if(File.Exists(filePath)) {
            string dataAsJson = File.ReadAllText(filePath);
            ServerList loadedData = JsonUtility.FromJson<ServerList>(dataAsJson);
			for(int i = 0; i < loadedData.servers.Length; i++) {
				options.Add(loadedData.servers[i].name);
				urls.Add(loadedData.servers[i].url);
			}
        }
    }

    void OnGUI() {
		selected = EditorGUILayout.Popup("Server", selected, options.ToArray());
		EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("URL:", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
        EditorGUILayout.SelectableLabel(urls[selected], EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
    	EditorGUILayout.EndHorizontal();
    }

	void Update() {
        if(EditorApplication.isPlaying) {
			if(socketio != null) {
				if(socketio.url != urls[selected]) {
					socketio.url = urls[selected];
					socketio.Awake();
				}
			} else {
				socketio = GameObject.FindObjectOfType<SocketIOComponent>();
			}
        }
    }
}

[System.Serializable]
public class server {
    public string name;
    public string url;
}

[System.Serializable]
public class ServerList {
	public server[] servers;

    public static ServerList CreateFromJSON(string jsonString) {
        return JsonUtility.FromJson<ServerList>(jsonString);
    }
}