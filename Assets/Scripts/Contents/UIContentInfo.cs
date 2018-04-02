using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

using TMPro;

namespace TTrainer {
	public class UIContentInfo : MonoBehaviour {

		[SerializeField]
		protected Image imgLogo;
		[SerializeField]
		protected TMP_Text displayTitle;
		[SerializeField]
		protected TMP_Text displayDesc;

		[SerializeField]
		protected Button buttonDownload;
		[SerializeField]
		protected Button buttonEnter;
		


		[SerializeField]
		protected ContentData info;
		// Use this for initialization
		void Start () {

			if(null != info)
				Prime(info);
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		public void Download(Button button) {
			Main.instance.Download(button, info);
		}

		public void Enter(Button button) {
			Main.instance.Enter(button, info);
		}
		public void Prime(ContentData info) {
			if(null == info)
				return;
			if(null != imgLogo)
				imgLogo.sprite = info.logo;
			if(null != displayTitle)
				displayTitle.text = string.Format("[ {0} ]",info.Title);
			if(null != displayDesc)
				displayDesc.text = string.Format("{0}",info.Description);
		}


	#if UNITY_EDITOR
		[CustomEditor(typeof(TTrainer.UIContentInfo))]
		public class UIContentInfoEditor : Editor
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				TTrainer.UIContentInfo myScript = (TTrainer.UIContentInfo)target;
				if(GUILayout.Button("Apply Info"))
				{
					myScript.Prime(myScript.info);
				}
			}
		}
	#endif	
	}
}