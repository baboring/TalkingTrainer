/* *************************************************
*  Created:  2018-4-01 19:46:32
*  File:     ViewController.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ActionBehaviour {

    using Common.Utilities;

    public class UIViewManager : MonoSingleton<UIViewManager> {

		protected override void Awake()
		{
            base.Awake();
		}

		public void Execute() {
            
        }

		
	}

#if UNITY_EDITOR
    [CustomEditor(typeof(UIViewManager), true)]
    [CanEditMultipleObjects]
    public class UIViewManagerEditor : Editor
    {
      public override void OnInspectorGUI()
      {
            DrawDefaultInspector();

            UIViewManager myScript = (UIViewManager)target;
            if(GUILayout.Button("Execute"))
            {
                myScript.Execute();
            }
      }
    }
#endif
}
