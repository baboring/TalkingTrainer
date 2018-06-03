/* *************************************************
*  Created:  2018-1-28 19:46:32
*  File:     ActionController.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using UnityEngine;

namespace ActionBehaviour {

    public class ActionController : Execute {

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

}
