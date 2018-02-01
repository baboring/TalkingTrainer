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
	public class ActionController : BaseNode {

		[SerializeField]
		protected ActionNode Node;

		void OnEnable() {
			state = ActionState.None;
			Run();
		}

		// Use this for initialization
		IEnumerator Execute() {
			while(Node != null && Node.Run() != ActionState.Success)
				yield return null;
			yield return null;
		}

		protected override void OnStart() {
			StartCoroutine(Execute());
		}

		public override ActionState Run() {
			if(ActionState.None != state)
				return state;
			return base.Run();
		}
		
	}
}
