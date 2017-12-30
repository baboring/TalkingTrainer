/********************************************************************
	created:	2017/07/19
	filename:	Singleton.cs
	author:		Benjamin
	purpose:	[managed singleton for MonoBehaviour and classes]
*********************************************************************/
using UnityEngine;
using System;


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



public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
{
	private static T _instance;
	private static object _lock = new object();

	protected Action DestroyEventHandler;	// when object is deleted called

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
	protected virtual void Awake()	{
		Debug.Log("Awake singleton : " + typeof(T));
		if (null == _instance)
			_instance = (T)this;
	}

	public virtual T Create() {
		return Instantiate();
	}

	public static bool IsInstanced { get { return  null != _instance; } }

	static protected T Instantiate() {

		_instance = (T)FindObjectOfType(typeof(T));
		if (null == _instance) {
			GameObject obj = new GameObject(typeof(T).ToString());
			_instance = obj.AddComponent<T>();
			if (null == _instance)
				Debug.LogError("FATAL! Cannot create an instance of " + typeof(T) + ".");
			else {
				DontDestroyOnLoad(obj);
			}
		}
		else {
			Debug.Log("Aleady Instance of " + typeof(T) + " exists in the scene.");
		}
		return _instance;
	}
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

		if (DestroyEventHandler != null)
			DestroyEventHandler();

		//Debug.Log("Singleton object destroy");
	}
}

