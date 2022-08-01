using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2[] VerticesWorldPosition { get; private set; }

    public Vector2[] DebugVertices;

    public static float CalculateAreaByVertices(List<Vector2> vertices)
    {
        float area = 0f;
        for (int i = 0; i < vertices.Count; i++)
        {
            int previousVerticeIndex = i - 1;
            previousVerticeIndex = previousVerticeIndex < 0 ? vertices.Count - Math.Abs(previousVerticeIndex % vertices.Count) : previousVerticeIndex % vertices.Count;
            int nextVerticeIndex = i + 1;
            nextVerticeIndex = nextVerticeIndex < 0 ? vertices.Count - Math.Abs(nextVerticeIndex % vertices.Count) : nextVerticeIndex % vertices.Count;

            area += vertices[i].x * (vertices[nextVerticeIndex].y - vertices[previousVerticeIndex].y);
        }
        area /= 2;
        return (area);
    }

    public void CalculateTiledVerticesWorldPosition()
	{
        var localToWorld = transform.localToWorldMatrix;
        MeshFilter mf = GetComponent<MeshFilter>();
        VerticesWorldPosition = new Vector2[mf.mesh.vertices.Length];

        string vertices = "";
        for (int i = 0; i < mf.mesh.vertices.Length; ++i)
        {

            Vector3 world_v = localToWorld.MultiplyPoint3x4(mf.mesh.vertices[i]);
            VerticesWorldPosition[i] = world_v;
            vertices += world_v.ToString() + " ";
        }
        ChangeVerticesOrder();
        DebugVertices = VerticesWorldPosition;
        //Debug.Log(vertices);
    }

    public float CalculateArea()
	{
		float area = 0f;
		for (int i = 0; i < VerticesWorldPosition.Length; i++)
		{
            int previousVerticeIndex = i - 1;
            previousVerticeIndex = previousVerticeIndex < 0 ? VerticesWorldPosition.GetLength(0) - Math.Abs(previousVerticeIndex % VerticesWorldPosition.GetLength(0)) : previousVerticeIndex % VerticesWorldPosition.GetLength(0);
            int nextVerticeIndex = i + 1;
            nextVerticeIndex = nextVerticeIndex < 0 ? VerticesWorldPosition.GetLength(0) - Math.Abs(nextVerticeIndex % VerticesWorldPosition.GetLength(0)) : nextVerticeIndex % VerticesWorldPosition.GetLength(0);

            area += VerticesWorldPosition[i].x * (VerticesWorldPosition[nextVerticeIndex].y - VerticesWorldPosition[previousVerticeIndex].y);
        }
        area /= 2;
		return (area);
	}

    public void ChangeVerticesOrder()
	{
        Vector2 temp = VerticesWorldPosition[2];
        VerticesWorldPosition[2] = VerticesWorldPosition[3];
        VerticesWorldPosition[3] = temp;
    }
}
