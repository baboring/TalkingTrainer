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

		[SerializeField]
		protected AudioSource audioSource;
	
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

		public void PlaySound(string szText) {

			if(null == audioSource)
				audioSource = gameObject.AddComponent<AudioSource>();
			Debug.Log("PlaySound:"+szText);
			if(szText == "PlayTop")
				audioSource.PlayOneShot(soundTop);
			else
				audioSource.PlayOneShot(soundBottom);
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

			ResourceManager.instance.StartCoroutine(ResourceManager.instance.LoadAssetBundle(
				new AssetInfo(info.AssetBundle, info.Question_File,typeof(AudioClip),(bundleLoaded)=>{
				soundTop = bundleLoaded.GetAsset<AudioClip>();
				Debug.Assert(soundTop != null,"soundTop is null (audioClip Asset) "+info.Question_File);
			})));				
			ResourceManager.instance.StartCoroutine(ResourceManager.instance.LoadAssetBundle(
				new AssetInfo(info.AssetBundle, info.Answer_File,typeof(AudioClip),(bundleLoaded)=>{
				soundBottom = bundleLoaded.GetAsset<AudioClip>();
				Debug.Assert(soundBottom != null,"soundTop is null (audioClip Asset) "+info.Answer_File);
			})));				
				
		}		
	}
}