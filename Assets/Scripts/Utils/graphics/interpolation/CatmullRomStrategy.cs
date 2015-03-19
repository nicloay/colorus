using System;
using UnityEngine;
using System.Collections.Generic;

public class CatmullRomStrategy : InterpoalateStrategyInterface {

	float diffX,diffY,maxD;
	public void interpolate (List<IntVector2> points, List<IntVector2> result) {		
		if (points.Count <= 1){
			result.AddRange(points);
			return;
		}
		if (points.Count < 4) {		
			int lastId = points.Count -1;
			while (points.Count < 4)
				points.Add(points[lastId]);
		}

 
		for (int n = 0; n < points.Count -1; n++) {
			
			if (n == 0) {
				diffX = Math.Abs (points [n].x - points [n + 1].x);
				diffY = Math.Abs (points [n].y - points [n + 1].y);
				maxD = (int)Math.Sqrt (diffX * diffX + diffY * diffY) * 3f;
				for (int i = 0; i <maxD; i++) {
					result.Add (PointOnCurve (points [n],
											points [n],
											points [n + 1],
											points [n + 2], (1f / maxD) * i));
				
				}
				
				
			} else if (n > 0 && n < points.Count - 2) {
		
				diffX = Math.Abs (points [n].x - points [n - 1].x);
				diffY = Math.Abs (points [n].y - points [n - 1].y);
				maxD = (int)Math.Sqrt (diffX * diffX + diffY * diffY) * 3f;
				for (int i = 0; i <maxD; i++) {
					result.Add (PointOnCurve (points [n - 1],
											points [n],
											points [n + 1],
											points [n + 2], (1f / maxD) * i));
				
				}
				
				
			} else if (n == points.Count - 2) {
				diffX = Math.Abs (points [points.Count - 1].x - points [points.Count - 2].x);
				diffY = Math.Abs (points [points.Count - 1].y - points [points.Count - 2].y);
				maxD = (int)Math.Sqrt (diffX * diffX + diffY * diffY) * 3f;
				for (int i = 0; i < maxD; i++) {
					result.Add (PointOnCurve (points [n - 1],
											points [n],
											points [n + 1],
											points [n + 1], (1f / maxD) * i));
				
				}
				
			}
			
			
			
		}
	}

	static float t0,t1,t2,t3;
	public static IntVector2 PointOnCurve (IntVector2 p0, IntVector2 p1, IntVector2 p2, IntVector2 p3, float t) {
		IntVector2 result = new IntVector2 ();
	 
		t0 = ((-t + 2f) * t - 1f) * t * 0.5f;
		t1 = (((3f * t - 5f) * t) * t + 2f) * 0.5f;
		t2 = ((-3f * t + 4f) * t + 1f) * t * 0.5f;
		t3 = ((t - 1f) * t * t) * 0.5f;
	 
		result.x = (int)(t0 * p0.x + t1 * p1.x + t2 * p2.x + t3 * p3.x);
		result.y = (int)(t0 * p0.y + t1 * p1.y + t2 * p2.y + t3 * p3.y);
		//result.z = p0.z * t0 + p1.z * t1 + p2.z * t2 + p3.z * t3;
	 
		return result;
	}

}


