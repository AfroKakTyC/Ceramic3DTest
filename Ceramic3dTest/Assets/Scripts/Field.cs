using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public Vector2[] VerticesWorldPosition { get; private set; }

    public void CalculateFieldVerticesWorldPosition()
	{
        VerticesWorldPosition = new Vector2[4];
        var localToWorld = transform.localToWorldMatrix;
        MeshFilter mf = GetComponent<MeshFilter>();
        string vertices = "";
        for (int i = 0; i < 4; ++i)
        {
            Vector3 world_v = localToWorld.MultiplyPoint3x4(mf.mesh.vertices[i]);
            VerticesWorldPosition[i] = world_v;
            vertices += world_v.ToString() + " ";
        }
        ChangeVerticesOrder();
        //Debug.Log(vertices);
    }

    public void ChangeVerticesOrder()
    {
        Vector2 temp = VerticesWorldPosition[2];
        VerticesWorldPosition[2] = VerticesWorldPosition[3];
        VerticesWorldPosition[3] = temp;
    }

    public Vector2[] GetEdgeCoordinates()
	{
        Vector2 downLeftCoordinate = new Vector2(Mathf.Infinity, Mathf.Infinity);
        Vector2 upRightCoordinate = new Vector2(Mathf.NegativeInfinity, Mathf.NegativeInfinity);
        foreach (var corner in VerticesWorldPosition)
		{
            if (corner.x < downLeftCoordinate.x)
			{
                downLeftCoordinate.x = corner.x;
			}
            if (corner.y < downLeftCoordinate.y)
			{
                downLeftCoordinate.y = corner.y;
			}
            if (corner.x > upRightCoordinate.x)
			{
                upRightCoordinate.x = corner.x;
			}
            if (corner.y > upRightCoordinate.y)
			{
                upRightCoordinate.y = corner.y;
			}
		}
        return (new Vector2[] { downLeftCoordinate, upRightCoordinate });
	}
}
