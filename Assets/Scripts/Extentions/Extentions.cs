using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.ComponentModel;

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

 
public class IosCompat
{
    private static Int32Converter i32 = new Int32Converter();
    private static Int64Converter i64 = new Int64Converter();
    private static BooleanConverter booleanConv = new BooleanConverter();
    private static StringConverter stringConv = new StringConverter();
    private static SingleConverter singleConv = new SingleConverter();
}
