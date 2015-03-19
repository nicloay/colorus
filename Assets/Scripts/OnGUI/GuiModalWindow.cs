using UnityEngine;
using System.Collections;
using System;


[Serializable]
public class GuiModalWindow {
	public GUISkin skin;
	
	public int         windowWidth;
	public int         windowHeight;
	public Rect        windowRect;
	public int         windowDragHeight;
	public Rect        windowDragRect;
	public GUIContent  windowGuiContent;

	[NonSerialized]
	public int         closeButtonWidth;
	[NonSerialized]
	public int         closeButtonHeight;
	public int         closeButtonMargin = 8;
	public string      closeButtonStyleName = "closeWindowButton";
	public GUIStyle    closeButtonStyle;
	public Rect        closeButtonRect;
	public Rect        windowOverFlowRect;
	public Rect        windowContentRect;
	
	public Action      onExitByEscOrClose;
	public Action      onClickOutsideOfWindow;
	public Action doMyWindowMethod;
	
	int closeButtonMarginX;
	int closeButtonMarginY;
	
	static GUIStyle windowStyle;
	static string windowStyleName = "window";
	public void setProperties(Rect windowContentRect, GUIContent windowGuiContent, GUISkin skin,
				Action doMyWindowMethod, Action onExitByEscOrClose, Action onClickOutsideOfWindow){
		if (windowStyle==null)
			windowStyle = skin.FindStyle(windowStyleName);
		closeButtonStyle = skin.FindStyle(closeButtonStyleName);
		this.doMyWindowMethod = doMyWindowMethod;
		this.onExitByEscOrClose = onExitByEscOrClose;
		this.onClickOutsideOfWindow = onClickOutsideOfWindow;
		this.skin = skin;
		this.windowGuiContent = windowGuiContent;



		windowRect = new Rect ( windowContentRect.x - windowStyle.padding.left,
						windowContentRect.y - windowStyle.padding.top,
						windowContentRect.width + windowStyle.padding.horizontal,
						windowContentRect.height + windowStyle.padding.vertical);
		windowContentRect.x = windowStyle.padding.left;
		windowContentRect.y = windowStyle.padding.top;
		this.windowContentRect = windowContentRect;
		this.windowDragHeight = windowStyle.padding.top;
		
		updateDragRect();	
		closeButtonHeight = closeButtonStyle.normal.background.height - closeButtonStyle.overflow.vertical;
		closeButtonWidth = closeButtonStyle.normal.background.width - closeButtonStyle.overflow.horizontal;
		
		int top = 8;
		int left = (int)( windowRect.width - closeButtonWidth) - 12;
		
		closeButtonRect = new Rect(left,
			top,
			closeButtonWidth,
			closeButtonHeight);			
		
	}

	void updateDragRect(){
		windowDragRect = new Rect(windowRect.x, windowRect.y, windowRect.width, windowStyle.padding.top);
	}

	
	
	public void OnGUI(GUIContent content){
		GUI.BeginGroup(windowRect, content, skin.window);
		doThisWindow();
		GUI.EndGroup();
		handleWindowEvent();
	}

	
	public void OnGUI(){
		OnGUI(windowGuiContent);
	}
	
	void doThisWindow(){
		if (GUI.Button(closeButtonRect, GUIContent.none, closeButtonStyle) && onExitByEscOrClose != null)
			onExitByEscOrClose();		
		GUI.BeginGroup(windowContentRect);
			doMyWindowMethod();	
		GUI.EndGroup();
	}
	
	void handleWindowEvent(){			
		Event e = Event.current;	
		if (e.type == EventType.Used || e.type == EventType.Ignore)
			return;
		if (e.isKey && e.keyCode == KeyCode.Escape && onExitByEscOrClose!=null){		
			isDragging = false;
			e.Use();
			WorkspaceEventManager.instance.onExitByEscFromColorPicker();
		}


		dragNDrop(e);



    		if (e.type == EventType.mouseDown && !windowRect.Contains(e.mousePosition) && onClickOutsideOfWindow!=null){
			e.Use();		
			onClickOutsideOfWindow ();
		}
	}

	bool isDragging = false;

	Vector2 initialOffset;
	void dragNDrop(Event e){
		if (isDragging){
			if (e.type == EventType.mouseDrag){
				//;
				//GUI.matrix[1,1];
				float x = e.mousePosition.x - initialOffset.x;
				float y = e.mousePosition.y - initialOffset.y;
				float x1 = x + windowRect.width;
				float y1 = y + windowRect.height;
				x = x < 0 ? 0 : x;
				y = y < 0 ? 0 : y;
				float screenWidth  = Screen.width / GUI.matrix[0,0];
				float screenHeight = Screen.height / GUI.matrix[1,1];
				x = x1 > screenWidth  ? screenWidth  - windowRect.width  : x;
				y = y1 > screenHeight ? screenHeight - windowRect.height : y;
				windowRect.x = x;
				windowRect.y = y;
				updateDragRect();				
			} else if (e.type == EventType.mouseUp){
				isDragging = false;
				e.Use();
			}
		} else {
			if (e.type == EventType.mouseDown && windowDragRect.Contains(e.mousePosition)){
				isDragging = true;			
				initialOffset = new Vector2(
						e.mousePosition.x - windowRect.x,
						e.mousePosition.y - windowRect.y
					);
				e.Use();
			}
		}
		 


	}

}
