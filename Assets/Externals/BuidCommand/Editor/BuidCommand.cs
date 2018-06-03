using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;

/// 
/// Put me inside an Editor folder
/// 
/// Add a Build menu on the toolbar to automate multiple build for different platform
/// 
/// Use #define BUILD in your code if you have build specification 
/// Specify all your Target to build All
/// 
/// Install to Android device using adb install -r "pathofApk"
/// 
public class BuidCommand : MonoBehaviour
{
    const string androidKeystorePass = "";
    const string androidKeyaliasName = "";
    const string androidKeyaliasPass = "";

    public static string ProductName {
        get
        {
            return PlayerSettings.productName;
        }
    }

    private static string BuildPathRoot {
        get
        {
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), ProductName);
            if(!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            return path;
        }
    }

    static int AndroidLastBuildVersionCode {
        get
        {
            return PlayerPrefs.GetInt("LastVersionCode", -1);
        }
        set
        {
            PlayerPrefs.SetInt("LastVersionCode", value);
        }
    }

    static BuildTargetGroup ConvertBuildTarget(BuildTarget buildTarget)
    {
        switch(buildTarget)
        {
            case BuildTarget.StandaloneOSX:
            case BuildTarget.iOS:
                return BuildTargetGroup.iOS;
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneLinux:
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneLinux64:
            case BuildTarget.StandaloneLinuxUniversal:
                return BuildTargetGroup.Standalone;
            case BuildTarget.Android:
                return BuildTargetGroup.Android;
            case BuildTarget.WebGL:
                return BuildTargetGroup.WebGL;
            case BuildTarget.WSAPlayer:
                return BuildTargetGroup.WSA;
            case BuildTarget.Tizen:
                return BuildTargetGroup.Tizen;
            case BuildTarget.PSP2:
                return BuildTargetGroup.PSP2;
            case BuildTarget.PS4:
                return BuildTargetGroup.PS4;
            case BuildTarget.PSM:
                return BuildTargetGroup.PSM;
            case BuildTarget.XboxOne:
                return BuildTargetGroup.XboxOne;
            case BuildTarget.N3DS:
                return BuildTargetGroup.N3DS;
            case BuildTarget.WiiU:
                return BuildTargetGroup.WiiU;
            case BuildTarget.tvOS:
                return BuildTargetGroup.tvOS;
            case BuildTarget.Switch:
                return BuildTargetGroup.Switch;
            case BuildTarget.NoTarget:
            default:
                return BuildTargetGroup.Standalone;
        }
    }
    static string GetExtension(BuildTarget buildTarget)
    {
        switch(buildTarget)
        {
            case BuildTarget.StandaloneOSX:
                break;
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return ".exe";
            case BuildTarget.iOS:
                break;
            case BuildTarget.Android:
                return ".apk";
            case BuildTarget.StandaloneLinux:
                break;
            case BuildTarget.WebGL:
                break;
            case BuildTarget.WSAPlayer:
                break;
            case BuildTarget.StandaloneLinux64:
                break;
            case BuildTarget.StandaloneLinuxUniversal:
                break;
            case BuildTarget.Tizen:
                break;
            case BuildTarget.PSP2:
                break;
            case BuildTarget.PS4:
                break;
            case BuildTarget.PSM:
                break;
            case BuildTarget.XboxOne:
                break;
            case BuildTarget.N3DS:
                break;
            case BuildTarget.WiiU:
                break;
            case BuildTarget.tvOS:
                break;
            case BuildTarget.Switch:
                break;
            case BuildTarget.NoTarget:
                break;
            default:
                break;
        }

        return ".unknown";
    }

    static BuildPlayerOptions GetDefaultPlayerOptions()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        List<string> listScenes = new List<string>();
        foreach(var s in EditorBuildSettings.scenes)
        {
            if(s.enabled)
                listScenes.Add(s.path);
        }

        buildPlayerOptions.scenes = listScenes.ToArray();
        buildPlayerOptions.options = BuildOptions.None;

        // To define
        // buildPlayerOptions.locationPathName = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\LightGunBuild\\Android\\LightGunMouseArcadeRoom.apk";
        // buildPlayerOptions.target = BuildTarget.Android;

        return buildPlayerOptions;
    }

    static void DefaultBuild(BuildTarget buildTarget)
    {
        BuildTargetGroup targetGroup = ConvertBuildTarget(buildTarget);

        string buildName = string.Format("{0} ({1}) {2} [{3}]", ProductName
                                            , PlayerSettings.Android.bundleVersionCode
                                            , System.DateTime.Now.ToString("yyyy.MM.dd-HH") + System.DateTime.Now.ToString("mm")
                                            , buildTarget);

        string path = Path.Combine(Path.Combine(BuildPathRoot, targetGroup.ToString()), buildName);
        string name = ProductName + GetExtension(buildTarget);


        string defineSymbole = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineSymbole + ";BUILD");

        PlayerSettings.Android.keystorePass = androidKeystorePass;
        PlayerSettings.Android.keyaliasName = androidKeyaliasName;
        PlayerSettings.Android.keyaliasPass = androidKeyaliasPass;

        BuildPlayerOptions buildPlayerOptions = GetDefaultPlayerOptions();

        buildPlayerOptions.locationPathName = Path.Combine(path, name);
        buildPlayerOptions.target = buildTarget;

        EditorUserBuildSettings.SwitchActiveBuildTarget(targetGroup, buildTarget);

        string result = buildPlayerOptions.locationPathName + ": " + BuildPipeline.BuildPlayer(buildPlayerOptions);
        Debug.Log(result);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defineSymbole);

        if(buildTarget == BuildTarget.Android)
            AndroidLastBuildVersionCode = PlayerSettings.Android.bundleVersionCode;

        UnityEditor.EditorUtility.RevealInFinder(path);
    }


    //[MenuItem("Build/Build Specific/Build Win32")]
    static void BuildWin32()
    {
        DefaultBuild(BuildTarget.StandaloneWindows);
    }

    [MenuItem("Build/Build Specific/Build Win64")]
    static void BuildWin64()
    {
        DefaultBuild(BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Build/Get Build Number")]
    static void BuildNumber()
    {
        Debug.Log("Current/Last: " + PlayerSettings.Android.bundleVersionCode + "/" + AndroidLastBuildVersionCode);
    }

    [MenuItem("Build/Build Number/Up Build Number")]
    static void BuildNumberUp()
    {
        PlayerSettings.Android.bundleVersionCode++;
        BuildNumber();
    }

    [MenuItem("Build/Build Number/Down Build Number")]
    static void BuildNumberDown()
    {
        PlayerSettings.Android.bundleVersionCode--;
        BuildNumber();
    }


}
#endif