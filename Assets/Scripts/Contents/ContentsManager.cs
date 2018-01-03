using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TTrainer {

	public class ContentsManager : SingletonMono<ContentsManager> {

		public GameObject mainManu;

		[System.NonSerialized]
		public Dictionary<string,AssetBundle> dicContents =  new Dictionary<string,AssetBundle>();

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		// called second
		void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			Debug.Log("OnSceneLoaded: " + scene.name);
			Debug.Log(mode);
		}
	}
}