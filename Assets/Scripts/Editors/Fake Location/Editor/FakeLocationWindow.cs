using UnityEngine;
using UnityEditor;

public class FakeLocationWindow : CustomWebViewEditorWindow {

    [MenuItem("Window/Fake Location")]
    static void Open() {
        string projectpath = Application.dataPath.Remove(Application.dataPath.LastIndexOf("/Assets"));
        CreateWebViewEditorWindow<FakeLocationWindow>(
            "Fake GPS Location",
            projectpath + "/FakeLocationSettings/Website/index.html", 200, 530, 800, 600);
    }

    public void UpdateCoords(string coordJSON) {
        CustomLocation loc = JsonUtility.FromJson<CustomLocation>(coordJSON);
        if(GameController.instance != null) {
            GameController.instance.currentLocation.SetLocation(loc.lat, loc.lng);
        } else {
            Debug.LogError("No GameManager found.");
        }
    }

    public void ToggleMode(int i) {
        if(i == 0) {
            //Use GPS
            Debug.Log("Fake Location OFF.");
        } else {
            //Use fake location
            Debug.Log("Fake Location ON.");
        }
    }

    public struct CustomLocation {
        public float lat, lng;
    }
}