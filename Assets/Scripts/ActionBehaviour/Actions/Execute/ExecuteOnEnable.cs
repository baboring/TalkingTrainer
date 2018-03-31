/* *************************************************
*  Created:  2018-1-28 19:46:32
*  File:     ExecuteOnEnable.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {

	public class ExecuteOnEnable : ActionNode {
		void OnEnable() {
			Execute();
		}		
	}
}
