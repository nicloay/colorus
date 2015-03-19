using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ToolPanelConfig{
	public int left;
	public int top;
	public int buttonSize = 37;
	public int totalWidth = 86;
	public string styleName = "toolButton";
	public GUIStyle style;
	
	public Texture2D incIcon;
	public Texture2D brushIcon;	
	public Texture2D bigBrushIcon;
	public Texture2D rollerIcon;
	public Texture2D bucketIcon;
	public Texture2D stampIcon;
}


[Serializable]
public class ToolPanel : GUIPart {
	public ToolPanelConfig config;
	Rect incRect;
	Rect brushRect;
	Rect bigBrushRect;
	Rect rollerRect;
	Rect bucketRect;
	Rect stampRect;

	
	
	public void setTopLeftPosition(int left, int top, GUISkin skin){		
		setStyle(skin, config.styleName, out config.style);
		
		config.left = left;
		config.top = top;
		recalculatePositions();
	}
		
	ToolType clickedTool;
	public override void OnGUI (){
		clickedTool = ToolType.NONE;
		if (GUI.Button(incRect, config.incIcon, config.style))
			clickedTool = ToolType.INK;
		if (GUI.Button(brushRect, config.brushIcon, config.style))
			clickedTool = ToolType.BRUSH;
		if (GUI.Button(bigBrushRect, config.bigBrushIcon, config.style))
			clickedTool = ToolType.BRUSH_LARGE;
		if (GUI.Button(rollerRect, config.rollerIcon, config.style))
			clickedTool = ToolType.ROLLER;
		if (GUI.Button(bucketRect, config.bucketIcon, config.style))
			clickedTool = ToolType.BUCKET;
		if (GUI.Button(stampRect, config.stampIcon, config.style))
			clickedTool = ToolType.STAMP;
		
		if (clickedTool != ToolType.NONE && WorkspaceEventManager.instance.onToolButtonClick !=null)
			WorkspaceEventManager.instance.onToolButtonClick(clickedTool);
	}	
	
	void recalculatePositions(){
		int rightColumnLeft  = config.left + (config.totalWidth-config.buttonSize);
		int verticalStep = (config.totalWidth - config.buttonSize);
		int y = config.top;
		int x = config.left;
		incRect      = new Rect(x, y, config.buttonSize, config.buttonSize);
		y += verticalStep;
		brushRect    = new Rect(x, y, config.buttonSize, config.buttonSize);
		y += verticalStep;
		bucketRect = new Rect(x, y, config.buttonSize, config.buttonSize);
		y = config.top;
		x = rightColumnLeft;
		rollerRect   = new Rect(x, y, config.buttonSize, config.buttonSize);
		y += verticalStep;
		bigBrushRect = new Rect(x, y, config.buttonSize, config.buttonSize);
		y += verticalStep;
		stampRect    = new Rect(x, y, config.buttonSize, config.buttonSize);		
	}
	
	
	public void setPosition (Rect rect)
	{
		
	}
	
	
}
