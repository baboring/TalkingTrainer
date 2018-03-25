
/* *************************************************
*  Created:  2018-1-28 20:15:39
*  File:     WaitNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {

	public class WaitNode : ActionNode {

		[SerializeField]
		protected float delay;
		private float elapsedTime;

		protected override void OnReset() {
            base.OnReset();
			elapsedTime = 0;
		}
        public override ActionState OnUpdate() {

			// parent update
			ActionState result = base.OnUpdate();
			if(result != ActionState.Success)
				return result;
			
			if(elapsedTime >= delay)
				return ActionState.Success;

			elapsedTime += Time.deltaTime;

			return ActionState.Running;
		}
	}

}