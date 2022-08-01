using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector
{
	public float X;
	public float Y;

	// Constructors.
	public Vector(float x, float y) { X = x; Y = y; }
	public Vector() : this(float.NaN, float.NaN) { }

	public static Vector operator -(Vector v, Vector w)
	{
		return new Vector(v.X - w.X, v.Y - w.Y);
	}

	public static Vector operator +(Vector v, Vector w)
	{
		return new Vector(v.X + w.X, v.Y + w.Y);
	}

	public static float operator *(Vector v, Vector w)
	{
		return v.X * w.X + v.Y * w.Y;
	}

	public static Vector operator *(Vector v, float mult)
	{
		return new Vector(v.X * mult, v.Y * mult);
	}

	public static Vector operator *(float mult, Vector v)
	{
		return new Vector(v.X * mult, v.Y * mult);
	}

	public float Cross(Vector v)
	{
		return X * v.Y - Y * v.X;
	}

	public override bool Equals(object obj)
	{
		var v = (Vector)obj;
		return (X - v.X).IsZero() && (Y - v.Y).IsZero();
	}
}
