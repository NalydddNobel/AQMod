﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Utilities;

namespace Aequus;

public static class Helper {
    public static Point WorldClamp(this Point value, int fluff = 0) {
        return new(Math.Clamp(value.X, fluff, Main.maxTilesX - fluff), Math.Clamp(value.Y, fluff, Main.maxTilesX - fluff));
    }

    public static float Oscillate(float time, float magnitude) {
        return Oscillate(time, magnitude, magnitude);
    }

    public static float Oscillate(float time, float minimum, float maximum) {
        return (float)(minimum + (Math.Sin(time) + 1f) / 2f * (maximum - minimum));
    }

    public static Rectangle Frame(this Rectangle rectangle, int frameX, int frameY, int sizeOffsetX = 0, int sizeOffsetY = 0) {
        return new Rectangle(rectangle.X + (rectangle.Width - sizeOffsetX) * frameX, rectangle.Y + (rectangle.Width - sizeOffsetY) * frameY, rectangle.Width, rectangle.Height);
    }

    #region Type
    public static string NamespacePath(this object obj) {
        return NamespacePath(obj.GetType());
    }
    public static string NamespacePath<T>() {
        return NamespacePath(typeof(T));
    }
    public static string NamespacePath(Type t) {
        return t.Namespace.Replace('.', '/');
    }
    public static string GetPath(this object obj) {
        return GetPath(obj.GetType());
    }
    public static string GetPath<T>() {
        return GetPath(typeof(T));
    }
    public static string GetPath(Type t) {
        return $"{NamespacePath(t)}/{t.Name}";
    }
    #endregion

    #region RNG
    public static float NextFloat(this ref FastRandom random, float min, float max) {
        return min + random.NextFloat() * (max - min);
    }
    public static float NextFloat(this ref FastRandom random, float max) {
        return random.NextFloat() * max;
    }
    public static ulong TileSeed(int i, int j) {
        ulong x = (ulong)i;
        ulong y = (ulong)j;
        return x * x + y * y * x + x;
    }
    public static ulong TileSeed(Point point) {
        return TileSeed(point.X, point.Y);
    }
    #endregion
}