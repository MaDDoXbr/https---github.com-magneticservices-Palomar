using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Extensions {

//	public static float InverseLerp(this Vector3 value, Vector3 a, Vector3 b) 
//	{
//		var avg = (Mathf.InverseLerp(a.x, b.x, value.x) +
//			Mathf.InverseLerp(a.y, b.y, value.y)
////			+ Mathf.InverseLerp(a.z, b.z, value.z) +    <= Currently doesn't account for z
//			)/2f;
//		//Debug.Log(avg);
//		return avg;
//	}
	
	public static Vector3[] ToArray (this IList<Vector3> vertices ) 
    {
		var array = new Vector3[vertices.Count];
		for (int i = 0; i < vertices.Count; i++) {
			array[i] = vertices[i];
		}
		return array;
	}

    //TODO: Optimize and don't use List conversion
    // call it using element = element.Add
    public static T[] Add<T>(this T[] array, T element) {
        var tmp = new List<T>(array);
        tmp.Add(element);
        return tmp.ToArray();
    }

    //TODO: Optimize and don't use List conversion    
    public static T[] RemoveAt<T>(this T[] array, int idx)
    // call it using element = element.RemoveAt
    {
        var tmp = new List<T>(array);
        tmp.RemoveAt(idx);
        return tmp.ToArray();           
    }

    //TODO: Optimize and don't use List conversion
    public static bool Contains<T>(this T[] array, T element) {
        var tmp = new List<T>(array);
        return tmp.Contains(element);
    }
}
