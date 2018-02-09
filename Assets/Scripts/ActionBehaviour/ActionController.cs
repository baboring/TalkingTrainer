/* *************************************************
*  Created:  2018-1-28 19:46:32
*  File:     ActionController.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {



	public class ActionController : ActionNode {

		public enum StartOption {
			None = 0,	// nothing to start
			AutoStart,	// called on Start
		}

		[SerializeField]
		protected StartOption startType;

		[SerializeField]
		protected ActionNode Node;

		void Start() {
			
			if( StartOption.AutoStart == startType )
				base.Execute();
		}

		// Use this for initialization
		IEnumerator ExecuteNode() {

			while(Node != null && Node.Execute() == ActionState.Running)
				yield return null;
			yield return null;
		}

		// On Update
		protected override ActionState OnUpdate() {

			// parent update
			ActionState result = base.OnUpdate();
			if(result != ActionState.Success)
				return result;

			Debug.Assert(Node != null,"Node is null");
			Debug.Assert(Node != this,"Node is owner");
			if(Node == this || Node == null)
				return ActionState.Error;
			
			// run 
			StartCoroutine(ExecuteNode());

			return ActionState.Success;
		}

		// called by outside
		public void RunController() {

			base.Execute();
		}
		
	}
}
