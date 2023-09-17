using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalloffMapGenerator : MonoBehaviour
{
    public static float[,] GenerateFalloffMap(int xSize, int zSize)
    {
        float[,] map = new float[xSize, zSize];

        for (int i = 0; i < xSize; i++)
        {
            for (int j = 0; j < zSize; j++)
            {
                float x = i / (float)xSize * 2 - 1;
                float z = j / (float)zSize * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(z));
                map[i, j] = value * 5;//Evaluate(value);
            }
        }
        return map;
    }

    static float Evaluate(float value)
    {
        float a = 3;
        float b = 2.2f;

        return Mathf.Pow(value, a) / Mathf.Pow(value, a) + Mathf.Pow((b - b * value), a);
    }
}
