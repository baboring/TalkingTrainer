
/* *************************************************
*  Created:  2018-1-28 20:15:39
*  File:     LogNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {

    using Common.Utilities;

	public class Comment : ActionNode {

        [SerializeField]
        protected LogLevel type;
		[SerializeField]
		protected string logText;


        public override ActionState OnUpdate() {

			// parent update
			ActionState result = base.OnUpdate();
			if(result != ActionState.Success)
				return result;
            if(logText.Length > 0)
                Logger.Log(type, logText);
			return ActionState.Success;

		}
	}

}