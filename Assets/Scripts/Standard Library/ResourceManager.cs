using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
//Makes a namespace for other scripts to use this code
namespace TTrainer {

	using AssetBundles;

	public class ResourceManager : SingletonMono<ResourceManager>{

		//makes a list _singleWords and a dictionary _SentencesTypes
		public List<InfoLessonData> _lstLesson;

		void Start() {

		}
		bool LoadAll(string path) {

			// load & make a table with GreetingWords csv
			//CsvUtil class
			Debug.Log("Load csv - ContantsTable");
			_lstLesson = R.CsvUtil.LoadObjects<InfoLessonData>(path+"ContentsTable.csv");

			Debug.Log("Load csv [complete]");

			return true;
		}

		public void LoadAsyncLessionList(AssetInfo info) {

			StartCoroutine(LoadAssetBundle(info,(request)=>{
				_lstLesson = R.CsvUtil.LoadObjects<InfoLessonData>(request.GetAsset<TextAsset>(),false);
				info.isLoaded = true;
				Debug.Log("Load csv - ContantsTable");
			}));
		}

		public IEnumerator LoadAssetBundle(AssetInfo info,System.Action<AssetBundleLoadAssetOperation> callback = null) {
			AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(info.bundleName, info.assetName, typeof(TextAsset) );
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
		public System.Action<AssetBundleLoadAssetOperation> calldone = null;

		public AssetInfo(string bundleName, string assetName, System.Action<AssetBundleLoadAssetOperation> calldone = null) {
			this.bundleName = bundleName;
			this.assetName = assetName;
			this.calldone = calldone;
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
		
	}
}
