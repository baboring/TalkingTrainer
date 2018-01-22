/* *************************************************
*  Created:  2018-1-21 20:52:30
*  File:     ConfigManager.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Utilities {
	public class ConfigManager : SingletonMono<ConfigManager> {

		Dictionary<System.Type, Dictionary<string,Variable>> dicVariables = new Dictionary<System.Type, Dictionary<string,Variable>>();

		static public bool SetValue<T>(string key, T val) {
			var v = GetTable(val.GetType());
			if(v.ContainsKey(key))
				return v[key].SetValue(val);
			v.Add(key, new Variable(val.GetType(),val));
			return true;
		}

		// Get Value
		static public T GetValue<T>(string key) {
			var v = GetVariable<T>(key);
			if(null != v)
				return (T)v.Value;
			return default(T);
		}
		

		// Get Variable Object
		static public Variable GetVariable<T>(string key) {
			var dic = GetTable(typeof(T));
			// find key
			if(dic.ContainsKey(key))
				return dic[key];
			return null;
		}

		// find or new table for T type
		static Dictionary<string,Variable> GetTable(System.Type type){
			var v = instance;
			Dictionary<string,Variable> dic;
			if(!v.dicVariables.ContainsKey(type)){
				dic = new Dictionary<string,Variable>();
				v.dicVariables.Add(type,dic);
			}
			else {
				dic = v.dicVariables[type];
			}
			return dic;
		}

	}
}
