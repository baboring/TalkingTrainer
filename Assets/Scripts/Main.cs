using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace TTrainer {


	public class Main : MonoBehaviour {

		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		public void StartGame() {
			Debug.Log("StartGame");
			AssetLoader.instance.LoadTest();
		}
		public void UnloadScene() {
			AssetLoader.instance.UnloadTest();
		}
	}
}