
/* *************************************************
*  Created:  2018-1-28 20:15:39
*  File:     LogNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {

	public class LogNode : ActionNode {

		[SerializeField]
		protected string logString;

        public override ActionState OnUpdate() {

			// parent update
			ActionState result = base.OnUpdate();
			if(result != ActionState.Success)
				return result;
			if(logString.Length > 0)
				Debug.Log(logString);
			return ActionState.Success;

		}
	}

}