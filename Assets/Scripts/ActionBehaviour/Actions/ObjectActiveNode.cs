
/* *************************************************
*  Created:  2018-1-28 20:15:39
*  File:     ObjectActiveNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {

	public class ObjectActiveNode : ActionNode {

		[SerializeField]
		protected GameObject[] objects;
		protected override void OnStart() {
			for( int i=0;i < objects.Length; ++i )
				objects[i].SetActive(true);
		}
	}

}