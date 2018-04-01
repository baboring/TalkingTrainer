/* *************************************************
*  Created:  2018-1-28 19:46:32
*  File:     Execute.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using UnityEngine;

namespace ActionBehaviour {

    using NaughtyAttributes;

	public class Execute : ActionNode {

		[SerializeField]
		protected ActionNode Node;

        protected override void OnReset()
        {
            base.OnReset();
            Debug.Assert(Node != null, "Node is null:" + label);
        }

        public override ActionState OnUpdate() {

			// parent update
			ActionState result = base.OnUpdate();
			if(result != ActionState.Success)
				return result;
			
            Debug.Assert(Node != null, "Node is null:" + label);
            Debug.Assert(Node != this, "Node is owner:" + label);
            if (Node == this || Node == null)
                return ActionState.Error;

			return Node.Execute();;
		}
		
	}
}
