/* *************************************************
*  Created:  2018-1-28 19:46:40
*  File:     BaseNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {

	public enum ActionState {
		None = 0,
		Ready,
		Success,
		Fail,
		Running,
		Error = -1,

	}

	// Base class
	public abstract class BaseNode : MonoBehaviour {

		// state
		protected ActionState state = ActionState.None;

		protected virtual void OnReady() {}
		// ready
		protected abstract ActionState OnUpdate();

		// default core function
		public virtual ActionState Execute() {

			// start up 
			if(ActionState.None == state) {
				state = ActionState.Ready;
				OnReady();
			}

			// update 
			if(ActionState.Running == state || ActionState.Ready == state) 
				state = OnUpdate();

			return state;
		}
	}

}