using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static float Remap(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public static float Remap11_01(this float value)
    {
        return Remap(value, -1f, 1f, 0f, 1f);
    }

    /// <summary>
    /// Obtained from here -> https://stackoverflow.com/questions/24644846/random-shuffle-listing-in-unity-3d
    /// Seems like it may have been a faulty method as we got a bug every once in a while. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_list"></param>
    /// <returns></returns>
    public static List<T> Shuffle<T>(List<T> _list)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            T temp = _list[i];
            int randomIndex = Random.Range(i, _list.Count);
            _list[i] = _list[randomIndex];
            _list[randomIndex] = temp;
        }

        return _list;
    }

    /// <summary>
    /// Obtained from here on Malachi's rec -> https://stackoverflow.com/questions/108819/best-way-to-randomize-an-array-with-net/110570#110570
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="rng"></param>
    /// <param name="array"></param>
    public static void ShuffleArray<T>(this System.Random rng, T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}