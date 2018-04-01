/* *************************************************
*  Created:  2018-1-28 19:46:32
*  File:     ExecuteOnSceneLoaded.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;

namespace ActionBehaviour {

    using Common.Utilities;

    public class ExecuteOnSceneLoaded : Execute {

        // called first
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        // called when the game is terminated
        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        } 

        // called second
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Logger.DebugFormat("OnSceneLoaded: {0},Mode = {1}", scene.name, mode.ToString());
            Execute();
        }
	}
}
