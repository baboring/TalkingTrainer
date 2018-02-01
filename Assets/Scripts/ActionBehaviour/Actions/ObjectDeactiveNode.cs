
/* *************************************************
*  Created:  2018-1-28 20:15:39
*  File:     ObjectDeactiveNode.cs
*  Author:   Benjamin
*  Purpose:  []
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionBehaviour {

	public class ObjectDeactiveNode : ActionNode {

		[SerializeField]
		protected GameObject[] objects;
		protected override void OnStart() {
			for( int i=0;i < objects.Length; ++i )
				objects[i].SetActive(false);
		}
	}

}