/* *************************************************
*  Created:  2018-1-21 20:52:30
*  File:     GlobalBlackBoard.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Utilities {
    
	public class GlobalBlackBoard : MonoSingleton<GlobalBlackBoard> {

        static BlackBoard blackboard = new BlackBoard();

        // Set value
		static public bool SetValue<T>(string key, T val) {
            return blackboard.SetValue<T>(key, val);
		}

		// Get Value
		static public T GetValue<T>(string key) {
            return blackboard.GetValue<T>(key);
		}
		

		// Get Variable Object
		static public Variable GetVariable<T>(string key) {
            return blackboard.GetVariable<T>(key);
		}
	}
}
