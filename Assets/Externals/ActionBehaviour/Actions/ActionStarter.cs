/* *************************************************
*  Created:  2018-06-02 19:46:32
*  File:     ActionStarter.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using UnityEngine;

namespace ActionBehaviour {

	// deprecated ( ActionStarter instead )
    public class ActionStarter : Execute {

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

	}

}
