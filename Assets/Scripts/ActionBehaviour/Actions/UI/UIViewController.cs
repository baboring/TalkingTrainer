/* *************************************************
*  Created:  2018-3-28 19:46:32
*  File:     UIViewController.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ActionBehaviour {



    public class UIViewController : Execute {

		public enum StartOption {
			None = 0,	// nothing to start
			AutoStart,	// called on Start
		}

		[SerializeField]
		protected StartOption startType;

		void Start() {
			
			if( StartOption.AutoStart == startType )
				base.Execute();
		}

        // called by outside
        public void RunController()
        {

            base.Execute();
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
