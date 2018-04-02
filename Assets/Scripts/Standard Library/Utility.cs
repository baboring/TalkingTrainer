using UnityEngine;

using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Common.Utilities {

	public enum ColorType
	{
		black,
		white,
		red,
		yellow,
		blue,
		green,
		gray,
		cyan,
		magenta,
	}
	
	static public class Facade
	{
		// setting layer number.
		static public void SetLayerRecursively(GameObject _object, System.Int32 _layer)
		{
			if (null == _object || _layer < 0)
				return;
			_object.layer = _layer;
			foreach( Transform child in _object.transform ) {
				if( child == null ) {
					continue;
				};
				SetLayerRecursively(child.gameObject, _layer);
			};

			return;
		}

		// Random
		static public System.Int32 GetRandomInt( System.Int32 _max_value )
		{
			return UnityEngine.Random.Range(0, _max_value);
		}

		// file path
		static public string pathForDocumentsFile(string filename)
		{
			if (Application.platform == RuntimePlatform.IPhonePlayer) {
				string path = Application.dataPath.Substring(0, Application.dataPath.Length - 5);
				path = path.Substring(0, path.LastIndexOf('/'));
				return Path.Combine(Path.Combine(path, "Documents"), filename);
			}
			else if (Application.platform == RuntimePlatform.Android) {
				string path = Application.persistentDataPath;
				path = path.Substring(0, path.LastIndexOf('/'));
				return Path.Combine(path, filename);
			}
			else {
				string path = Application.dataPath;
				path = path.Substring(0, path.LastIndexOf('/'));
				return Path.Combine(path, filename);
			}
		}

		// Detect cross vector
		static public bool CheckCross(Vector2 AP1, Vector2 AP2, Vector2 BP1, Vector2 BP2, ref Vector2 result)
		{
			float t;
			float s;
			float under = (BP2.y - BP1.y) * (AP2.x - AP1.x) - (BP2.x - BP1.x) * (AP2.y - AP1.y);
			if (under == 0)
				return false;

			float _t = (BP2.x - BP1.x) * (AP1.y - BP1.y) - (BP2.y - BP1.y) * (AP1.x - BP1.x);
			float _s = (AP2.x - AP1.x) * (AP1.y - BP1.y) - (AP2.y - AP1.y) * (AP1.x - BP1.x);

			if (_t == 0 && _s == 0)
				return false;

			t = _t / under;
			s = _s / under;

			if (t < 0.0 || t > 1.0 || s < 0.0 || s > 1.0)
				return false;

			//result.x = AP1.x + t * (AP2.x-AP1.x);
			//result.y = AP1.y + t * (AP2.y-AP1.y);
			result.x = BP1.x + s * (BP2.x - BP1.x);
			result.y = BP1.y + s * (BP2.y - BP1.y);

			return true;
		}

		static public int RandomRange(int min_include, int max_include)	// max가 int는 exclusive 이고 float 는 include다 ( 설명 미스매치다~~~)
		{
			return UnityEngine.Random.Range(min_include, max_include + 1);
		}
		static public float RandomRange(float min_include, float max_include)	// max가 int는 exclusive 이고 float 는 include다 ( 설명 미스매치다~~~)
		{
			return UnityEngine.Random.Range(min_include, max_include);
		}

		// topParentObj 의 하위 컨포넌트에서 해당 이름의 GO를 찾는다.
		static public GameObject FindGameObjectInChildren(GameObject topParentObj, string objName)
		{
			Transform[] childTr = topParentObj.GetComponentsInChildren<Transform>();
			var result = childTr.Where(v => v.name == objName);
			if (result.Count() < 1)
				return null;
			Transform findTr = result.First();
			if (findTr != null)
				return findTr.gameObject;
			return null;
		}
	}


	public static class StringExtensionMethods
	{
		/// Sets the color of the text according to the parameter value.
		public static string ColorFormat(this string message,Colors color,string format,params object[] args)
		{
			return string.Format("<color={0}>{1}</color>", color.ToString(), string.Format(format, args));
		}
		public static string Colored(this string message, Colors color)
		{
			return string.Format("<color={0}>{1}</color>", color.ToString(), message);
		}

		/// Sets the color of the text according to the traditional HTML format parameter value.

		public static string Colored(this string message, string colorCode)
		{
			return string.Format("<color={0}>{1}</color>", colorCode, message);
		}

		/// Sets the size of the text according to the parameter value, given in pixels.

		public static string Sized(this string message, int size)
		{
			return string.Format ("<size={0}>{1}</size>", size, message);
		}

		/// Renders the text in boldface.

		public static string Bold(this string message)
		{
			return string.Format ("<b>{0}</b>", message);
		}


		/// Renders the text in italics.

		public static string Italics(this string message)
		{
			return string.Format ("<i>{0}</i>", message);
		}
	}

	public enum Colors
	{
		aqua,
		black,
		blue,
		brown,
		cyan,
		darkblue,
		fuchsia,
		green,
		grey,
		lightblue,
		lime,
		magenta,
		maroon,
		navy,
		olive,
		purple,
		red,
		silver,
		teal,
		white,
		yellow
	}


	public static class LogExtensionMethods
	{
		static public void LogFormatColor(this Debug target, ColorType color,string format, params object[] args)
		{
			Debug.Log(string.Format("<color={0}>{1}</color>", color.ToString(), string.Format(format, args)));
		}			
		static public void LogFormat(this Debug target, ColorType color,string format, params object[] args)
		{
			Debug.Log(string.Format("<color={0}>{1}</color>", color.ToString(), string.Format(format, args)));
		}	
		static public void Log(this Debug target, ColorType color,string arg)
		{
			Debug.Log(string.Format("<color={0}>{1}</color>", color.ToString(), arg));
		}			
		static public Transform Search(this Transform target, string name)
		{
			if (target.name == name) return target;

			for (int i = 0; i < target.childCount; ++i) {
				var result = Search(target.GetChild(i), name);

				if (result != null) return result;
			}

			return null;
		}

		// Get or Add component one time
		static public T GetOrAddComponent<T>(this Component child) where T : Component {
			T result = child.GetComponent<T>();
			if (result == null) {
				result = child.gameObject.AddComponent<T>();
			}
			return result;
		}

		// transforms
		static public void SetLocalPosX(this Transform t, float newX) {
			t.localPosition = new Vector3(newX, t.localPosition.y, t.localPosition.z);
		}
		static public void SetLocalPosY(this Transform t, float newY) {
			t.localPosition = new Vector3(t.localPosition.x, newY, t.localPosition.z);
		}
		static public void SetLocalPosZ(this Transform t, float newZ) {
			t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, newZ);
		}
		static public void SetLocalPosXY(this Transform t, float newX, float newY) {
			t.localPosition = new Vector3(newX, newY, t.localPosition.z);
		}
		static public void SetLocalPosYZ(this Transform t, float newY, float newZ) {
			t.localPosition = new Vector3(t.localPosition.x, newY, newZ);
		}
		static public void SetLocalPosXZ(this Transform t, float newX, float newZ) {
			t.localPosition = new Vector3(newX, t.localPosition.y, newZ);
		}

		

	}

}