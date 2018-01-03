using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class TransformExtention {
     public static Transform Clear(this Transform transform)
     {
         foreach (Transform child in transform) {
			 if(child.gameObject.activeSelf)
	             GameObject.Destroy(child.gameObject);
         }
         return transform;
     }
 }
