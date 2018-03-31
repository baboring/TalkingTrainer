/* *************************************************
*  Created:  2018-1-28 19:46:40
*  File:     BaseNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using UnityEngine;

namespace ActionBehaviour {

	public enum ActionState {
		None = 0,
		Success,
		Fail,
		Running,
		Error = -1,

	}

	// Base class
	public abstract class BaseNode : MonoBehaviour {

		// state
		protected ActionState state = ActionState.None;

        protected virtual void OnReset() { state = ActionState.None; }

        public void Reset() { OnReset(); }

		// ready
		public abstract ActionState OnUpdate();

		// default core function
		public virtual ActionState Execute() {

			// start up 
            OnReset();

			// update 
            if(ActionState.Running == state || ActionState.None == state) 
				state = OnUpdate();

			return state;
		}
	}
}