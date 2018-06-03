using UnityEngine;
using UnityEditor;

namespace Common
{
    using ActionBehaviour;
    public class AssetCreateMenu
    {
        [MenuItem("Assets/Create/Data/StringSet")]
        public static void CreateAsset()
        {
            ScriptableObjectUtility.CreateAsset<StringSet>();
        }
    }
}