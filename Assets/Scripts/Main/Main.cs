﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace TTrainer {

	using Common.Utilities;
	
	public class Main : MonoSingleton<Main> {

		[SerializeField]
		private ConfigSceneData config;

		private string lastSceneBundle = "";
		// Use this for initialization
		IEnumerator Start () {
			Logger.Init();
			GlobalBlackBoard.SetValue<int>("Version",1);
			Logger.Debug("Version:"+GlobalBlackBoard.GetValue<int>("Version"));
			
			if(null != config) {
				AssetInfo assetInfo = new AssetInfo(config.tableBundleName, config.tableAssetName,typeof(object));
				//Resources.instance.LoadAsyncLessionList(assetInfo);

				if(!assetInfo.isLoaded)
					yield return null;
			}
		}
		
		public void UnloadScene() {
			ContentsManager.instance.mainManu.SetActive(true);
			ResourceManager.UnloadScene(lastSceneBundle);
		}

		public void Download(Button button,ContentData info) {
			if(!button.interactable)
				return;
			button.interactable = false;
			StartCoroutine(downloadContent(button, info));
		}

		public void Enter(Button button, ContentData info) {
			if(!button.interactable)
				return;
			button.interactable = false;
			StartCoroutine(enterContent(button, info));
		}
		IEnumerator downloadContent(Button button, ContentData info) {
			// Load variant level which depends on variants.
			yield return StartCoroutine(ResourceManager.instance.InitializeLevelAsync (info.bundle, info.sceneName, true) );

			yield return null;
			button.interactable = true;
		}

		IEnumerator enterContent(Button button, ContentData info) {
			// Load variant level which depends on variants.
			yield return StartCoroutine(ResourceManager.instance.InitializeLevelAsync (info.bundle, info.sceneName, true) );

			ContentsManager.instance.mainManu.SetActive(false);
			lastSceneBundle = info.sceneName;
			yield return null;
			button.interactable = true;
		}		
	}
}