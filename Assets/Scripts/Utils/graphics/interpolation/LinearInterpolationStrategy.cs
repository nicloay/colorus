using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LinearInterpolationStrategy : InterpoalateStrategyInterface {
	#region InterpoalateStrategyInterface implementation
	IntVector2 leftPoint,rightPoint;
	int lengthX, lengthY, max,x,y, i, prevI;

	public void interpolate (List<IntVector2> points, List<IntVector2>  result) {
		if (result == null)
			Debug.LogError("you passed null list here");
		if (points.Count < 3 ) 
			result.AddRange(points);

		prevI = 0;
		
		for (i=1;i<points.Count;prevI=i++){
			leftPoint  = points[prevI];
			rightPoint = points[i];
			if (leftPoint.equalsTo( rightPoint))
				continue;
			lengthX = rightPoint.x - leftPoint.x;
			lengthY = rightPoint.y - leftPoint.y;
			max = Mathf.Max (Mathf.Abs(lengthX), Mathf.Abs(lengthY));			
			for (int j = 0; j <= max; j++) {
				x = leftPoint.x + (lengthX * j / max);
				y = leftPoint.y + (lengthY * j / max);
				result.Add(new IntVector2(x,y));
			}			
		}			
	}
	#endregion
	
}


