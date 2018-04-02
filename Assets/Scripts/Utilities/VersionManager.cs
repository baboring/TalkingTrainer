using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using System;

public class VersionManager
{
	static public VersionConfig local;
	static public VersionConfig remote;

//	static string dataPath = Application.temporaryCachePath;
	static string dataPath = Application.persistentDataPath;
	static string filename = "Version.json";

	public static VersionConfig LoadLocalVersion(string relativePath = "")
	{
		if(relativePath.Length > 0)
			filename = relativePath;
		var str = LoadFileText (filename);
		VersionConfig bundleConfig = null;

		bundleConfig = JsonMapper.ToObject<VersionConfig> (str == null? "": str );

		if(bundleConfig == null)
			bundleConfig = new VersionConfig();

		if (!Directory.Exists (dataPath + "/" + bundleConfig.bundleRelativePath))
			Directory.CreateDirectory (dataPath + "/" + bundleConfig.bundleRelativePath);

		return bundleConfig;
	}

	public static void UpdateVersionConfig(VersionConfig localVersionConfig)
	{
		string fullPathFileName = dataPath;
		if(localVersionConfig.bundleRelativePath.Length > 0)
		 	fullPathFileName += "/" + localVersionConfig.bundleRelativePath;
		fullPathFileName += "/"+filename;
		Common.Utilities.Logger.Debug("Update VersionConfig:"+fullPathFileName);
		// StreamWriter file = null;
		// file = File.CreateText(fullPathFileName);
		// file.Write(JsonMapper.ToJson (localVersionConfig));
		// file.Close();
		
		File.WriteAllText(fullPathFileName, JsonMapper.ToJson (localVersionConfig));
	}

	public static byte[] LoadFileMemory (string relativePath)
	{
		if (File.Exists (dataPath + "/" + relativePath)) {
			return File.ReadAllBytes (dataPath + "/" + relativePath);
		} else {
			if(relativePath.Contains (".")){
				relativePath = relativePath.Substring (0, relativePath.LastIndexOf ("."));
			}
			var objInResources = Resources.Load (relativePath, typeof( TextAsset )) as TextAsset;
			if(null == objInResources)
				return null;
			var memory = objInResources.bytes;
			Resources.UnloadAsset (objInResources);
			return memory;
		}
	}

	public static string LoadFileText (string relativePath)
	{
		if (File.Exists (dataPath + "/" + relativePath)) {
			return File.ReadAllText (dataPath + "/" + relativePath);
		} else {
			if(relativePath.Contains (".")){
				relativePath = relativePath.Substring (0, relativePath.LastIndexOf ("."));
			}
            var objResources = Resources.Load<TextAsset>( relativePath );
			if(null == objResources)
				return null;
            var text = objResources.text;
            Resources.UnloadAsset( objResources );
            return text;
		}
	}

    public static string LoadResourcesText( string path )
    {
        string result = string.Empty;
        string resourcePath = string.Format( @"Assets/Resources/{0}", path );
        if( File.Exists( resourcePath ) )
        {
            result = File.ReadAllText( resourcePath );
        }
        return result;
    }

	private static string LoadMemoryString (string relativePath)
	{
		return BitConverter.ToString(LoadFileMemory (relativePath));
	}


	[System.Serializable]
	public class VersionConfig
	{
		public string versionNum = "";
		public string bundleRelativePath = "";
		public List<BundleInfo> bundles = new List<BundleInfo>();

		public bool IsExist(string url) {
			return (null != Find(url));
		}
		public BundleInfo Find(string url) {
			int found = bundles.FindIndex(va=> va.url == url);
			if( found > -1)
				return bundles[found];
			return null;
		}
	}

	[System.Serializable]
	public class BundleInfo
	{
		public string url = "";
		public string name = "";
		public string md5 = "";
		public int size = 0;
		public string[] include = new string[0];
		public string[] dependency = new string[0];

		// public bool isExist()
		// {
		// 	var existBundles = BundleManager.GetInstance ().LocalVersionConfig.bundles;
		// 	foreach( var item in existBundles ) {
		// 		if( item.name == name && item.md5 == md5 )
		// 			return true;
		// 	}
		// 	return false;
		// }

		public BundleInfo()
		{
			
		}
#if UNITY_EDITOR
		public BundleInfo(string name)
		{
			this.name = name;
			Update ();
		}
		void Update()
		{
			UpdateInclude ();
			UpdateDependency ();
		}

		void UpdateInclude()
		{
			include = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle( name );
		}

		void UpdateDependency()
		{
			dependency = UnityEditor.AssetDatabase.GetDependencies( UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle( name ) );
		}
#endif
	}
}

