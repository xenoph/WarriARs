using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AutoBuilder : MonoBehaviour {

    [MenuItem("Auto Builder/Android")]
    static void BuildNightlyAndroid() {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        List<string> scenes = new List<string>();
        foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
            scenes.Add(scene.path);
        }

        string date = DateTime.Now.ToString("yyyy") + DateTime.Now.ToString("MM") + DateTime.Now.ToString("dd"); // + DateTime.Now.ToString("HH") + DateTime.Now.ToString("mm") + DateTime.Now.ToString("ss");

        buildPlayerOptions.scenes = scenes.ToArray();
        buildPlayerOptions.locationPathName = "WarriARs-NIGHTLY-" + date + ".apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}
