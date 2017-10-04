using System;
using UnityEditor;
using System.Reflection;
using System.IO;


//Fixed version of this: https://gist.github.com/anchan828/2405b9b792366327e502
//Source: http://qiita.com/kyusyukeigo/items/71db22676c6f4743913e
public class CustomWebViewEditorWindow {
    private object webViewEditorWindow = null;

    static Type webViewEditorWindowType {
        get {
#if UNITY_5_4_OR_NEWER
            return (typeof(Editor).Assembly).GetType("UnityEditor.Web.WebViewEditorWindowTabs");
#else
            return Types.GetType("UnityEditor.Web.WebViewEditorWindow", "UnityEditor.dll");
#endif
        }
    }

    const string PATH = "Temp/webViewEditorWindowNames.txt";

    [InitializeOnLoadMethod]
    static void AddGlobalObjects() {

        if(File.Exists(PATH)) {

            foreach(var globalObjectName in File.ReadAllLines(PATH)) {
                var type = Type.GetType(globalObjectName);

                if(type == null)
                    continue;
                AddGlobalObject(type);
            }
        }

    }

    public static T CreateWebViewEditorWindow<T>(string title, string sourcesPath, int minWidth, int minHeight, int maxWidth, int maxHeight) where T : CustomWebViewEditorWindow, new() {
        var createMethod = webViewEditorWindowType.GetMethod("Create", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).MakeGenericMethod(webViewEditorWindowType);

        var window = createMethod.Invoke(null, new object[] {
            title,
            sourcesPath,
            minWidth,
            minHeight,
            maxWidth,
            maxHeight
        });


        var customWebEditorWindow = new T {
            webViewEditorWindow = window
        };

        EditorApplication.delayCall += () => {
            EditorApplication.delayCall += () => {
                var webView = webViewEditorWindowType.GetField("m_WebView", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(customWebEditorWindow.webViewEditorWindow);
                AddGlobalObject<T>();
            };
        };


        return customWebEditorWindow;
    }

    private static void AddGlobalObject<T>() where T : CustomWebViewEditorWindow {
        File.AppendAllText("Temp/webViewEditorWindowNames.txt", typeof(T).Name + "\n", System.Text.Encoding.UTF8);
        AddGlobalObject(typeof(T));
    }

    private static void AddGlobalObject(Type type) {
#if UNITY_5_4_OR_NEWER
        var jsproxyMgrType = (typeof(Editor).Assembly).GetType("UnityEditor.Web.JSProxyMgr");
#else
        var jsproxyMgrType = Types.GetType("UnityEditor.Web.JSProxyMgr", "UnityEditor.dll");
#endif
        var instance = jsproxyMgrType.GetMethod("GetInstance").Invoke(null, new object[0]);

        jsproxyMgrType.GetMethod("AddGlobalObject").Invoke(instance, new object[] {
            type.Name,
            Activator.CreateInstance (type)
        });
    }

    public void InvokeJSMethod(string objectName, string name, params object[] args) {
        var invokeJSMethodMethod = webViewEditorWindowType.GetMethod("InvokeJSMethod", BindingFlags.NonPublic | BindingFlags.Instance);
        invokeJSMethodMethod.Invoke(webViewEditorWindow, new object[] {
            objectName, name, args
        });
    }
}