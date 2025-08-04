using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CYE_Utils
{
    public static T[] ShuffleArray<T>(T[] arraySet)// where T : Type
    {
        for (int cnt = 0; cnt < arraySet.Length; cnt++)
        {
            int changeIndex = new System.Random().Next(0, cnt + 1);
            (arraySet[changeIndex], arraySet[cnt]) = (arraySet[cnt], arraySet[changeIndex]);
        }
        return arraySet;
    }
}
