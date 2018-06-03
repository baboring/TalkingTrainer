/* *************************************************
*  Created:  2018-1-28 19:51:59
*  File:     ActionNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {

	using Common.Utilities;
    using NaughtyAttributes;

	public class ActionNode : BaseNode {

		public enum LogMode { All, JustErrors };
		static LogMode m_LogMode = LogMode.All;
	

		[SerializeField]
		public string comments = null;

        [Button("Execute(Click)")]
        private void _RunInEdtior() {
            Execute();
        }

        public override ActionState OnUpdate() {
            if(null != comments && comments.Length > 0)
                Log(LogType.Log, comments);
			return ActionState.Success;
		}

		private static void Log(LogType logType, string text)
		{
			if (logType == LogType.Error)
				Logger.LogError("[ActionNode] " + text);
			else if (m_LogMode == LogMode.All)
				Logger.Debug("[ActionNode] " + text);
		}
		
	}
}