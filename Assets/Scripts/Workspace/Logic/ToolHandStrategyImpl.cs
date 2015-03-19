using UnityEngine;
using System.Collections;

public class ToolHandStrategyImpl : ToolLogicStrategy {
	CanvasController canvas;


	public ToolHandStrategyImpl(){
		canvas = PropertiesSingleton.instance.canvasWorkspaceController.canvas;
	}

#region ToolLogicStrategy implementation

#if !UNITY_IPHONE
	public void onMouseDown (IntVector2 position) {
		startDrag();
	}

	public void onMouseOverWithButton (IntVector2 position, Vector3 globalPosition) {
		doDrag();
	}

	public void onMouseOverWithButtonDone (IntVector2 position) {
	}

	public void onMouseEnter () {	
	}

	public void onMouseExit () {	
	}

	public void onMouseOver (IntVector2 pixelCursorPosition, Vector3 globalCursorPosition) {	
	}
#else
	public void onTouchStart (IntVector2 pixelPosition, Vector3 globalPosition)
	{
		startDrag();
	}

	public void onTouchOver (IntVector2 pixelPosition, Vector3 globalPosition)
	{
		doDrag();
	}

	public void onTouchEnd ()
	{
	}
#endif
	Vector3 startWorldPoint;
	void startDrag(){
		startWorldPoint = canvas.canvasCamera.screenPointToWorld(Input.mousePosition);
        }

	void doDrag(){
		canvas.canvasCamera.syncScreenPointWithWorld(Input.mousePosition, startWorldPoint);
	}

	#endregion
}
