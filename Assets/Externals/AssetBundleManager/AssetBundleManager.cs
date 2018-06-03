using UnityEngine;
#if UNITY_EDITOR	
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

/*
 	In this demo, we demonstrate:
	1.	Automatic asset bundle dependency resolving & loading.
		It shows how to use the manifest assetbundle like how to get the dependencies etc.
	2.	Automatic unloading of asset bundles (When an asset bundle or a dependency thereof is no longer needed, the asset bundle is unloaded)
	3.	Editor simulation. A bool defines if we load asset bundles from the project or are actually using asset bundles(doesn't work with assetbundle variants for now.)
		With this, you can player in editor mode without actually building the assetBundles.
	4.	Optional setup where to download all asset bundles
	5.	Build pipeline build postprocessor, integration so that building a player builds the asset bundles and puts them into the player data (Default implmenetation for loading assetbundles from disk on any platform)
	6.	Use WWW.LoadFromCacheOrDownload and feed 128 bit hash to it when downloading via web
		You can get the hash from the manifest assetbundle.
	7.	AssetBundle variants. A prioritized list of variants that should be used if the asset bundle with that variant exists, first variant in the list is the most preferred etc.
*/

namespace AssetBundles
{	
	using Common.Utilities;
	// Loaded assetBundle contains the references count which can be used to unload dependent assetBundles automatically.
	public class LoadAssetBundleInfo {
		public string assetBundleName;
		public bool isManifestBundle;

		public WWW download;
		public AssetBundle m_AssetBundle;
		public int m_ReferencedCount;
		

		public LoadAssetBundleInfo(string bundleName, bool isManifest = false) {
			assetBundleName = bundleName;
			isManifestBundle = isManifest;
		}
	}	

	
	// Class takes care of loading assetBundle and its dependencies automatically, loading variants automatically.
	public class AssetBundleManager : MonoBehaviour
	{
		public enum LogMode { All, JustErrors };
		public enum LogType { Info, Warning, Error };
	
		static LogMode m_LogMode = LogMode.All;
		static string m_BaseDownloadingURL = "";
		static string[] m_ActiveVariants =  {  };
		static AssetBundleManifest m_AssetBundleManifest = null;
	#if UNITY_EDITOR	
		static int m_SimulateAssetBundleInEditor = -1;
		const string kSimulateAssetBundles = "SimulateAssetBundles";
	#endif
	
		static Dictionary<string, LoadAssetBundleInfo> m_LoadedAssetBundles = new Dictionary<string, LoadAssetBundleInfo> ();
		static Dictionary<string, LoadAssetBundleInfo> m_DownloadingWWWs = new Dictionary<string, LoadAssetBundleInfo> ();
		static Dictionary<string, string> m_DownloadingErrors = new Dictionary<string, string> ();
		static List<AssetBundleLoadOperation> m_InProgressOperations = new List<AssetBundleLoadOperation> ();
		static Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]> ();
	
		public static LogMode logMode
		{
			get { return m_LogMode; }
			set { m_LogMode = value; }
		}
	
		// The base downloading url which is used to generate the full downloading url with the assetBundle names.
		public static string BaseDownloadingURL
		{
			get { return m_BaseDownloadingURL; }
			set { m_BaseDownloadingURL = value; }
		}
	
		// Variants which is used to define the active variants.
		public static string[] ActiveVariants
		{
			get { return m_ActiveVariants; }
			set { m_ActiveVariants = value; }
		}

		public static int InProgressCount {
			get { return m_InProgressOperations.Count; }
		}
		public static int DownloadingCount {
			get { return m_DownloadingWWWs.Count; }
		}

		
	
		// AssetBundleManifest object which can be used to load the dependecies and check suitable assetBundle variants.
		public static AssetBundleManifest AssetBundleManifestObject
		{
			set {m_AssetBundleManifest = value; }
		}
	
		private static void Log(LogType logType, string text)
		{
			if (logType == LogType.Error)
				Logger.LogError("[AssetBundleManager] " + text);
			else if (m_LogMode == LogMode.All)
				Logger.Debug("[AssetBundleManager] " + text);
		}
	
	#if UNITY_EDITOR
		// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
		public static bool SimulateAssetBundleInEditor 
		{
			get
			{
				if (m_SimulateAssetBundleInEditor == -1)
					m_SimulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;
				
				return m_SimulateAssetBundleInEditor != 0;
			}
			set
			{
				int newValue = value ? 1 : 0;
				if (newValue != m_SimulateAssetBundleInEditor)
				{
					m_SimulateAssetBundleInEditor = newValue;
					EditorPrefs.SetBool(kSimulateAssetBundles, value);
				}
			}
		}
		
	
		#endif
	
