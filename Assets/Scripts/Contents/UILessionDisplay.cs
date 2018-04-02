using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TTrainer {

	using Common.Utilities;
	public class UILessionDisplay : MonoBehaviour {

		[SerializeField]
		protected Image imgLogo;
		[SerializeField]
		protected Text displayTitle;
		[SerializeField]
		protected Text displayDesc;

		public InfoLessonData info {
			private set;
			get;
		}


		public void Prime(string bundleName, InfoLessonData info) {
			if(null == info)
				return;
			this.info = info;
			// if(null != imgLogo)
			// 	imgLogo.sprite = info.logo;
			if(null != displayTitle)
				displayTitle.text = info.TitleName;
			if(null != displayDesc)
				displayDesc.text = string.Format("Total : 0");

			Action<int> funcUpdateTotal = new Action<int>((total)=>{
				if(null != displayDesc)
					displayDesc.text = string.Format("Total : {0}",total);
			});

			if(info.lstStudyBoard == default(InfoStudyBoardData[])) {
				StartCoroutine(ResourceManager.instance.LoadAssetBundle(
					new AssetInfo(bundleName, info.SheetName,typeof(TextAsset), (bundleLoaded)=>{
					var lstBoard = CsvUtil.LoadObjects<InfoStudyBoardData>(bundleLoaded.GetAsset<TextAsset>(),false);
					info.lstStudyBoard = lstBoard.ToArray();
					Debug.Log("Load asset " + info.SheetName);
					funcUpdateTotal(info.lstStudyBoard.Length);
				})));
			}
			else 
				funcUpdateTotal(info.lstStudyBoard.Length);
		}

		
	}
}