using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils
{
    //static int maxHeight = 120;
    //static float smooth = 0.01f;
    //static int octaves = 4;
    //static float persistance = 0.5f;
    static float offset = UnityEngine.Random.Range(12000f, 32000f);

    public static int GenerateStoneHeight(float x, float y, int maxHeight, float smooth, int octaves, float persistance)
    {
        float height = Map(0, maxHeight - 20, 0, 1, fBM(x * smooth * 1.1f, y * smooth * 1.1f, octaves + 1, persistance));
        return (int)height;
    }

    public static int GenerateHeight(float x, float y, int maxHeight, float smooth, int octaves, float persistance)
    {
        float height = Map(0, maxHeight, 0, 1, fBM(x * smooth, y * smooth, octaves, persistance));
        return (int)height;
    }

    public static float fBM3D(float x, float y, float z, float sm, int oct)
    {
        float XY = fBM(x * sm, y * sm, oct, 0.5f);
        float YZ = fBM(y * sm, z * sm, oct, 0.5f);
        float XZ = fBM(x * sm, z * sm, oct, 0.5f);

        float YX = fBM(y * sm, z * sm, oct, 0.5f);
        float ZY = fBM(z * sm, y * sm, oct, 0.5f);
        float ZX = fBM(z * sm, x * sm, oct, 0.5f);

        return (XY + YZ + XZ + YX + ZY + ZX) / 6.0f;
    }

    static float Map(float nMin, float nMax, float oMin, float oMax, float value)
    {
        return Mathf.Lerp(nMin, nMax, Mathf.InverseLerp(oMin, oMax, value));
    }

    private static float fBM(float x, float z, int oct, float pers)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;

        for (int i = 0; i < oct; i++)
        {
            total += Mathf.PerlinNoise((x + offset) * frequency, (z + offset) * frequency) * amplitude;

            maxValue += amplitude;

            amplitude *= pers;
            frequency *= 2;
        }

        return total / maxValue;
    }
}
