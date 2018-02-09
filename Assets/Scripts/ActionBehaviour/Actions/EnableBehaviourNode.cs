/* *************************************************
*  Created:  2018-1-28 20:15:39
*  File:     ObjectEnableNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {
	public class EnableBehaviourNode : ActionNode {

		[SerializeField]
		protected ActionNode[] targets;

		protected override ActionState OnUpdate() {

			// parent update
			ActionState result = base.OnUpdate();
			if(result != ActionState.Success)
				return result;

			for( int i=0;i < targets.Length; ++i )
				targets[i].enabled = true;

			return ActionState.Success;
		}
	}

}