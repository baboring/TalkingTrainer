
/* *************************************************
*  Created:  2018-3-28 20:15:39
*  File:     DestroyGameObject.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {

    using NaughtyAttributes;

	public class DestroyGameObject : ActionNode {

        [ReorderableList]
		[SerializeField]
		protected GameObject[] objects;

        public override ActionState OnUpdate() {

			// parent update
			ActionState result = base.OnUpdate();
			if(result != ActionState.Success)
				return result;

            for (int i = 0; i < objects.Length; ++i)
                GameObject.Destroy(objects[i]);
            return ActionState.Success;
		}
	}

}