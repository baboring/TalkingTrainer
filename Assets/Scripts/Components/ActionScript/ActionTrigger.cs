using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Common.ActionScript {
	using System;

	public abstract class ActionTrigger : MonoBehaviour {

		[SerializeField]
		protected ActionObject actObj;

		// condition
		protected Predicate<Variable> match = null;
		protected Action<ActionObject> action = null;

		public string Name;
		// Use this for initialization
		public void Initialize () {
			if(null != actObj)
				actObj.Initialize(this.gameObject);
		}
		
		// Update is called once per frame
		public void Tick () {
			if( null == match || null == action)
				return;
			if(match(actObj.Value))
				action(actObj);
		}
	}

}
