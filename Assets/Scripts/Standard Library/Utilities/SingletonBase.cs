
/********************************************************************
	created:	2017/07/19
	filename:	Singleton.cs
	author:		Benjamin
	purpose:	[managed singleton for MonoBehaviour and classes]
*********************************************************************/
using UnityEngine;
using System;

namespace Common.Utilities { 

	public abstract class SingletonBase<T> where T : class, new()
	{
		static protected T _instance;
		static public T instance
		{
			get { return _instance; }
			set
			{
				if (_instance != null)
					throw new System.ApplicationException("cannot set Instance twice!");

				_instance = value;
			}
		}
		public SingletonBase() {
			if (_instance != null)
				throw new System.ApplicationException("cannot set Instance twice!");
		}

		static public bool isInstanced { get { return  null != _instance; } }

		static protected T Instantiate()
		{
			instance = new T();
			return instance;
		}

		static public void Destroy()
		{
			_instance = null;
		}



	}

}