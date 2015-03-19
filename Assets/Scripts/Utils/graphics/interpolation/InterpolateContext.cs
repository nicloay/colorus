using System;
using UnityEngine;
using System.Collections.Generic;

public class InterpolateContext
{
	InterpoalateStrategyInterface interpoalateStrategyInterface;
	public InterpolateContext (InterpoalateStrategyInterface interpoalateStrategyInterface)
	{
		this.interpoalateStrategyInterface=interpoalateStrategyInterface;		
	}
	
	public void interpolate(List<IntVector2> points, List<IntVector2> result){
		interpoalateStrategyInterface.interpolate(points, result);	
	}
}


