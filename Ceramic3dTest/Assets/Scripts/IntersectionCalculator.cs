using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionCalculator : MonoBehaviour
{
 //   public static bool GetSegmentIntersectionCoordinate(Vector2 segmentAStart, Vector2 segmentAEnd, Vector2 segmentBStart, Vector2 segmentBEnd, Point intersectionPoint)
	//{
	//	if (segmentAStart.x >= segmentAEnd.x)
	//	{
	//		Vector2 temp = segmentAStart;
	//		segmentAStart = segmentAEnd;
	//		segmentAEnd = temp;
	//	}
	//	if (segmentBStart.x >= segmentBEnd.x)
	//	{
	//		Vector2 temp = segmentBStart;
	//		segmentBStart = segmentBEnd;
	//		segmentBEnd = temp;
	//	}

	//	float k1;
	//	if (segmentAStart.y == segmentAEnd.y)
	//	{
	//		k1 = 0f;
	//	}
	//	else
	//	{
	//		k1 = (segmentAEnd.y - segmentAStart.y) / (segmentAEnd.x - segmentAStart.x);
	//	}
	//	float k2;
	//	if (segmentBStart.y == segmentBEnd.y)
	//	{
	//		k2 = 0f;
	//	}
	//	else
	//	{
	//		k2 = (segmentBEnd.y - segmentBStart.y) / (segmentBEnd.x - segmentBStart.x);
	//	}
		
	//	if (k1 == k2)
	//	{
	//		return false;
	//	}
	//}

	

	/// <summary>
	/// Test whether two line segments intersect. If so, calculate the intersection point.
	/// <see cref="http://stackoverflow.com/a/14143738/292237"/>
	/// </summary>
	/// <param name="p">Vector to the start point of p.</param>
	/// <param name="p2">Vector to the end point of p.</param>
	/// <param name="q">Vector to the start point of q.</param>
	/// <param name="q2">Vector to the end point of q.</param>
	/// <param name="intersection">The point of intersection, if any.</param>
	/// <param name="considerOverlapAsIntersect">Do we consider overlapping lines as intersecting?
	/// </param>
	/// <returns>True if an intersection point was found.</returns>
	public static bool LineSegementsIntersect(Vector p, Vector p2, Vector q, Vector q2,
		out Vector intersection, bool considerCollinearOverlapAsIntersect = false)
	{
		intersection = new Vector();

		var r = p2 - p;
		var s = q2 - q;
		var rxs = r.Cross(s);
		var qpxr = (q - p).Cross(r);

		// If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
		if (rxs.IsZero() && qpxr.IsZero())
		{
			// 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
			// then the two lines are overlapping,
			if (considerCollinearOverlapAsIntersect)
				if ((0 <= (q - p) * r && (q - p) * r <= r * r) || (0 <= (p - q) * s && (p - q) * s <= s * s))
					return true;

			// 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
			// then the two lines are collinear but disjoint.
			// No need to implement this expression, as it follows from the expression above.
			return false;
		}

		// 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
		if (rxs.IsZero() && !qpxr.IsZero())
			return false;

		// t = (q - p) x s / (r x s)
		var t = (q - p).Cross(s) / rxs;

		// u = (q - p) x r / (r x s)

		var u = (q - p).Cross(r) / rxs;

		// 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
		// the two line segments meet at the point p + t r = q + u s.
		if (!rxs.IsZero() && (0 <= t && t <= 1) && (0 <= u && u <= 1))
		{
			// We can calculate the intersection point using either t or u.
			intersection = p + t * r;

			// An intersection was found.
			return true;
		}

		// 5. Otherwise, the two line segments are not parallel but do not intersect.
		return false;
	}
}