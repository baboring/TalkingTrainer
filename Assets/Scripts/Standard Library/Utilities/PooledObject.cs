/* *************************************************
*  Created:  7/20/2017, 2:05:05 PM
*  File:     PooledObject.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/
using UnityEngine;

namespace Common.Utilities {

	public class PooledObject : MonoBehaviour {

		[System.NonSerialized]
		ObjectPool poolInstanceForPrefab;
		public ObjectPool instance {
			get { return poolInstanceForPrefab;}
		}

		public T Instanciate<T> (T arg=default(T)) where T : PooledObject {
			if (!poolInstanceForPrefab) {
				poolInstanceForPrefab = ObjectPool.CreateObjectPool(this);
			}
			return (T)poolInstanceForPrefab.GetObject(arg);
		}

		public ObjectPool poolHandler { get; set; }

		public void ReturnToPool () {
			if (poolHandler) {
				poolHandler.AddObject(this);
			}
			else {
				Logger.Debug("I die!");
				Destroy(gameObject);
			}
		}
	}
}