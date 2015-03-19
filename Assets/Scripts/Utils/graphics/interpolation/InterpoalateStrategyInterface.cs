using System;
using UnityEngine;
using System.Collections.Generic;

public interface InterpoalateStrategyInterface
{
	void interpolate(List<IntVector2> points, List<IntVector2> result);
}


