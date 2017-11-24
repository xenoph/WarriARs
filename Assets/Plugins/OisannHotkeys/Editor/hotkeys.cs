using UnityEditor;
using UnityEngine;

namespace Oisann.Editor {
	public class hotkeys : MonoBehaviour {
        [MenuItem("Assets/Open C# Project %&O")]
        private static void OpenProject() {
            EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
        }
    }
}