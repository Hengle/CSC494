using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities {

public static class ListUtilities 
{
    public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> collection, int n)
    {
        if (collection == null)
            throw new ArgumentNullException("collection");
        if (n < 0)
            throw new ArgumentOutOfRangeException("n", "n must be 0 or greater");

        LinkedList<T> temp = new LinkedList<T>();

        foreach (var value in collection)
        {
            temp.AddLast(value);
            if (temp.Count > n)
                temp.RemoveFirst();
        }

        return temp;
    }

    public static T FindObjectOfType<T>(this IEnumerable<GameObject> collection, T stock) 
    {
        foreach (GameObject obj in collection) 
        {
            if (obj.GetComponent<T>() != null) return obj.GetComponent<T>();
        }

        return stock;
    }

    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self) 
    {
        return self.Select((item, index) => (item, index)); 
    }

    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue val) 
    {
        key = kvp.Key;
        val = kvp.Value;
    }

    public static void Populate<T>(this T[] arr, T value) 
    {
        for (int i = 0; i < arr.Length; i++) 
        {
            arr[i] = value;
        }
    }

    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (T element in source)
        {
            action(element);
        }
    }

    // Returns source[i : j] (inclusive)
    public static T[] Range<T>(this T[] source, int? i = null, int? j = null)
    {
        int start = i.HasValue ? i.Value : 0;
        int end = j.HasValue ? j.Value : source.Length - 1;

        T[] result = new T[end - start + 1];

        for (int x = start; x <= end; x++)
        {
            result[x - start] = source[x];
        }

        return result;
    }

    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
        TValue value;
        return dictionary.TryGetValue(key, out value) ? value : defaultValue;
    }

    public static int IndexOf<T>(this T[] array, T element) 
    {
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;

        for (int i = 0; i < array.Length; i++)
        {
            if (comparer.Equals(array[i], element))
                return i;
        }

        return -1;
    }

    public static T[] SubArray<T>(this T[] data, int index, int length)
    {
        T[] result = new T[length];
        Array.Copy(data, index, result, 0, length);
        return result;
    }

    public static void AddOrSet<T, T2>(this Dictionary<T, T2> dictionary, T key, T2 value) 
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] = value;
        }
        else
        {
            dictionary.Add(key, value);
        }
    }
}}