using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TTrainer {

	[RequireComponent(typeof(Button))]
	public class ClickSound : MonoBehaviour {

		[SerializeField]
		private AudioClip sound;

		[SerializeField]
		private AudioSource source;

		private Button button {
			get {
				return GetComponent<Button>();
			}
		}

		void Start() {
			Reset();
			button.onClick.AddListener(()=>PlaySound());
		}

		void Reset() {
			if( null == source)
				gameObject.AddComponent<AudioSource>();
			if(source != null) {
				source.clip = sound;
				source.playOnAwake = false;
			}
		}
		
		public void PlaySound() {
			if(null != source)
				source.PlayOneShot(sound);
		}

		#if UNITY_EDITOR
			[CustomEditor(typeof(TTrainer.ClickSound))]
			public class ClickSoundEditor : Editor
			{
				public override void OnInspectorGUI()
				{
					base.OnInspectorGUI();

					TTrainer.ClickSound myScript = (TTrainer.ClickSound)target;
					if(GUILayout.Button("Apply Info"))
					{
						myScript.Reset();
					}
				}
			}
		#endif	
	}
}