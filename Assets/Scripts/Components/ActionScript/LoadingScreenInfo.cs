using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common.ActionScript;

[CreateAssetMenu (menuName = "ActionObject/LoadingScreenInfo")]
public class LoadingScreenInfo : ActionObject {

	[SerializeField]
	protected GameObject prefab;
	public override void Initialize(GameObject obj) {

	}
	public override void TriggerEvent(object arg = null) {
		
	}
}
