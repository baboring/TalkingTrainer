/* *************************************************
*  Created:  2018-1-28 19:46:46
*  File:     RepeatNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {
    
    using NaughtyAttributes;

	public class Repeat : ActionNode {

        [SerializeField]
        protected int loop = 1;
        
        [ReorderableList]
		[SerializeField]
		protected ActionNode[] childNodes;

		private int m_index = 0;
		private int m_count = 0;

        AsyncProcessing async = null;

        // Reset Action
		protected override void OnReset() {
            
            base.OnReset();

            if(null != async && null != async.backgroundWorking) {
                StopCoroutine(async.backgroundWorking);
                async = null;
            }

            m_index = 0;
            m_count = 0;

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

        // Run Action
        public override ActionState OnUpdate() {

            if(null == async || !async.isDone) {
                ActionState result = base.OnUpdate();
                if (result != ActionState.Success)
                    return result;
            }

            // no more tasks
            if(loop < 1)
                return ActionState.Success;

            state = this.UpdateSequence();
            if(ActionState.Running == state && null == async) {
                async = new AsyncProcessing(StartCoroutine(CoUpdateSequence()));
            }

            return state;
		}

        // coroutine for updating
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

        // inner update sequence
        ActionState UpdateSequence() {
            
            ActionState result = ActionState.Success;

            while(m_count < loop) {
                // processing sequence step by step
                while (m_index < childNodes.Length) {
                    // exception infinite loop
                    Debug.Assert(childNodes[m_index] != this, "child node is ownself!! " + this.name);
                    if (childNodes[m_index] == this)
                        return ActionState.Error;

                    result = childNodes[m_index].OnUpdate();
                    if (ActionState.Success != result)
                        return result;
                    ++m_index;
                }

                // reset children
                ResetChildren();

                // check loop count
                m_count++;
                m_index = 0;
            }

            // success
            state = result;

            return state;
        }

	}


}