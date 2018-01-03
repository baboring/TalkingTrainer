using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TTrainer {

	using AssetBundles;
	public class ContentStudyBoardController : ContentControllerBase {

		[SerializeField]
		private ConfigSceneData config;

		[SerializeField]
		protected ScrollRect objListup;
		[SerializeField]
		protected ScrollRect objPlay;

		[SerializeField]
		protected RectTransform objContentViewListup;
		[SerializeField]
		protected RectTransform objContentViewSnap;
		
		[SerializeField]
		protected UILessionDisplay prefabDisplayItem;	// prefab
		[SerializeField]
		protected UIStudyBoard prefabStudyBoard;	// prefab

		private UILessionDisplay currentUnit = null;
		private List<InfoLessonData> _lstUnits = null;

		ControllerEntity _controllerEntity = new ControllerEntity();


		// Use this for initialization
		IEnumerator Start () {
			
			prefabDisplayItem.gameObject.SetActive(false);
			prefabStudyBoard.gameObject.SetActive(false);

			yield return StartCoroutine(ResourceManager.instance.LoadAssetBundle(new AssetInfo(config.tableBundleName, config.tableAssetName,(bundleLoaded)=>{
				_lstUnits = R.CsvUtil.LoadObjects<InfoLessonData>(bundleLoaded.GetAsset<TextAsset>());
				Debug.Log("Load asset " + config.tableAssetName);
			})));

			_controllerEntity.controller = this;
			_controllerEntity.Event = eEntityState.Contents;

			// list up items
			ListupContents();
		}

		void ListupContents() {
			prefabDisplayItem.gameObject.SetActive(false);
			objContentViewListup.transform.Clear();
			foreach(var info in this._lstUnits) {
				var go = GameObject.Instantiate(prefabDisplayItem.gameObject,objContentViewListup);
				go.SetActive(true);
				var controller = go.GetComponent<UILessionDisplay>();
				controller.Prime(config.tableBundleName,info);
			}
		}
		void ListupStudyBoards(InfoLessonData lesson) {
			prefabStudyBoard.gameObject.SetActive(false);
			objContentViewSnap.transform.Clear();
			foreach(var info in lesson.lstStudyBoard) {
				var go = GameObject.Instantiate(prefabStudyBoard.gameObject,objContentViewSnap);
				go.SetActive(true);
				var controller = go.GetComponent<UIStudyBoard>();
				controller.Prime(config.tableBundleName,info);
			}
		}
		
		// Update is called once per frame
		void Update () {
			_controllerEntity.UpdateState();
		}
		void OnEnable() {
			SceneManager.sceneLoaded += OnSceneLoaded;	
		}
		void OnDisable() {
			SceneManager.sceneLoaded -= OnSceneLoaded;	
		}
		void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			Debug.Log("OnSceneLoaded: " + scene.name);
			Debug.Log(mode);
		}		

		public void UpdateDisplay(eEntityState eState) {
			objListup.gameObject.SetActive(eState == eEntityState.Contents);
			objPlay.gameObject.SetActive(eState != eEntityState.Contents);
			
		}

		public void OnClickLession(UILessionDisplay uiInfo) {
			Debug.Log("Click " + uiInfo.info.TitleName);

			currentUnit = uiInfo;
			ListupStudyBoards(currentUnit.info);
			_controllerEntity.Event = eEntityState.Lesson;
		}

		public void OnClickExit() {
			switch((eEntityState)_controllerEntity.Event){
				case eEntityState.Contents:
					TTrainer.Main.instance.UnloadScene();
					break;
				case eEntityState.Lesson:
					ListupContents();
					_controllerEntity.Event = eEntityState.Contents;
					break;
			}
		}
		

		/// -------------------------------------
		public enum eEntityState {
			None = 0,
			Contents,
			Lesson,
		}

		class ControllerEntity : Entity {
			public ContentStudyBoardController controller;
			class FSM : StateTransitionTable {}
			public ControllerEntity() {
				transitionTable = new FSM();
				transitionTable.SetState(eEntityState.Contents,new ContentsState());
				transitionTable.SetState(eEntityState.Lesson,new LessonState());
			}

		}

		// Wander around
		class ContentsState : IState {
			public void Enter(Entity e) {
				ControllerEntity entity = (ControllerEntity)e;
				entity.controller.UpdateDisplay((eEntityState)e.Event);
            }
            public void Exit(Entity e){}
			public void Execute(Entity e){}
		}

		class LessonState : IState {
			public void Enter(Entity e) {
				ControllerEntity entity = (ControllerEntity)e;
				entity.controller.UpdateDisplay((eEntityState)e.Event);
            }
            public void Exit(Entity e){}
			public void Execute(Entity e){}
		}		
		
	}

}