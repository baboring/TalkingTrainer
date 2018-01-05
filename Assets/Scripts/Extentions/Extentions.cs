﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

public static class StringExtensions { 
    public static bool StartsWithAny(this string s, IEnumerable<string> items) { 
        return items.Any(i => s.StartsWith(i)); 
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
