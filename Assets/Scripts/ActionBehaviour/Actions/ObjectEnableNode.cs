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
	public class ObjectEnableNode : ActionNode {

		[SerializeField]
		protected ActionNode[] targets;
		protected override void OnStart() {
			for( int i=0;i < targets.Length; ++i )
				targets[i].enabled = true;

		}
	}

}