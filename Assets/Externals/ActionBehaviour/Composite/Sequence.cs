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

    using NaughtyAttributes;

	public class Sequence : ActionNode {

        [ReorderableList]
		[SerializeField]
		protected ActionNode[] childNodes;

		private int m_index = 0;
        AsyncProcessing async = null;

		protected override void OnReset() {
            
            base.OnReset();

            if(null != async && null != async.backgroundWorking) {
                StopCoroutine(async.backgroundWorking);
                async = null;
            }

            m_index = 0;

            // reset children
            ResetChildren();
		}

        // reset children
        protected void ResetChildren() {

            if(childNodes != null && childNodes.Length > 0) {
                foreach (var node in childNodes)
                    if (node != null)
                        node.Reset();
            }
        }


        public override ActionState OnUpdate() {

            if(null == async || !async.isDone) {
                ActionState result = base.OnUpdate();
                if (result != ActionState.Success)
                    return result;
            }

            state = this.UpdateSequence();
            if(ActionState.Running == state && null == async) {
                async = new AsyncProcessing(StartCoroutine(CoUpdateSequence()));
            }

            return state;
		}

        // Coroutine for updating sequence multi
        IEnumerator CoUpdateSequence() {

            // performs to update Sequence
            while (m_index < childNodes.Length)
            {
                state = this.UpdateSequence();
                if (ActionState.Running != state)
                    break;
                yield return null;
            }
            async.isDone = true;
        }

        // inner update sequnce
        ActionState UpdateSequence() {
            
            ActionState result = ActionState.Success;
            while (m_index < childNodes.Length)
            {
                // exception infinite loop
                Debug.Assert(childNodes[m_index] != this, "child node is ownself!! " + this.name);
                if (childNodes[m_index] == this)
                    return ActionState.Error;

                result = childNodes[m_index].OnUpdate();
                if (ActionState.Success != result)
                    return result;
                ++m_index;
            }

            return result;
        }

	}


}