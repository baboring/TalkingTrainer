using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TTrainer {

	public class UILessionDisplay : MonoBehaviour {

		[SerializeField]
		protected Image imgLogo;
		[SerializeField]
		protected Text displayTitle;
		[SerializeField]
		protected Text displayDesc;

		public LessionInfo info {
			private set;
			get;
		}

		public List<LessionUnitInfo> _lessionUnitTable = null;
		public void Prime(string bundleName, LessionInfo info) {
			if(null == info)
				return;
			this.info = info;
			// if(null != imgLogo)
			// 	imgLogo.sprite = info.logo;
			if(null != displayTitle)
				displayTitle.text = info.TitleName;
			if(null != displayDesc)
				displayDesc.text = string.Format("Total : 0");

			StartCoroutine(ResourceManager.instance.LoadAssetBundle(new AssetInfo(bundleName, info.SheetName,(bundleLoaded)=>{
				_lessionUnitTable = R.CsvUtil.LoadObjects<LessionUnitInfo>(bundleLoaded.GetAsset<TextAsset>());
				Debug.Log("Load csv - lessionUnitTable");
				if(null != displayDesc)
					displayDesc.text = string.Format("Total : {0}",_lessionUnitTable.Count);
			})));
		}

		
	}
}