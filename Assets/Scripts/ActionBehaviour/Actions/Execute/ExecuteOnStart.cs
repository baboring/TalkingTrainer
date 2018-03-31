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

		void Start() {
			Execute();
		}		
	}
}
