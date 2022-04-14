using System.Collections;
using System.Collections.Generic;
using System;


public static class ListShuffler
{
    private static Random _ran = new Random();

    public static void Shuffle<T>(this List<T> list)
    {
        int to = list.Count;
        while(to > 1)
        {
            int from = _ran.Next(--to + 1);
            T tmp = list[from];
            list[from] = list[to];
            list[to] = tmp;
        }
    }

}
