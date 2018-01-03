using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TTrainer {

	public class UIStudyBoard : MonoBehaviour {

		[SerializeField]
		protected Text txtTop;
		[SerializeField]
		protected Text txtBottom;

		public InfoStudyBoardData info {
			private set;
			get;
		}
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		public void Prime(string bundleName, InfoStudyBoardData info) {
			if(null == info)
				return;
			this.info = info;
			// if(null != imgLogo)
			// 	imgLogo.sprite = info.logo;
			if(null != txtTop)
				txtTop.text = info.Sentence;
			if(null != txtBottom)
				txtBottom.text = info.Translate;

			// StartCoroutine(ResourceManager.instance.LoadAssetBundle(new AssetInfo(bundleName, info.SheetName,(bundleLoaded)=>{
			// 	var lstBoard = R.CsvUtil.LoadObjects<InfoStudyBoardData>(bundleLoaded.GetAsset<TextAsset>());
			// 	info.lstLession = lstBoard.ToArray();
			// 	Debug.Log("Load csv - lessionUnitTable");
			// 	if(null != displayDesc)
			// 		displayDesc.text = string.Format("Total : {0}",info.lstLession.Length);
			// })));
		}		
	}
}