using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.ActionScript {

	public class ActionObserver : MonoBehaviour {

		[SerializeField]
		protected ActionTrigger[] list;
		// Use this for initialization
		void Start () {
			list = GetComponents<ActionTrigger>();

			foreach(var ev in list)
				Debug.Log(ev.Name);
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}

}