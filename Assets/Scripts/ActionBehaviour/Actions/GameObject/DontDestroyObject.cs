
/* *************************************************
*  Created:  2018-3-31 20:15:39
*  File:     DontDestroyObject.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {

    using NaughtyAttributes;

	public class DontDestroyObject : ActionNode {

        [ReorderableList]
		[SerializeField]
		protected GameObject[] objects;

        public override ActionState OnUpdate() {

			// parent update
			ActionState result = base.OnUpdate();
			if(result != ActionState.Success)
				return result;
			
			for( int i=0;i < objects.Length; ++i )
                DontDestroyOnLoad(objects[i]);
            
			return ActionState.Success;
		}
	}

}