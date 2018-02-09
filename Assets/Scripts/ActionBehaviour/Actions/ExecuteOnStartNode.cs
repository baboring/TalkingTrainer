/* *************************************************
*  Created:  2018-1-28 19:46:32
*  File:     ExecuteOnStartNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {

	public class ExecuteOnStartNode : ActionNode {

		[SerializeField]
		protected ActionNode Node;

		void Start() {
			Execute();
		}

		protected override ActionState OnUpdate() {

			// parent update
			ActionState result = base.OnUpdate();
			if(result != ActionState.Success)
				return result;

			if(Node == null || Node == this)
				return ActionState.Error;

			return Node.Execute();;
		}
		
	}
}
