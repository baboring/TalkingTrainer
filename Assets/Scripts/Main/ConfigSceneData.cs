using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Abilities/ConfigSceneData")]
public class ConfigSceneData : ScriptableObject {

	[SerializeField]
	public string tableBundleName;
	[SerializeField]
	public string tableAssetName;

	[SerializeField]
	public string[] dependencyBundles;
}
