using System;
using UnityEngine;


public class ToolPipetteStrategyImpl:ToolLogicStrategy {
	#region ToolLogicStrategy implementation

#if !UNITY_IPHONE
	public void onMouseDown (IntVector2 position) {		

	}

	public void onMouseOverWithButton (IntVector2 position, Vector3 globalPosition) {		
	}

	public void onMouseOverWithButtonDone (IntVector2 position) {		
		takeColor(position);
	}

	public void onMouseEnter () {
	}

	public void onMouseExit () {
	}

	public void onMouseOver (IntVector2 pixelCursorPosition, UnityEngine.Vector3 globalCursorPosition) {
	}

#else
	IntVector2 lastPosition;
	public void onTouchStart (IntVector2 pixelPosition, Vector3 globalPosition)
	{
		lastPosition = pixelPosition;
	}

	public void onTouchOver (IntVector2 pixelPosition, Vector3 globalPosition)
	{
		lastPosition = pixelPosition;
	}

	public void onTouchEnd ()
	{
		takeColor(lastPosition);
	}
#endif
	void takeColor(IntVector2 position){
		Color32 color =  PropertiesSingleton.instance.canvasWorkspaceController.canvas.actualColors[(position.y)*(PropertiesSingleton.instance.width)  + position.x];
		if (WorkspaceEventManager.instance.onPipetteSelectedColor!=null)
			WorkspaceEventManager.instance.onPipetteSelectedColor(color);
	}

	#endregion
	
}


