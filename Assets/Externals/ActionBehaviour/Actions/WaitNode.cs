
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
		protected float second;

        Coroutine m_working = null;

		protected override void OnReset() {
            base.OnReset();
            if (null != m_working)
                StopCoroutine(CoUpdateSequence());
            m_working = null;
		}

        // coroutine for updating
        IEnumerator CoUpdateSequence()
        {
            yield return new WaitForSeconds(second);

            state = ActionState.Success;
            m_working = null;
        }

        // Update node
        public override ActionState OnUpdate() {

			// parent update
            if(null == m_working && state != ActionState.Success) {
                ActionState result = base.OnUpdate();
                if (result != ActionState.Success)
                    return result;

                state = ActionState.Running;
                m_working = StartCoroutine(CoUpdateSequence());
            }


            return state;
		}
	}

}