		private static string GetStreamingAssetsPath()
		{
			if (Application.isEditor)
				return "file://" +  System.Environment.CurrentDirectory.Replace("\\", "/"); // Use the build output folder directly.
			//else if (Application.isWebPlayer)
				//return System.IO.Path.GetDirectoryName(Application.absoluteURL).Replace("\\", "/")+ "/StreamingAssets";
			else if (Application.isMobilePlatform || Application.isConsolePlatform)
				return Application.streamingAssetsPath;
			else // For standalone player.
				return "file://" +  Application.streamingAssetsPath;
		}
	
		public static void SetSourceAssetBundleDirectory(string relativePath)
		{
			BaseDownloadingURL = GetStreamingAssetsPath() + relativePath;
		}
		
		public static void SetSourceAssetBundleURL(string absolutePath)
		{
			BaseDownloadingURL = absolutePath + Utility.GetPlatformName() + "/";
		}
	
		public static void SetDevelopmentAssetBundleServer()
		{
			#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't have to setup a download URL
			if (SimulateAssetBundleInEditor)
				return;
			#endif
			
			TextAsset urlFile = Resources.Load("AssetBundleServerURL") as TextAsset;
			string url = (urlFile != null) ? urlFile.text.Trim() : null;
			if (url == null || url.Length == 0)
			{
				Log(LogType.Error,"Development Server URL could not be found.");
				AssetBundleManager.SetSourceAssetBundleURL("http://localhost:7888/" + Utility.GetPlatformName() + "/");
			}
			else
			{
				AssetBundleManager.SetSourceAssetBundleURL(url);
			}
		}
		
