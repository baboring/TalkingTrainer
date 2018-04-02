using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTrainer {

	using Common.Utilities;
	using TMPro;

	public class LoadingScreen : MonoSingleton<LoadingScreen> {

		[SerializeField]
		protected TMP_Text textProgressing;
		static public void initial() {
			if(!LoadingScreen.isInstanced) {
				GameObject go = Instantiate(Resources.Load("Prefabs/LoadingScreen (Canvas)")) as GameObject; 
			}
		}

		void Update() {
			if(null != textProgressing) {
				var percent = GlobalBlackBoard.GetValue<float>("loading progress");
				textProgressing.text = string.Format("{0:0} %",percent);
			}
		}
	}

}