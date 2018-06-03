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

        [BoxGroup("Setting")]
        [Dropdown("Set")]
        [HideIf("isIndex")]
        [SerializeField]
        protected string LevelName; // Scene Name

        [SerializeField]
        protected StringSet Set;


        [BoxGroup("Setting")]
        [ShowIf("isIndex")]
		[SerializeField]
		protected int LeveIndex;		// Scene Build Index

        [BoxGroup("Setting")]
		[SerializeField]
		protected LOAD_SCENE_METHOD loadMethod;	// Sync, Async

        [BoxGroup("Setting")]
		[SerializeField]
		protected LoadSceneMode loadMode;	// Addictive, Single

        [BoxGroup("Actions")]
        [SerializeField]
        protected ActionNode NodeOnStartLoad;

        [BoxGroup("Actions")]
        [SerializeField]
        protected ActionNode NodeOnSceneLoaded;


        AsyncOperation operation;

        // Action Script
        public override ActionState OnUpdate() {

			// parent update
			ActionState result = base.OnUpdate();
			if(result != ActionState.Success)
				return result;

            //SceneManager.sceneLoaded += OnSceneLoaded;

            // Run Before load Scene
            if (null != NodeOnStartLoad)
                NodeOnStartLoad.Execute();

			// Load scene index
            if(isIndex){
                if(loadMethod == LOAD_SCENE_METHOD.Async) {
                    state = ActionState.Running;
                    operation = SceneManager.LoadSceneAsync(LeveIndex, loadMode);
                    operation.completed += (var) =>
                    {
                        state = ActionState.Success;
                        if (null != NodeOnSceneLoaded)
                            NodeOnSceneLoaded.Execute();
                    };
                }
                else {
                    SceneManager.LoadScene(LeveIndex, loadMode);
                    state = ActionState.Success;
                }
			}
			// Load scene name
			else {
                if(loadMethod == LOAD_SCENE_METHOD.Async) {
                    state = ActionState.Running;
                    operation = SceneManager.LoadSceneAsync(LevelName, loadMode);
                    operation.completed += (var) =>
                    {
                        state = ActionState.Success;
                        if (null != NodeOnSceneLoaded)
                            NodeOnSceneLoaded.Execute();
                    };
                }
                else {
                    SceneManager.LoadScene(LevelName, loadMode);
                    state = ActionState.Success;
                }
			}

            return state;

		}

	}

}