

/* *************************************************
*  File:     LoadSceneNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;

namespace ActionBehaviour {

	using Common.Utilities;
    using NaughtyAttributes;

	public enum LOAD_SCENE_METHOD {
		Sync = 0,
		Async = 1
	}

    public class LoadScene : ActionNode {

        [SerializeField]
        protected bool isIndex	= true;

        [HideIf("isIndex")]
		[SerializeField]
		protected string LevelName;		// Scene Name

        [ShowIf("isIndex")]
		[SerializeField]
		protected int LeveIndex;		// Scene Build Index

		[SerializeField]
		protected LOAD_SCENE_METHOD loadMethod;	// Sync, Async

		[SerializeField]
		protected LoadSceneMode loadMode;	// Addictive, Single

        [SerializeField]
        protected ActionNode NodeOnStartLoad;
        [SerializeField]
        protected ActionNode NodeOnSceneLoaded;

        // Action Script
        public override ActionState OnUpdate() {

			// parent update
			ActionState result = base.OnUpdate();
			if(result != ActionState.Success)
				return result;

            Debug.Log("LoadScene :" + loadMethod);
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Run Before load Scene
            if (null != NodeOnStartLoad)
                NodeOnStartLoad.Execute();

			// Load scene index
            if(isIndex){
				if(loadMethod == LOAD_SCENE_METHOD.Async)
                    SceneManager.LoadSceneAsync(LeveIndex, loadMode);
				else
                    SceneManager.LoadScene(LeveIndex, loadMode);
			}
			// Load scene name
			else {
				if(loadMethod == LOAD_SCENE_METHOD.Async)
                    SceneManager.LoadSceneAsync(LevelName, loadMode);
				else
                    SceneManager.LoadScene(LevelName, loadMode);
			}

			return ActionState.Success;

		}

		// called second
		void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
            SceneManager.sceneLoaded -= OnSceneLoaded;

            Logger.DebugFormat("OnSceneLoaded: {0},Mode = {1}",scene.name,mode.ToString());
            if (null != NodeOnSceneLoaded)
                NodeOnSceneLoaded.Execute();
		}

		// called first
		//void OnEnable()
		//{
		//	Debug.Log("OnEnable called");
		//	SceneManager.sceneLoaded += OnSceneLoaded;
		//}
		//// called when the game is terminated
		//void OnDisable()
		//{
		//	Logger.Debug("OnDisable");
		//	SceneManager.sceneLoaded -= OnSceneLoaded;
		//}		

	}

}