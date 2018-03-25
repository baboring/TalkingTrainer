﻿/* *************************************************
*  Created:  2018-1-28 19:46:40
*  File:     BaseNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ActionBehaviour {

	public enum ActionState {
		None = 0,
		Success,
		Fail,
		Running,
		Error = -1,

	}

	// Base class
	public abstract class BaseNode : MonoBehaviour {

		// state
		protected ActionState state = ActionState.None;

        protected virtual void OnReset() { state = ActionState.None; }

		// ready
		public abstract ActionState OnUpdate();

		// default core function
		public virtual ActionState Execute() {

			// start up 
            OnReset();

			// update 
            if(ActionState.Running == state || ActionState.None == state) 
				state = OnUpdate();

			return state;
		}
	}

	#if UNITY_EDITOR
	[CustomEditor(typeof(BaseNode), true)]
	[CanEditMultipleObjects]
	public class BaseNodeEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			
			BaseNode myScript = (BaseNode)target;
			if(GUILayout.Button("Execute"))
			{
				myScript.Execute();
			}
		}
	}
	#endif	

}