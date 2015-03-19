using UnityEngine;
using System.Collections;

public static class RectExtension {
	public static bool LocalContains(this Rect rect, Vector2 position){
		return (position.x >= 0 && 
			position.y >= 0 &&
			position.x < rect.width &&
			position.y < rect.height);
	}
}
