using UnityEngine;
using System.Collections.Generic;

namespace ActionBehaviour
{
    using NaughtyAttributes;

    public class StringSet : ScriptableObject
    {
        [ReorderableList]
        [SerializeField]
        protected List<string> List;

        public List<string>.Enumerator GetEnumerator()
        {
            return List.GetEnumerator();
        }

        public List<string> GetList()
        {
            return List;
        }

    }


}