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

		protected override void OnStart() {
			index = 0;
		}

		public override ActionState Run() {
			// run parent
			ActionState result = base.Run();

			// default result
			if(result != ActionState.Success)
				return result;

			while(index < childNodes.Length) {
				result = childNodes[index].Run();
				if(ActionState.Success != result)
					return result;
				++index;
			}

			return ActionState.Success;
		}

	}

}