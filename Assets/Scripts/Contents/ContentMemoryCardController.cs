using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TTrainer {

	using AssetBundles;
	public class ContentMemoryCardController : ContentControllerBase {

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
		protected UILessionDisplay objDisplayItem;	// prefab

		private UILessionDisplay currentUnit = null;

		ControllerEntity _controllerEntity = new ControllerEntity();


		// Use this for initialization
		IEnumerator Start () {
			
			objDisplayItem.gameObject.SetActive(false);
			yield return null;

			AssetInfo assetInfo = new AssetInfo(config.tableBundleName, config.tableAssetName);
			// Resources.instance.LoadAsyncLessionsTable(assetInfo);
			ResourceManager.instance.LoadAsyncLessionList(assetInfo);

			if(!assetInfo.isLoaded)
				yield return null;

			_controllerEntity.controller = this;
			_controllerEntity.Event = eEntityState.Listup;

			objDisplayItem.gameObject.SetActive(false);

			// list up items
			foreach(var info in ResourceManager.instance._lessionList) {
				var go = GameObject.Instantiate(objDisplayItem.gameObject,objContentViewListup);
				go.SetActive(true);
				var controller = go.GetComponent<UILessionDisplay>();
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
			objListup.gameObject.SetActive(eState == eEntityState.Listup);
			objPlay.gameObject.SetActive(eState != eEntityState.Listup);
			
		}

		public void OnClickLession(UILessionDisplay uiInfo) {
			Debug.Log("Click " + uiInfo.info.TitleName);

			currentUnit = uiInfo;
			_controllerEntity.Event = eEntityState.Play;
		}

		/// -------------------------------------
		public enum eEntityState {
			None = 0,
			Listup,
			Play,
		}

		class ControllerEntity : Entity {
			public ContentMemoryCardController controller;
			class FSM : StateTransitionTable {}
			public ControllerEntity() {
				transitionTable = new FSM();
				transitionTable.SetState(eEntityState.Listup,new ListupState());
				transitionTable.SetState(eEntityState.Play,new PlayState());
			}

		}

		// Wander around
		class ListupState : IState {
			public void Enter(Entity e) {
				ControllerEntity entity = (ControllerEntity)e;
				entity.controller.UpdateDisplay((eEntityState)e.Event);
            }
            public void Exit(Entity e){}
			public void Execute(Entity e){}
		}

		class PlayState : IState {
			public void Enter(Entity e) {
				ControllerEntity entity = (ControllerEntity)e;
				entity.controller.UpdateDisplay((eEntityState)e.Event);
            }
            public void Exit(Entity e){}
			public void Execute(Entity e){}
		}		
		
	}

}