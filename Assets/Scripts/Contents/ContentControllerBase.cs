using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TTrainer {

	public class ContentControllerBase : MonoBehaviour {

		[SerializeField]
		protected ContentData info;

		public void Exit() {
			TTrainer.Main.instance.UnloadScene();
		}
	}
}