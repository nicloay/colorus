using UnityEngine;


/*
 * http://www.gizma.com/easing/#circ1
 */

public delegate float InterpolationFunction(float currentTime, float startPosition, float duration, float routeLength);

public class InterpolationUtil {
	public static float moveInLinear (float currentTime, float startPosition, float duration, float routeLength){		
		float timeStep = currentTime / duration;
		return routeLength * timeStep + startPosition;
	}


	public static float moveInQuad (float currentTime, float startPosition, float duration, float routeLength){		
		float timeStep = currentTime / duration;
		return routeLength*timeStep*timeStep*timeStep*timeStep + startPosition;
	}

	public static float moveOutQuad (float currentTime, float startValue, float destValue, float duration) {
		currentTime /= duration;
		return -destValue * currentTime * (currentTime - 2) + startValue;
	}

	public static float moveInCirc (float currentTime, float startPosition, float duration, float routeLength){		
		float timeStep = currentTime / duration;
		return -routeLength *(Mathf.Sqrt(1 - timeStep * timeStep)-1) +startPosition;
	}
	
	public static float moveOutCirc (float currentTime, float startPosition, float duration, float routeLength){		
		float timeStep = currentTime / duration;
		return routeLength *(Mathf.Sqrt(1 - timeStep * timeStep)) +startPosition;
	}
	
	public static float moveInQuint (float currentTime, float startPosition, float duration, float routeLength){		
		float timeStep = currentTime / duration;
		return routeLength * timeStep * timeStep * timeStep * timeStep * timeStep +startPosition;
	}
	
	public static float moveOutQuint (float currentTime, float startPosition, float duration, float routeLength){		
		float timeStep = currentTime / duration;
		timeStep--;
		return routeLength * (timeStep * timeStep * timeStep * timeStep * timeStep +1 ) + startPosition;
	}	
}
