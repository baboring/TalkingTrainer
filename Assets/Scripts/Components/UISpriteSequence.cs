using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TTrainer {

	[RequireComponent(typeof(Image))]
	public class UISpriteSequence : MonoBehaviour {

		// array of sprite
		[SerializeField]
		protected Sprite[] List;

		// target sprite 
		[SerializeField]
		protected Image target;

		[SerializeField]
		protected float delay;


		[SerializeField]
		protected bool isLoop;	// infinite repeating

		[SerializeField]
		protected int repeatCount;
		
		Coroutine coroutine = null;
		// Use this for initialization

		void OnEnable()
		{
			coroutine = StartCoroutine(Run(delay));
		}

		void OnDisable()
		{
			if(null != coroutine)
				StopCoroutine(coroutine);
		}
		
		// Update is called once per frame
		IEnumerator Run (float delay) {

			do {

				for(int i=0;i<List.Length;++i) {
					target.sprite = List[i];
					if(delay > 0)
						yield return new WaitForSeconds(delay);	// seconds
					else
						yield return null; 
				}

				if(repeatCount > 0)
					repeatCount--;

			} while(repeatCount != 0 || isLoop);

			coroutine = null;
			yield break;
		}
	}

}