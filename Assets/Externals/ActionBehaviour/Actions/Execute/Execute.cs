/* *************************************************
*  Created:  2018-1-28 19:46:32
*  File:     Execute.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using UnityEngine;

namespace ActionBehaviour {

	public class Execute : ActionNode {

		[SerializeField]
		protected ActionNode Node;

        protected override void OnReset()
        {
            base.OnReset();
            Debug.Assert(Node != null, "Node is null:");
        }

        public override ActionState OnUpdate() {

			// parent update
			ActionState result = base.OnUpdate();
			if(result != ActionState.Success)
				return result;
			
            Debug.Assert(Node != null, "Node is null:");
            Debug.Assert(Node != this, "Node is owner:");
            if (Node == this || Node == null)
                return ActionState.Error;
            
            state = Node.Execute();

            return state;
		}
		
	}
}
