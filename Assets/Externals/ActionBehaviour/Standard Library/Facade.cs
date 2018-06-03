using UnityEngine;

using System.IO;
using System.Linq;

namespace Common.Utilities {

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

    
   
 

}