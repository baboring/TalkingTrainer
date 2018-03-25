
/* *************************************************
*  Created:  2018-1-28 20:15:39
*  File:     ObjectDeactiveNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {

	public class DeactiveBehaviourNode : ActionNode {

		[SerializeField]
		protected GameObject[] objects;

        public override ActionState OnUpdate() {

			// parent update
			ActionState result = base.OnUpdate();
			if(result != ActionState.Success)
				return result;

			for( int i=0;i < objects.Length; ++i )
				objects[i].SetActive(false);

			return ActionState.Success;
		}
		
		
	}

}