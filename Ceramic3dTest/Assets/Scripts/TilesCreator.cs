using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TilesCreator : MonoBehaviour
{
    [SerializeField]
    FieldCreator FieldCreatorScript;
    [SerializeField]
    GameObject TilePrefab;
    [SerializeField]
    GameObject PointIntersectionPrefab;
    [SerializeField]
    Material CutedTileMaterial;
    [SerializeField]
    Material CutTileMaskMaterial;
    [SerializeField]
    Text AreaTextField;

    private float tileSizeInM = 0.1f;
    private float tileSeamInM = 0.001f;
    private float tileOffset = 0.025f;
    
    private float area = 0f;

    private int tilesReserve = 5;

    public void SetSeamSize(float seamSize)
	{
        tileSeamInM = seamSize;
	}

    public void SetOffset(float offset)
	{
        tileOffset = offset;
    }

    public void CreateTiles()
	{
        area = 0f;
        float tileAndSeamSize = tileSizeInM + tileSeamInM;
        int horizontalTilesCount = Mathf.CeilToInt(FieldCreatorScript.FieldWidthInM / tileAndSeamSize);
        int verticalTilesCount = Mathf.CeilToInt(FieldCreatorScript.FieldHeightInM / tileAndSeamSize);
        int maximumTilesOnSideCount = (horizontalTilesCount > verticalTilesCount ? horizontalTilesCount : verticalTilesCount);
        tilesReserve = (maximumTilesOnSideCount / 3);

        List<Tile> tiles = new List<Tile>();

        for (int i = -(int)((maximumTilesOnSideCount - FieldCreatorScript.FieldHeightInM) / 2); i < maximumTilesOnSideCount + tilesReserve; i++)
		{
            float tileOffsetInRow = (tileOffset * i) % tileAndSeamSize;
            for (int j = -(int)((maximumTilesOnSideCount - FieldCreatorScript.FieldWidthInM) / 2); j < maximumTilesOnSideCount + tilesReserve; j++)
			{
                GameObject tile = Instantiate(TilePrefab, new Vector3(tileAndSeamSize * j + tileOffsetInRow, tileAndSeamSize * i, 0f), Quaternion.identity);

                Tile tileScript = tile.AddComponent<Tile>();
                tileScript.CalculateTiledVerticesWorldPosition();
                if (IsTileInsideField(tileScript))
                {
                    tiles.Add(tileScript);
                    tile.transform.SetParent(FieldCreatorScript.TilesHolder);
                    if (IsTilePartiallyOutsideField(tileScript))
                    {
                        List<Vector2> newTileVertices = CutTile(tileScript);
                        area += Tile.CalculateAreaByVertices(newTileVertices);
                    }
                    else
					{
                        area += tileScript.CalculateArea();
					}
                }
                else
				{
                    Destroy(tile);
				}
            }
		}

        AreaTextField.text = Math.Round(area, 3).ToString();
	}

	public List<Vector2> CutTile(Tile tileScript)
	{
		List<Vector2> tileNewVertices = new List<Vector2>();

		for (int i = 0; i < tileScript.VerticesWorldPosition.GetLength(0); i++)
		{
			if (IsPointInsideField(tileScript.VerticesWorldPosition[i]))
			{
                tileNewVertices.Add(tileScript.VerticesWorldPosition[i]);
			}
			else
			{
				int previousVerticeIndex = i - 1;
				previousVerticeIndex = previousVerticeIndex < 0 ? tileScript.VerticesWorldPosition.GetLength(0) - Math.Abs(previousVerticeIndex % tileScript.VerticesWorldPosition.GetLength(0)) : previousVerticeIndex % tileScript.VerticesWorldPosition.GetLength(0);
                int nextVerticeIndex = i + 1;
                nextVerticeIndex = nextVerticeIndex < 0 ? tileScript.VerticesWorldPosition.GetLength(0) - Math.Abs(nextVerticeIndex % tileScript.VerticesWorldPosition.GetLength(0)) : nextVerticeIndex % tileScript.VerticesWorldPosition.GetLength(0);

                List<Vector2> intersections =  CalculateIntersections(new Vector(tileScript.VerticesWorldPosition[i].x, tileScript.VerticesWorldPosition[i].y),
                                        new Vector(tileScript.VerticesWorldPosition[previousVerticeIndex].x, tileScript.VerticesWorldPosition[previousVerticeIndex].y),
                                        new Vector(tileScript.VerticesWorldPosition[nextVerticeIndex].x, tileScript.VerticesWorldPosition[nextVerticeIndex].y));

                foreach (var intersection in intersections)
				{
                    tileNewVertices.Add(intersection);
				}
            }
        }

        //  foreach (var vertice in tileNewVertices)
        //{
        //      Instantiate(PointIntersectionPrefab, new Vector3(vertice.x, vertice.y, 0f), Quaternion.identity);
        //}

        AddFieldCornersIfIncludedInTile(tileNewVertices, tileScript.VerticesWorldPosition.ToList());

        tileScript.GetComponent<MeshRenderer>().material = CutedTileMaterial;

		CreateTileMask(tileScript, tileNewVertices);

        return (tileNewVertices);
	}

    void CreateTileMask(Tile tileScript, List<Vector2> tileVertices)
    {
        tileVertices = ConvexHull.ComputeConvexHull(tileVertices);
        int[] triangles = Triangulation.TriangulateConvexPoligonToIndexes(tileVertices);
        Vector3[] points = new Vector3[tileVertices.Count];
        for (int i = 0; i < tileVertices.Count; i++)
        {
            points[i] = new Vector3(tileVertices[i].x, tileVertices[i].y, 0f);
        }

        var mesh = new Mesh();
        mesh.vertices = points;
        mesh.triangles = triangles;

        var tileMask = new GameObject("TileMask");
        tileMask.AddComponent<MeshFilter>().sharedMesh = mesh;
        tileMask.AddComponent<MeshRenderer>().material = CutTileMaskMaterial;
        tileMask.transform.SetParent(tileScript.transform);

    }

    public List<Vector2> CalculateIntersections(Vector checkingPoint, Vector previousPoint, Vector nextPoint)
	{
        Vector2[] fieldVertices = FieldCreatorScript.FieldScript.VerticesWorldPosition;
        List<Vector2> intersections = new List<Vector2>();
        for (int i = 0; i < fieldVertices.GetLength(0); i++)
		{
            Vector fieldSideStart = new Vector(fieldVertices[i].x, fieldVertices[i].y);
            Vector fieldSideEnd = new Vector(fieldVertices[(i + 1) % 4].x, fieldVertices[(i + 1) % 4].y);

            Vector intersectionPoint = new Vector();
            if (IntersectionCalculator.LineSegementsIntersect(checkingPoint, previousPoint, fieldSideStart, fieldSideEnd, out intersectionPoint))
			{
                intersections.Add(new Vector2(intersectionPoint.X, intersectionPoint.Y));
			}
            if (IntersectionCalculator.LineSegementsIntersect(checkingPoint, nextPoint, fieldSideStart, fieldSideEnd, out intersectionPoint))
            {
                intersections.Add(new Vector2(intersectionPoint.X, intersectionPoint.Y));
            }
        }
        return (intersections);
	}

    public void AddFieldCornersIfIncludedInTile(List<Vector2> listAddTo, List<Vector2> tileVertices)
	{
        Field field = FieldCreatorScript.FieldScript;
        foreach (var fieldCorner in field.VerticesWorldPosition)
		{
            if (IsPointInsideField(fieldCorner, tileVertices))
			{
                listAddTo.Add(fieldCorner);
			}
		}
	}

    public bool IsTileInsideField(Tile tileScript)
	{
        foreach (var tileVertice in tileScript.VerticesWorldPosition)
		{
            if (IsPointInsideField(tileVertice))
			{
                return (true);
			}
		}
        return (false);
	}

    public bool IsTilePartiallyOutsideField(Tile tileScript)
	{
        foreach (var tileVertice in tileScript.VerticesWorldPosition)
        {
            if (!IsPointInsideField(tileVertice))
            {
                return (true);
            }
        }
        return (false);
    }

    //Реализация - считаются произведения(1,2,3 - вершины треугольника, 0 - точка):
    //(x1-x0)*(y2-y1)-(x2-x1)*(y1-y0)
    //(x2-x0)*(y3-y2)-(x3-x2)*(y2-y0)
    //(x3-x0)*(y1-y3)-(x1-x3)*(y3-y0)
    //Если они одинакового знака, то точка внутри треугольника, если что-то из этого - ноль, то точка лежит на стороне, иначе точка вне треугольника.

    public bool IsPointInsideField(Vector2 point)
	{
        Field field = FieldCreatorScript.FieldScript;
        for (int i = 0; i < 4; i++)
		{
            Vector2 vertA = field.VerticesWorldPosition[i % 4];
            Vector2 vertB = field.VerticesWorldPosition[(i + 1) % 4];
            Vector2 vertC = field.VerticesWorldPosition[(i + 2) % 4];
            if (IsHaveSameSign(point, vertA, vertB, vertC))
			{
                return (true);
			}
		}
        return (false);
	}

    public bool IsPointInsideField(Vector2 point, List<Vector2> fieldVertices)
    {
        Field field = FieldCreatorScript.FieldScript;
        for (int i = 0; i < 4; i++)
        {
            Vector2 vertA = fieldVertices[i % 4];
            Vector2 vertB = fieldVertices[(i + 1) % 4];
            Vector2 vertC = fieldVertices[(i + 2) % 4];
            if (IsHaveSameSign(point, vertA, vertB, vertC))
            {
                return (true);
            }
        }
        return (false);
    }

    public bool IsHaveSameSign(Vector2 point, Vector2 vertA, Vector2 vertB, Vector2 vertC)
	{
        if (
            (((vertA.x - point.x) * (vertB.y - vertA.y) - (vertB.x - vertA.x) * (vertA.y - point.y) <= 0)
            & ((vertB.x - point.x) * (vertC.y - vertB.y) - (vertC.x - vertB.x) * (vertB.y - point.y) <= 0)
            & ((vertC.x - point.x) * (vertA.y - vertC.y) - (vertA.x - vertC.x) * (vertC.y - point.y) <= 0))
            ||
            (((vertA.x - point.x) * (vertB.y - vertA.y) - (vertB.x - vertA.x) * (vertA.y - point.y) >= 0)
            & ((vertB.x - point.x) * (vertC.y - vertB.y) - (vertC.x - vertB.x) * (vertB.y - point.y) >= 0)
            & ((vertC.x - point.x) * (vertA.y - vertC.y) - (vertA.x - vertC.x) * (vertC.y - point.y) >= 0))
            )
        {
            return (true);
		}
        else
		{
            return (false);
		}
	}
}
