using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class  ScrollBarsConfig{
	
	
}
[Serializable]
public class ScrollBars : GUIPart {
	public ScrollBarsConfig config;
	int left;
	int top;
	int margin;
	int contentHeight;
	int contentWidth;
	
	Rect horizontalScrollRect;
	Rect verticalScrollRect;
	
	int verticalScrollWidth;
	int horizontalScrollHeight;	

	PropertiesSingleton propSingleton;

	CanvasController canvas;

	public void setProperties(int left, int top, int contentWidth, int contentHeight, int margin, GUISkin skin){
		propSingleton = PropertiesSingleton.instance;
		canvas = propSingleton.canvasWorkspaceController.canvas;
		this.left = left;
		this.top = top;
		this.margin = margin;
		this.contentWidth = contentWidth;
		this.contentHeight = contentHeight;
		
		
		horizontalScrollHeight =(int) skin.FindStyle("horizontalscrollbar").fixedHeight;
		
		
		verticalScrollWidth    =(int) skin.FindStyle("verticalscrollbar").fixedWidth;
		recalculatePositions();
	}

	float verticalValue;
	float horizontalValue;
	bool oldMouseOverScrollState = false;
	bool newMouseOverScrollState = false;
	Vector2 screenGUICoordinates;
	#region implemented abstract members of GUIPart
	public override void OnGUI ()
	{

		if (canvas == null || canvas.canvasCamera ==null)
			return;

		screenGUICoordinates = new Vector2(Input.mousePosition.x, (float) Screen.height - Input.mousePosition.y);
		Vector2 camBottLeftPosition = canvas.canvasCamera.cameraGlobalBottomLeftCornerPosition;
		showHorizontalScroll(camBottLeftPosition);
		showVerticalScroll(camBottLeftPosition);
#if !UNITY_IPHONE || UNITY_EDITOR
		newMouseOverScrollState = mouseOverVerticalScroll || mouseOverHorizontalScroll;

		if (newMouseOverScrollState != oldMouseOverScrollState){
			if (newMouseOverScrollState){
				if (WorkspaceEventManager.instance.onMouseEnterScrollbar !=null)
				    WorkspaceEventManager.instance.onMouseEnterScrollbar();
			} else {
				if (WorkspaceEventManager.instance.onMouseExitScrollbar !=null)
					WorkspaceEventManager.instance.onMouseExitScrollbar();

			}
		}
		oldMouseOverScrollState = newMouseOverScrollState;
#endif
	}
	#endregion


	bool mouseOverVerticalScroll = false;
	float newX,newY;
	void showVerticalScroll(Vector2 camPosition){
		if (canvas.canvasCamera.cameraSize.y >= canvas.size.y){
			mouseOverVerticalScroll  = false;
			return;
		}
		
		newY = GUI.VerticalScrollbar(verticalScrollRect,
		                                   camPosition.y,
		                                   canvas.canvasCamera.cameraSize.y,
		                                   canvas.extents.y,
		                                   -canvas.extents.y);
		mouseOverVerticalScroll = verticalScrollRect.Contains(screenGUICoordinates);
		if (newY != camPosition.y){
			camPosition.y = newY;
			canvas.canvasCamera.cameraGlobalBottomLeftCornerPosition = camPosition;
		}
	}

	bool mouseOverHorizontalScroll = false;

	void showHorizontalScroll(Vector2 camPosition){
		if (canvas.canvasCamera.cameraSize.x >= canvas.size.x){
			mouseOverHorizontalScroll = false;
			return;
		}
		newX = GUI.HorizontalScrollbar(horizontalScrollRect,
		                                     camPosition.x,
		                                     canvas.canvasCamera.cameraSize.x, 
		                                     -canvas.extents.x, 
		                                     canvas.extents.x);
		mouseOverHorizontalScroll = horizontalScrollRect.Contains(screenGUICoordinates);
		if (newX != camPosition.x){
			camPosition.x = newX;
			canvas.canvasCamera.cameraGlobalBottomLeftCornerPosition = camPosition;
		}
	}



	
	void recalculatePositions(){
		int horizontalSliderHeight = horizontalScrollHeight + margin * 2;
		int verticalSliderWidth    = verticalScrollWidth    + margin * 2;
		
		horizontalScrollRect = new Rect(left + verticalSliderWidth,
					top + contentHeight - margin - horizontalScrollHeight,			
					contentWidth - verticalSliderWidth *2, 
					horizontalScrollHeight);
		verticalScrollRect = new Rect (left + contentWidth - margin - verticalScrollWidth, 
					top + horizontalSliderHeight,
					verticalScrollWidth,
					contentHeight - horizontalSliderHeight *2);
	}
}
