/* *************************************************
*  Created:  2018-1-28 20:15:39
*  File:     AssignParentTransform.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {
	
    using NaughtyAttributes;

    public enum ManagerType
    {
        None = 0,
        UIViewManager,
    }

	public class AssignParentTransform : ActionNode {


        [SerializeField]
        protected ManagerType type;

        [SerializeField]
        protected Transform parent;

        [ReorderableList]
		[SerializeField]
		protected ActionNode[] targets;


        public override ActionState OnUpdate() {

			// parent update
			ActionState result = base.OnUpdate();
			if(result != ActionState.Success)
				return result;

            for (int i = 0; i < targets.Length; ++i) {
                if( type == ManagerType.None)
                    targets[i].transform.SetParent(this.parent);
                else if (type == ManagerType.UIViewManager)
                    targets[i].transform.SetParent(UIViewManager.instance.transform);
            }

			return ActionState.Success;
		}
	}

}