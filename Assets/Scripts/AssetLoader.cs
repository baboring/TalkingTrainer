using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using AssetBundles;

namespace TTrainer {


	public class AssetLoader : SingletonMono<AssetLoader> {

		public const string AssetBundlesOutputPath = "/AssetBundles/";

		public string sceneAssetBundle;
		public string sceneName;

		
		public string assetBundleName;
		public string assetName;

		// Use this for initialization

		bool isInitilizaed = false;
		IEnumerator Start ()
		{
			Debug.Log("start LoadAssets");
			yield return StartCoroutine(Initialize());
			
		}

		// Initialize the downloading url and AssetBundleManifest object.
		protected IEnumerator Initialize()
		{
			// Don't destroy this gameObject as we depend on it to run the loading script.
			DontDestroyOnLoad(gameObject);

			// With this code, when in-editor or using a development builds: Always use the AssetBundle Server
			// (This is very dependent on the production workflow of the project. 
			// 	Another approach would be to make this configurable in the standalone player.)
			#if DEVELOPMENT_BUILD || UNITY_EDITOR
			AssetBundleManager.SetDevelopmentAssetBundleServer ();
			#else
			// Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
			AssetBundleManager.SetSourceAssetBundleURL(Application.dataPath + "/");
			// Or customize the URL based on your deployment or configuration
			//AssetBundleManager.SetSourceAssetBundleURL("http://www.MyWebsite/MyAssetBundles");
			#endif

			// Initialize AssetBundleManifest which loads the AssetBundleManifest object.
			var request = AssetBundleManager.Initialize();
			if (request != null)
				yield return StartCoroutine(request);

			isInitilizaed = true;
		}

		public IEnumerator InitializeLevelAsync (string assetBundleName,string levelName, bool isAdditive)
		{
			// This is simply to get the elapsed time for this phase of AssetLoading.
			float startTime = Time.realtimeSinceStartup;

			// Load level from assetBundle.
			AssetBundleLoadOperation request = AssetBundleManager.LoadLevelAsync(assetBundleName, levelName, isAdditive);
			if (request == null)
				yield break;

			yield return StartCoroutine(request);

			// Calculate and display the elapsed time.
			float elapsedTime = Time.realtimeSinceStartup - startTime;
			Debug.Log("Finished loading scene " + levelName + " in " + elapsedTime + " seconds" );
		}

		protected IEnumerator InstantiateGameObjectAsync (string assetBundleName, string assetName)
		{
			// This is simply to get the elapsed time for this phase of AssetLoading.
			float startTime = Time.realtimeSinceStartup;

			// Load asset from assetBundle.
			AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject) );
			if (request == null)
				yield break;
			yield return StartCoroutine(request);

			// Get the asset.
			GameObject prefab = request.GetAsset<GameObject> ();

			if (prefab != null)
				GameObject.Instantiate(prefab);
			
			// Calculate and display the elapsed time.
			float elapsedTime = Time.realtimeSinceStartup - startTime;
			Debug.Log(assetName + (prefab == null ? " was not" : " was")+ " loaded successfully in " + elapsedTime + " seconds" );
		}

		bool isLoading = false;
		public void LoadTest() {
			if(!isLoading)
				StartCoroutine(LoadSequence());
		}

		public void UnloadTest() {
			SceneManager.UnloadSceneAsync( sceneName );	
		}

		IEnumerator LoadSequence() {
			isLoading = true;
			if(!isInitilizaed)
				yield return null;
			// Load variant level which depends on variants.
			yield return StartCoroutine(InitializeLevelAsync (sceneAssetBundle,sceneName, true) );
			
			// Load asset.
			yield return StartCoroutine(InstantiateGameObjectAsync (assetBundleName, assetName) );
			isLoading = false;
		}
	}
}