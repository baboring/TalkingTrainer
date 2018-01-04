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

		public AudioClip soundTop {
			private set;
			get;
		}
		public AudioClip soundBottom {
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

			ResourceManager.instance.StartCoroutine(ResourceManager.instance.LoadAssetBundle(new AssetInfo("cdata002-sound-mid-unit1-bundle", info.Question_File,(bundleLoaded)=>{
				soundTop = bundleLoaded.GetAsset<AudioClip>();
			})));				
			ResourceManager.instance.StartCoroutine(ResourceManager.instance.LoadAssetBundle(new AssetInfo("cdata002-sound-mid-unit1-bundle", info.Answer_File,(bundleLoaded)=>{
				soundBottom = bundleLoaded.GetAsset<AudioClip>();
			})));				
				
		}		
	}
}