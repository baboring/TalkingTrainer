/* *************************************************
*  Created:  2018-1-28 19:46:32
*  File:     ActionController.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ActionBehaviour {



	public class ActionController : ActionNode {

		public enum StartOption {
			None = 0,	// nothing to start
			AutoStart,	// called on Start
		}

		[SerializeField]
		protected StartOption startType;

		[SerializeField]
		protected ActionNode Node;

		void Start() {
			
			if( StartOption.AutoStart == startType )
				base.Execute();
		}

        // called by outside
        public void RunController()
        {

            base.Execute();
        }

        protected override void OnReset()
        {
            base.OnReset();
            Debug.Assert(Node != null, "Node is null:" + label);
        }


		// On Update
        public override ActionState OnUpdate() {

			// parent update
			ActionState result = base.OnUpdate();
			if(result != ActionState.Success)
				return result;

            Debug.Assert(Node != null,"Node is null:" + label);
            Debug.Assert(Node != this,"Node is owner:" + label);
			if(Node == this || Node == null)
				return ActionState.Error;

            // run 
            Node.Execute();

			return ActionState.Success;
		}

		
	}

#if UNITY_EDITOR
    [CustomEditor(typeof(ActionController), true)]
    [CanEditMultipleObjects]
    public class NodeActionControllerEditor : Editor
    {
      public override void OnInspectorGUI()
      {
          DrawDefaultInspector();

            ActionController myScript = (ActionController)target;
            if(GUILayout.Button("Execute"))
            {
                myScript.Execute();
            }
      }
    }
#endif
}
