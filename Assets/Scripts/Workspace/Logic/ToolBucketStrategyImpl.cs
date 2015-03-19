using UnityEngine;
using System.Collections;


public class ToolBucketStrategyImpl : ToolLogicStrategy {

	CanvasController canvas;
	CanvasConfig canvasConfig;
	PropertiesSingleton props;

	public ToolBucketStrategyImpl(){
		canvas = PropertiesSingleton.instance.canvasWorkspaceController.canvas;
		canvasConfig = canvas.config;
		props = PropertiesSingleton.instance;
	}


	#region ToolLogicStrategy implementation

#if !UNITY_IPHONE

	public void onMouseDown (IntVector2 position)
	{
	}

	public void onMouseOverWithButton (IntVector2 position, Vector3 globalPosition)
	{
	}

	public void onMouseOverWithButtonDone (IntVector2 position)
	{
		doFloodFill(position);
	}

	public void onMouseEnter ()
	{
	}

	public void onMouseExit ()
	{	
	}

	public void onMouseOver (IntVector2 pixelCursorPosition, Vector3 globalCursorPosition)
	{
	}

#else
	IntVector2 point;
	public void onTouchStart (IntVector2 pixelPosition, Vector3 globalPosition)
	{
		point = pixelPosition;
	}

	public void onTouchOver (IntVector2 pixelPosition, Vector3 globalPosition)
	{
		point = pixelPosition;
	}

	public void onTouchEnd ()
	{
		Handheld.StartActivityIndicator();
		doFloodFill (point);
		Handheld.StopActivityIndicator();
	}
#endif

	void doFloodFill(IntVector2 position){
		if (canvas.persistentLayer[(int)(position.y * canvas.size.x + position.x)]){
			if (WorkspaceEventManager.instance.onWrongAction!=null)
				WorkspaceEventManager.instance.onWrongAction("persistentborder click");
			return;
		}
		
		Color32[] colors= canvas.fetchColors();
		if (canvas.persistentLayer[(int)(position.y * canvas.size.x + position.x)])
			return;
		bool[,] resultRegion = TextureUtil.floodFillLineGetRegion (position, canvas.actualColors, canvas.persistentLayer, canvasConfig.canvasSize.x, canvasConfig.canvasSize.y);
		Color32 activeColor = props.colorProperties.activeColor;
		int tCounter = 0;
		for (int yy = 0; yy < canvasConfig.canvasSize.y; yy++) {
			for (int xx = 0; xx < canvasConfig.canvasSize.x; xx++) {
				if (resultRegion[xx,yy]){
					colors[tCounter].r = activeColor.r;
					colors[tCounter].g = activeColor.g;
					colors[tCounter].b = activeColor.b;
					colors[tCounter].a = 255;
				} 
				tCounter++;
			}
		}
		canvas.applyColors(colors,true,true,position);
	}
	#endregion
}
