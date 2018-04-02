/********************************************************************
	created:	2017/07/19
	filename:	Singleton.cs
	author:		Benjamin
	purpose:	[managed singleton for MonoBehaviour and classes]
*********************************************************************/
using UnityEngine;
using System;

namespace Common.Utilities {

	public abstract class Singleton<T> : SingletonBase<T> where T : class, new()
	{
		static new public T instance {
			get {
				if (_instance == null)
					return Instantiate();
				return _instance;
			}
			set {
				if (_instance != null)
					throw new System.ApplicationException("cannot set Instance twice!");

				_instance = value;
			}
		}

		public Singleton() {
			if (_instance != null)
				throw new System.ApplicationException("cannot set Instance twice!");
		}
	}



	public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
		private static T _instance;
		private static object _lock = new object();

		public static bool isInstanced { get { return  null != _instance; } }

		// Returns the instance of the singleton
		public static T instance
		{
			get
			{
				lock (_lock) {
					if (null == _instance)
						return Instantiate();
				}
				return _instance;
			}
		}

        // Awake
		protected virtual void Awake()	{
            Logger.Log(LogLevel.DEBUG,"Awake singleton : " + typeof(T));
            if (null == _instance) {
                _instance = (T)this;
                if(this.transform.parent == null)
                    DontDestroyOnLoad(this.gameObject);
            }
		}

		public virtual T Create() {
			return Instantiate();
		}

		static protected T Instantiate() {

			_instance = (T)FindObjectOfType(typeof(T));
			if (null == _instance) {
				GameObject obj = new GameObject(typeof(T).ToString());
				_instance = obj.AddComponent<T>();
				if (null == _instance)
					Debug.LogError("FATAL! Cannot create an instance of " + typeof(T) + ".");
			}
			else {
				Debug.Log("Aleady Instance of " + typeof(T) + " exists in the scene.");
			}
			return _instance;
		}

        // Destroy
		public static void SelfDestroy()
		{
			if (null != _instance) {
				DestroyImmediate( _instance.gameObject );
				//_instance = null;
			}
		}

		void OnApplicationQuit()
		{
			_instance = null;
			_lock = null;
		}

		void OnDestroy()
		{
			if (this != _instance)
				return;
			_instance = null;

			//Debug.Log("Singleton object destroy");
		}
	}

}