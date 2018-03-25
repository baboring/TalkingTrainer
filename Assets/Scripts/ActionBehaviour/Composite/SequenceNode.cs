/* *************************************************
*  Created:  2018-1-28 19:46:46
*  File:     SequenceNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {

	public class SequenceNode : ActionNode {

		[SerializeField]
		protected ActionNode[] childNodes;

		private int index = 0;
        Coroutine working = null;

		protected override void OnReset() {
            
            base.OnReset();

            if(null != working)
                StopCoroutine(CoUpdateSequence());
            
            working = null;
            index = 0;
		}

        public override ActionState OnUpdate() {

            ActionState result = base.OnUpdate();
            if (result != ActionState.Success)
                return result;

            state = this.UpdateSequence();
            if(ActionState.Running == state) {
                working = StartCoroutine(CoUpdateSequence());
            }

            return state;
		}

        IEnumerator CoUpdateSequence()
        {

            // performs to update Sequence
            while (index < childNodes.Length)
            {
                state = this.UpdateSequence();
                if (ActionState.Running != state)
                    break;
                yield return null;
            }
            working = null;
        }

        ActionState UpdateSequence() {
            
            ActionState result = ActionState.Success;
            while (index < childNodes.Length)
            {
                // exception infinite loop
                Debug.Assert(childNodes[index] != this, "child node is ownself!! " + this.name);
                if (childNodes[index] == this)
                    return ActionState.Error;

                result = childNodes[index].OnUpdate();
                if (ActionState.Success != result)
                    return result;
                ++index;
            }

            return result;
        }

	}


}