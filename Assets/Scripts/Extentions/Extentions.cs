using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using System.ComponentModel;
using System;
using System.Reflection;


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

/// <summary>
/// [ <c>public static object GetDefault(this Type type)</c> ]
/// <para></para>
/// Retrieves the default value for a given Type
/// </summary>
/// <param name="type">The Type for which to get the default value</param>
/// <returns>The default value for <paramref name="type"/></returns>
/// <remarks>
/// If a null Type, a reference Type, or a System.Void Type is supplied, this method always returns null.  If a value type 
/// is supplied which is not publicly visible or which contains generic parameters, this method will fail with an 
/// exception.
/// </remarks>
/// <example>
/// To use this method in its native, non-extension form, make a call like:
/// <code>
///     object Default = DefaultValue.GetDefault(someType);
/// </code>
/// To use this method in its Type-extension form, make a call like:
/// <code>
///     object Default = someType.GetDefault();
/// </code>
/// </example>
/// <seealso cref="GetDefault&lt;T&gt;"/>

public static class DefaultExtention {

    public static object Default(Type maybeNullable)
    {
        Type underlying = Nullable.GetUnderlyingType(maybeNullable);
        if (underlying != null)
            return Activator.CreateInstance(underlying);
        return Activator.CreateInstance(maybeNullable);
    }
    
    public static object GetDefault(this Type type)
    {
        // If no Type was supplied, if the Type was a reference type, or if the Type was a System.Void, return null
        if (type == null || !type.IsValueType || type == typeof(void))
            return null;

        // If the supplied Type has generic parameters, its default value cannot be determined
        if (type.ContainsGenericParameters)
            throw new ArgumentException(
                "{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type +
                "> contains generic parameters, so the default value cannot be retrieved");

        // If the Type is a primitive type, or if it is another publicly-visible value type (i.e. struct/enum), return a 
        //  default instance of the value type
        if (type.IsPrimitive || !type.IsNotPublic)
        {
            try
            {
                return Activator.CreateInstance(type);
            }
            catch (Exception e)
            {
                throw new ArgumentException(
                    "{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe Activator.CreateInstance method could not " +
                    "create a default instance of the supplied value type <" + type +
                    "> (Inner Exception message: \"" + e.Message + "\")", e);
            }
        }

        // Fail with exception
        throw new ArgumentException("{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type + 
            "> is not a publicly-visible type, so the default value cannot be retrieved");
    }    
}