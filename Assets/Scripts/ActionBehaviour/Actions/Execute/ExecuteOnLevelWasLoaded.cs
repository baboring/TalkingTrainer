/* *************************************************
*  Created:  2018-1-28 19:46:32
*  File:     ExecuteOnLevelWasLoaded.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using UnityEngine;
using UnityEngine.SceneManagement;

namespace ActionBehaviour {

    using Common.Utilities;

    public class ExecuteOnLevelWasLoaded : Execute {

        // called end of loaded level
        void OnLevelWasLoaded(int level)
        {
            Logger.DebugFormat("OnLevelWasLoaded: {0}", level);
            Execute();
        }
    }
}
