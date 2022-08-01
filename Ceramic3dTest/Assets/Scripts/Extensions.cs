using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    private const float Epsilon = 1e-10f;

    public static bool IsZero(this float d)
    {
        return Math.Abs(d) < Epsilon;
    }
}