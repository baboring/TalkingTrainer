using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Contents/Course")]

public class ContentData : ScriptableObject {
	public long uid;
	public string Title;
	public string Description;
	public string bundle;			// Scene bundle
	public string sceneName;		// Scene Name
	public string downloadUrl;
	public int version;

	public Sprite logo;
}
