/* *************************************************
*  Created:  2018-1-28 19:46:46
*  File:     SequenceNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {

	public class SequenceNode : ActionNode {

		[SerializeField]
		protected ActionNode[] childNodes;

		private int index = 0;

		protected override void OnReady() {
			index = 0;
		}

		protected override ActionState OnUpdate() {

			// parent update
			ActionState result = base.OnUpdate();
			if(result != ActionState.Success)
				return result;

			while(index < childNodes.Length) {

				// exception infinite loop
				Debug.Assert(childNodes[index] != this,"child node is ownself!! " + this.name);
				if(childNodes[index] == this)
					return ActionState.Error;

				result = childNodes[index].Execute();
				if(ActionState.Success != result)
					return result;
				++index;
			}

			return ActionState.Success;
		}

	}

}