		// Get loaded AssetBundle, only return vaild object when all the dependencies are downloaded successfully.
		static public LoadAssetBundleInfo GetLoadedAssetBundle (string assetBundleName, out string error)
		{
			if (m_DownloadingErrors.TryGetValue(assetBundleName, out error) )
				return null;
		
			LoadAssetBundleInfo bundle = null;
			m_LoadedAssetBundles.TryGetValue(assetBundleName, out bundle);
			if (bundle == null)
				return null;
			
			// No dependencies are recorded, only the bundle itself is required.
			string[] dependencies = null;
			if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies) )
				return bundle;
			
			// Make sure all dependencies are loaded
			foreach(var dependency in dependencies)
			{
				if (m_DownloadingErrors.TryGetValue(assetBundleName, out error) )
					return bundle;
	
				// Wait all the dependent assetBundles being loaded.
				LoadAssetBundleInfo dependentBundle;
				m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
				if (dependentBundle == null)
					return null;
			}
	
			return bundle;
		}
	
		static public AssetBundleLoadManifestOperation Initialize ()
		{
			Log(LogType.Info,"Initialize");
			return Initialize(Utility.GetPlatformName());
		}
	
		// Load AssetBundleManifest.
		static public AssetBundleLoadManifestOperation Initialize (string manifestAssetBundleName)
		{
	#if UNITY_EDITOR
			Log (LogType.Info, "Simulation Mode: " + (SimulateAssetBundleInEditor ? "Enabled" : "Disabled"));
	#endif
	
			var go = new GameObject("AssetBundleManager", typeof(AssetBundleManager));
			DontDestroyOnLoad(go);
		
	#if UNITY_EDITOR	
			// If we're in Editor simulation mode, we don't need the manifest assetBundle.
			if (SimulateAssetBundleInEditor)
				return null;
	#endif

			VersionManager.local = VersionManager.LoadLocalVersion();
	
			_loadAssetBundle(new LoadAssetBundleInfo(manifestAssetBundleName, true));
			var operation = new AssetBundleLoadManifestOperation (manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
			m_InProgressOperations.Add (operation);
			return operation;
		}
		
		// Load AssetBundle and its dependencies.
		static protected void _loadAssetBundle(LoadAssetBundleInfo info)
		{
			Log(LogType.Info, "Attempt to Load Asset Bundle " + info.assetBundleName + (info.isManifestBundle? "[Manifest]":""));
	
	#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't have to really load the assetBundle and its dependencies.
			if (SimulateAssetBundleInEditor)
				return;
	#endif
	
			if (!info.isManifestBundle)
			{
				if (m_AssetBundleManifest == null)
				{
					Log(LogType.Error,"Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
					return;
				}
			}
	
			// Check if the assetBundle has already been processed.
			bool isAlreadyProcessed = _loadAssetBundleInternal(info);
	
			// Load dependencies.
			if (!isAlreadyProcessed && !info.isManifestBundle)
				_loadDependencies(info.assetBundleName);
		}
		
		// Remaps the asset bundle name to the best fitting asset bundle variant.
		static protected string RemapVariantName(string assetBundleName)
		{
			string[] bundlesWithVariant = m_AssetBundleManifest.GetAllAssetBundlesWithVariant();

			string[] split = assetBundleName.Split('.');

			int bestFit = int.MaxValue;
			int bestFitIndex = -1;
			// Loop all the assetBundles with variant to find the best fit variant assetBundle.
			for (int i = 0; i < bundlesWithVariant.Length; i++)
			{
				string[] curSplit = bundlesWithVariant[i].Split('.');
				if (curSplit[0] != split[0])
					continue;
				
				int found = System.Array.IndexOf(m_ActiveVariants, curSplit[1]);
				
				// If there is no active variant found. We still want to use the first 
				if (found == -1)
					found = int.MaxValue-1;
						
				if (found < bestFit)
				{
					bestFit = found;
					bestFitIndex = i;
				}
			}
			
			if (bestFit == int.MaxValue-1)
			{
				Log(LogType.Warning,"Ambigious asset bundle variant chosen because there was no matching active variant: " + bundlesWithVariant[bestFitIndex]);
			}
			
			if (bestFitIndex != -1)
			{
				return bundlesWithVariant[bestFitIndex];
			}
			else
			{
				return assetBundleName;
			}
		}
	
		// Where we actuall call WWW to download the assetBundle.
		static protected bool _loadAssetBundleInternal (LoadAssetBundleInfo info)
		{
			// Already loaded.
			LoadAssetBundleInfo bundle = null;
			m_LoadedAssetBundles.TryGetValue(info.assetBundleName, out bundle);
			if (bundle != null)
			{
				bundle.m_ReferencedCount++;
				return true;
			}
	
			// @TODO: Do we need to consider the referenced count of WWWs?
			// In the demo, we never have duplicate WWWs as we wait LoadAssetAsync()/LoadLevelAsync() to be finished before calling another LoadAssetAsync()/LoadLevelAsync().
			// But in the real case, users can call LoadAssetAsync()/LoadLevelAsync() several times then wait them to be finished which might have duplicate WWWs.
			if (m_DownloadingWWWs.ContainsKey(info.assetBundleName) )
				return true;
	
			string url = m_BaseDownloadingURL + info.assetBundleName;
		
			//Log(LogType.Info,"[ Start Download ] : " + url);
			
			// For manifest assetbundle, always download it as we don't have hash for it.
			if (info.isManifestBundle)
				info.download = new WWW(url);
			else
				info.download = WWW.LoadFromCacheOrDownload(url, m_AssetBundleManifest.GetAssetBundleHash(info.assetBundleName), 0); 
	
			m_DownloadingWWWs.Add(info.assetBundleName, info);

			Log(LogType.Info,"[ Downloading ] : " + DownloadingCount);
	
			return false;
		}
	
		// Where we get all the dependencies and load them all.
		static protected void _loadDependencies(string assetBundleName)
		{
			Log(LogType.Info,"-- Load dependencies from " + assetBundleName);
			if (m_AssetBundleManifest == null)
			{
				Log(LogType.Error,"Please initialize AssetBundleManifest by calling AssetBundleManager.Initialize()");
				return;
			}
	
			// Get dependecies from the AssetBundleManifest object..
			string[] dependencies = m_AssetBundleManifest.GetAllDependencies(assetBundleName);
			if (dependencies.Length == 0)
				return;

			if (m_Dependencies.ContainsKey(assetBundleName)){
				Log(LogType.Error,"Already has dependency bundle : " + assetBundleName);
				return;
			}
			
			for (int i=0;i<dependencies.Length;i++)
				dependencies[i] = RemapVariantName (dependencies[i]);
				
			// Record and load all dependencies.
			m_Dependencies.Add(assetBundleName, dependencies);
			for (int i=0;i<dependencies.Length;i++)
				_loadAssetBundleInternal(new LoadAssetBundleInfo(dependencies[i]));
		}
	
		// Unload assetbundle and its dependencies.
		static public void UnloadAssetBundle(string assetBundleName)
		{
	#if UNITY_EDITOR
			// If we're in Editor simulation mode, we don't have to load the manifest assetBundle.
			if (SimulateAssetBundleInEditor)
				return;
	#endif
	
			//Log(LogType.Info,m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + assetBundleName);
	
			_unloadAssetBundleInternal(assetBundleName);
			_unloadDependencies(assetBundleName);
	
			//Log(LogType.Info,m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + assetBundleName);
		}
	
		static protected void _unloadDependencies(string assetBundleName)
		{
			string[] dependencies = null;
			if (!m_Dependencies.TryGetValue(assetBundleName, out dependencies) )
				return;
	
			// Loop dependencies.
			foreach(var dependency in dependencies)
			{
				_unloadAssetBundleInternal(dependency);
			}
	
			m_Dependencies.Remove(assetBundleName);
		}
	
		static protected void _unloadAssetBundleInternal(string assetBundleName)
		{
			string error;
			LoadAssetBundleInfo bundle = GetLoadedAssetBundle(assetBundleName, out error);
			if (bundle == null)
				return;
	
			if (--bundle.m_ReferencedCount == 0)
			{
				bundle.m_AssetBundle.Unload(false);
				m_LoadedAssetBundles.Remove(assetBundleName);
	
				Log(LogType.Info, assetBundleName + " has been unloaded successfully");
			}
		}
	
		void Update()
		{
			// Collect all the finished WWWs.
			var keysToRemove = new List<string>();
			foreach (var keyValue in m_DownloadingWWWs)
			{
				WWW download = keyValue.Value.download;
	
				// If downloading fails.
				if (download.error != null && download.error.Length > 0)
				{
					m_DownloadingErrors.Add(keyValue.Key, string.Format("Failed downloading bundle {0} from {1}: {2}", keyValue.Key, download.url, download.error));
					keysToRemove.Add(keyValue.Key);
					continue;
				}
	
				// If downloading succeeds.
				if(download.isDone)
				{
					AssetBundle bundle = download.assetBundle;
					if (bundle == null)
					{
						m_DownloadingErrors.Add(keyValue.Key, string.Format("{0} is not a valid asset bundle.", keyValue.Key));
						keysToRemove.Add(keyValue.Key);
						continue;
					}
				
					Log(LogType.Info, "[ done download ] " + keyValue.Key + " is done at frame " + Time.frameCount);
					if(!VersionManager.local.IsExist(download.url)) {
						VersionManager.local.bundles.Add(new VersionManager.BundleInfo() {
							url = download.url,
							size = download.bytesDownloaded
						});
						VersionManager.UpdateVersionConfig(VersionManager.local);
					}
					keyValue.Value.m_AssetBundle = download.assetBundle;
					m_LoadedAssetBundles.Add(keyValue.Key, keyValue.Value );
					keysToRemove.Add(keyValue.Key);
				}
			}
	
			// Remove the finished WWWs.
			foreach( var key in keysToRemove)
			{
				WWW download = m_DownloadingWWWs[key].download;
				m_DownloadingWWWs.Remove(key);
				download.Dispose();
			}
	
			// Update all in progress operations
			for (int i=0;i<m_InProgressOperations.Count;)
			{
				if (!m_InProgressOperations[i].Update())
					m_InProgressOperations.RemoveAt(i);
				else
					i++;
			}
		}
	
		// Load asset from the given assetBundle.
		static public AssetBundleLoadAssetOperation LoadAssetAsync (string assetBundleName, string assetName, System.Type type)
		{
			Log(LogType.Info, "Load Asset (" + assetName + ") from " + assetBundleName);
	
			AssetBundleLoadAssetOperation operation = null;
	#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
				if (assetPaths.Length == 0)
				{
					Log(LogType.Error, "There is no asset with name \"" + assetName + "\" in " + assetBundleName);
					return null;
				}
	
				// @TODO: Now we only get the main object from the first asset. Should consider type also.
				Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
				operation = new AssetBundleLoadAssetOperationSimulation (target);
			}
			else
	#endif
			{
				assetBundleName = RemapVariantName(assetBundleName);
				_loadAssetBundle(new LoadAssetBundleInfo(assetBundleName));
				operation = new AssetBundleLoadAssetOperationFull (assetBundleName, assetName, type);
	
				m_InProgressOperations.Add (operation);
			}
	
			return operation;
		}
	
		// Load level from the given assetBundle.
		static public AssetBundleLoadOperation LoadLevelAsync (string assetBundleName, string levelName, bool isAdditive)
		{
			Log(LogType.Info, "Load level ('" + levelName + "') from " + assetBundleName);
	
			AssetBundleLoadOperation operation = null;
	#if UNITY_EDITOR
			if (SimulateAssetBundleInEditor)
			{
				operation = new AssetBundleLoadLevelSimulationOperation(assetBundleName, levelName, isAdditive);
			}
			else
	#endif
			{
				assetBundleName = RemapVariantName(assetBundleName);
				_loadAssetBundle(new LoadAssetBundleInfo(assetBundleName));
				operation = new AssetBundleLoadLevelOperation (assetBundleName, levelName, isAdditive);
	
				m_InProgressOperations.Add (operation);
			}
	
			return operation;
		}
	} // End of AssetBundleManager.
}