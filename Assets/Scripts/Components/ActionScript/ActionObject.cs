using System.Collections;
using UnityEngine;

namespace Common.ActionScript {

	public abstract class ActionObject : ScriptableObject {

		public string Name = "New Action";

		public Variable Value;

		public abstract void Initialize(GameObject obj);
		public abstract void TriggerEvent(object arg = null);

	}

}