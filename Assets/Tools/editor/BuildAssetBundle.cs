using UnityEditor;
using System.IO;
using UnityEngine;

public class BuildAssetBundle {

	[MenuItem("Tool/Asset Bundle/Clean All Cache Bundle")]
    public static void CleanCache ()
    {
        if (Caching.ClearCache ()) 
            Debug.Log("Successfully cleaned the cache.");
        else 
            Debug.Log("Cache is being used.");
    }

	[MenuItem("Tool/Asset Bundle/Build Asset Bundles Using BuildMap")]
    static void BuildMapABs()
    {
        // Create the array of bundle build details.
        AssetBundleBuild[] buildMap = new AssetBundleBuild[2];

        string[] enemyAssets = new string[2];
        enemyAssets[0] = "Assets/BundleData/Skills/Raycast Ability.asset";
        enemyAssets[1] = "Assets/BundleData/Skills/Raycast Ability1.asset";
//        enemyAssets[1] = "Assets/Textures/char_enemy_alienShip-damaged.jpg";

        buildMap[0].assetBundleName = "enemybundle";
        buildMap[0].assetNames = enemyAssets;


        string[] heroAssets = new string[1];
        heroAssets[0] = "char_hero_beanMan";
        buildMap[1].assetBundleName = "herobundle";
        buildMap[1].assetNames = heroAssets;

        // Choose the output path according to the build target.
        string outputPath = Path.Combine(AssetBundles.Utility.AssetBundlesOutputPath,  AssetBundles.Utility.GetPlatformName());
        if (!Directory.Exists(outputPath) )
            Directory.CreateDirectory (outputPath);

        BuildPipeline.BuildAssetBundles(outputPath, buildMap, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }

}
