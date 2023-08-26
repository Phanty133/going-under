using System.Collections.Generic;
using UnityEngine;

public static class CollectionHelpers
{
    public static void Shuffle<T>(this IList<T> list)  
    {  
        int n = list.Count;  
        while (n > 1) {  
            n--;
            int k = Random.Range(0, n + 1); 
            (list[k], list[n]) = (list[n], list[k]);
        }  
    }
    
    public static T PopAt<T>(this List<T> list, int index)
    {
        T r = list[index];
        list.RemoveAt(index);
        return r;
    }
}