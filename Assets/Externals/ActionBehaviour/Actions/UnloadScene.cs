/* *************************************************
*  File:     UnloadSceneNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ActionBehaviour {

	using Common.Utilities;
    using NaughtyAttributes;

    public class UnloadScene : ActionNode {

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
        protected LOAD_SCENE_METHOD Method;   // Sync, Async

        AsyncOperation operation;

        // Action Script
        public override ActionState OnUpdate() {

			// parent update
			ActionState result = base.OnUpdate();
			if(result != ActionState.Success)
				return result;

            if(state == ActionState.None) {
                // Load scene index
                if (isIndex)
                    operation = SceneManager.UnloadSceneAsync(LeveIndex);
                // Load scene name
                else
                    operation = SceneManager.UnloadSceneAsync(LevelName);

                // set state
                if (Method == LOAD_SCENE_METHOD.Async) {
                    state = ActionState.Running;
                    operation.completed += (va) => {
                        state = ActionState.Success;
                    };
                }
                else
                    state = ActionState.Success;
            }


            return state;

		}

	}

}