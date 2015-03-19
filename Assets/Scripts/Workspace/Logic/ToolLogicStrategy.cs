using UnityEngine;
using System.Collections;

public interface ToolLogicStrategy  {

#if !UNITY_IPHONE
	void onMouseDown(IntVector2 position);
	void onMouseOverWithButton(IntVector2 position, Vector3 globalPosition);
	void onMouseOverWithButtonDone(IntVector2 position);
	void onMouseEnter();
	void onMouseExit();
	void onMouseOver(IntVector2 pixelCursorPosition, Vector3 globalCursorPosition);
#else
	void onTouchStart(IntVector2 pixelPosition, Vector3 globalPosition);
	void onTouchOver(IntVector2 pixelPosition, Vector3 globalPosition);
	void onTouchEnd();
#endif
}
