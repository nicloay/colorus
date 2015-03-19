using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class AdditionalRightPannelConfig{
	public int left;
	public int top;
	public int totalWidth = 86;
	public int buttonSize = 37;
	public string styleName = "toolButton";
	public GUIStyle style;
	
	public Texture2D zoomInIcon;
	public Texture2D zoomOutIcon;
	public Texture2D handIcon;
	public Texture2D pipetteIcon;	
}


[Serializable]
public class AdditionalRightPannel : GUIPart {
	public AdditionalRightPannelConfig config;
	Rect zoomInRect;
	Rect zoomOutRect;
	Rect handRect;
	Rect pipetteRect;

	CanvasController canvas;
	PropertiesSingleton props;

	public void setTopLeftPosition(int left, int top, GUISkin skin){		
		canvas = PropertiesSingleton.instance.canvasWorkspaceController.canvas;
		props = PropertiesSingleton.instance;
		setStyle(skin, config.styleName, out config.style);
		config.left = left;
		config.top = top;
		recalculatePositions();
	}
	
	bool guiEnabled;
	public override void OnGUI ()
	{
		guiEnabled = GUI.enabled;	
		
		GUI.enabled = guiEnabled && canvas.canvasCamera !=null && canvas.canvasCamera.canZoomIn;
		if (GUI.Button(zoomInRect,  config.zoomInIcon,  config.style) )
			canvas.canvasCamera.zoom(-props.camearZoomStep);
		
		GUI.enabled = guiEnabled && canvas.canvasCamera !=null && canvas.canvasCamera.canZoomOut;
		if (GUI.Button(zoomOutRect, config.zoomOutIcon, config.style))
			canvas.canvasCamera.zoom(props.camearZoomStep);


		if (GUI.Button(handRect,    config.handIcon,    config.style) && WorkspaceEventManager.instance.onToolButtonClick !=null)
			WorkspaceEventManager.instance.onToolButtonClick(ToolType.HAND);
		GUI.enabled = guiEnabled;
		
		if (GUI.Button(pipetteRect, config.pipetteIcon, config.style) && WorkspaceEventManager.instance.onToolButtonClick !=null)
			WorkspaceEventManager.instance.onToolButtonClick(ToolType.PIPETTE);
	}
	
	
	void recalculatePositions(){
		int rightColumnLeft  = config.left + (config.totalWidth-config.buttonSize);
		int downRowTop = config.top + (config.totalWidth-config.buttonSize);
		zoomInRect  = new Rect(config.left    , config.top, config.buttonSize, config.buttonSize);
		zoomOutRect = new Rect(rightColumnLeft, config.top, config.buttonSize, config.buttonSize);
		handRect    = new Rect(config.left    , downRowTop, config.buttonSize, config.buttonSize);
		pipetteRect = new Rect(rightColumnLeft, downRowTop, config.buttonSize, config.buttonSize);
	}
	
	
	public void setPosition (Rect rect)
	{
		
	}
}
