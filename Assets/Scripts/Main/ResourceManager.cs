using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine.SceneManagement;
using UnityEngine;

//Makes a namespace for other scripts to use this code
namespace TTrainer {

	using Common.Utilities;
	using Common.ActionScript;

	using AssetBundles;

	public class ResourceManager : MonoSingleton<ResourceManager>{

		[SerializeField]
		protected ActionObject actObj;	// 

		//makes a list _singleWords and a dictionary _SentencesTypes
		public List<InfoLessonData> _lstLesson;

		bool isInitilizaed = false;
		// Use this for initialization
		IEnumerator Start () {
			Logger.Debug("start LoadAssets");
			yield return StartCoroutine(Initialize());
			
		}

		// Initialize the downloading url and AssetBundleManifest object.
		protected IEnumerator Initialize() {
			// Don't destroy this gameObject as we depend on it to run the loading script.
            if(transform.parent == null)
    			DontDestroyOnLoad(gameObject);

			// With this code, when in-editor or using a development builds: Always use the AssetBundle Server
			// (This is very dependent on the production workflow of the project. 
			// 	Another approach would be to make this configurable in the standalone player.)
			#if DEVELOPMENT_BUILD || UNITY_EDITOR
			AssetBundleManager.SetDevelopmentAssetBundleServer ();
			#else
			// Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
			//AssetBundleManager.SetSourceAssetBundleURL(Application.dataPath + "/");
			// Or customize the URL based on your deployment or configuration
			TextAsset urlFile = Resources.Load("AssetBundleServerURL") as TextAsset;
			string url = (urlFile != null) ? urlFile.text.Trim() : "http://www.MyWebsite/MyAssetBundles";
			AssetBundleManager.SetSourceAssetBundleURL(url);
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
			Logger.Debug("[ Finished ] loading scene " + levelName + " in " + elapsedTime + " seconds" );
		}

		public static void UnloadScene(string sceneLoaded) {
			if(sceneLoaded.Length > 0)
				SceneManager.UnloadSceneAsync( sceneLoaded );	
		}



		bool LoadAll(string path) {

			// load & make a table with GreetingWords csv
			//CsvUtil class
			Debug.Log("Load csv - ContantsTable");
			_lstLesson = CsvUtil.LoadObjects<InfoLessonData>(path+"ContentsTable.csv");

			Debug.Log("Load csv [complete]");

			return true;
		}

		public void LoadAsyncLessionList(AssetInfo info) {

			StartCoroutine(LoadAssetBundle(info,(request)=>{
				_lstLesson = CsvUtil.LoadObjects<InfoLessonData>(request.GetAsset<TextAsset>(),false);
				info.isLoaded = true;
				Debug.Log("Load csv - ContantsTable");
			}));
		}

		public IEnumerator LoadAssetBundle(AssetInfo info,System.Action<AssetBundleLoadAssetOperation> callback = null) {
			AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(info.bundleName, info.assetName, info.assetType );
			if (request == null)
				yield break;
			yield return StartCoroutine(request);

			if(info.calldone != null)
				info.calldone(request);
			if(callback != null)
				callback(request);
		}

	}

	public class AssetInfo {
		public string bundleName;
		public string assetName;
		public bool isLoaded = false;

		public System.Type assetType;
		public System.Action<AssetBundleLoadAssetOperation> calldone = null;

		public AssetInfo(string bundleName, string assetName, System.Type assetType,System.Action<AssetBundleLoadAssetOperation> calldone = null) {
			this.bundleName = bundleName;
			this.assetName = assetName;
			this.calldone = calldone;
			this.assetType = assetType;
			isLoaded = false;
		}
	}


	public class InfoLessonData {

		public int index;
		public bool Enable;
		public string TitleName;
		public string SheetName;
		public bool IsAllowedShuffle;
		public float Speed;
		public bool UpsideDown;
		public int Repeat;
		public bool CustomSetting;

		public InfoStudyBoardData[]  lstStudyBoard {
			get;
			set;
		}
	}


	public class InfoStudyBoardData {
		public int index;
		public string Category;
		public string Path;
		public int Unit;
		public int Lesson;
		public string Sentence;
		public string Translate;
		public string Notes;
		public string Question_File;
		public float Question_During;
		public string Answer_File;
		public float Answer_During;
		public string AssetBundle;
		
	}
}
