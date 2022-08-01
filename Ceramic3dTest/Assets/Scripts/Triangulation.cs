using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangulation : MonoBehaviour
{
	//public static List<Triangle> TriangulateConvexPolygon(List<Vertex> convexHullpoints)
	//{
	//	List<Triangle> triangles = new List<Triangle>();

	//	for (int i = 2; i < convexHullpoints.Count; i++)
	//	{
	//		Vertex a = convexHullpoints[0];
	//		Vertex b = convexHullpoints[i - 1];
	//		Vertex c = convexHullpoints[i];

	//		triangles.Add(new Triangle(a, b, c));
	//	}

	//	return triangles;
	//}

	public static int[] TriangulateConvexPoligonToIndexes(List<Vector2> convexHullpoints)
	{
		int[] triangles;
		List<int> indexes = new List<int>();
		for (int i = 2; i < convexHullpoints.Count; i++)
		{
			indexes.Add(0);
			indexes.Add(i - 1);
			indexes.Add(i);
		}
		triangles = indexes.ToArray();
		return triangles;
	}
}
