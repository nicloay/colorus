using UnityEngine;
using System.Collections;
using System;
using localisation;

[Serializable]
public class StampSelectWindowConfig{
	public int centerX;
	public int centerY;
	public int windowPadding = 30;
	public int windowWidth = 740;
	public int windowHeight = 520;
	public int stampButtonMargin = 10;
	public int stampButtonWidth = 140;
	public int stampButtonHeight = 140;	
	public int rowNumber = 3;
	public int colNumber = 4;

	public int      scrollAreaPadding = 3;

	public string stampWindowButtonStyleName = "pictureButton";
	public GUIStyle stampWindowButtonStyle;
	public string windowCaption = "select stamp";
	public GUIStampList stampList;
}

[Serializable]
public class WindowStampSelect : GUIPart {
	public StampSelectWindowConfig config;
	public GuiModalWindow window;
	public Rect[] stampButtonRect;
	public Rect windowRect;
	public Rect scrollAreaRect;
	public Rect viewRect;
	public Vector2 scrollPosition;
	GUISkin currentSkin;
	ScrollviewAutocloser autocloser;


	Event e;
	public override void OnGUI ()
	{
		e= Event.current;
		autocloser.preprocess(e, ref scrollPosition);
		window.OnGUI(new GUIContent(config.windowCaption.Localized()));
		autocloser.postProcess(e, ref scrollPosition);
	}

	Color oldColor;
	void doMyWindow(){
		
		scrollPosition = GUI.BeginScrollView(scrollAreaRect,scrollPosition,viewRect,false,true);
			oldColor = GUI.contentColor	;
			GUI.contentColor = PropertiesSingleton.instance.colorProperties.activeColor;
			for (int i = 0; i < stampButtonRect.Length; i++) {
				if (GUI.Button(stampButtonRect[i], config.stampList.stampList[i].stampTexture, config.stampWindowButtonStyle)
			    && WorkspaceEventManager.instance.onStampClickInWindow!=null)
				WorkspaceEventManager.instance.onStampClickInWindow(i);
			}
			GUI.contentColor = oldColor;
		GUI.EndScrollView();		
	}
	

	public void setCenterPosition(int x, int y, GUISkin skin){
		currentSkin = skin;
		config.centerX = x;
		config.centerY = y;
		setStyle(skin,config.stampWindowButtonStyleName,out config.stampWindowButtonStyle);
		
		if (PropertiesSingleton.instance.guiStampList == null)
			Debug.LogError("guiStampList in prop singleton is empty please fix it");
		config.stampList = PropertiesSingleton.instance.guiStampList;
		recalculatePositions();
		window.setProperties(windowRect, new GUIContent(config.windowCaption.Localized()), skin, doMyWindow, 
			WorkspaceEventManager.instance.onExitFromStampChooserWindow,
			WorkspaceEventManager.instance.onExitFromStampChooserWindow);
		autocloser = new ScrollviewAutocloser(config.scrollAreaPadding, config.stampButtonWidth  + config.stampButtonMargin);
	}
	
	void recalculatePositions(){
		GUIStyle verticalScrollBar = currentSkin.FindStyle("verticalScrollbar");
		int scrollWidth =(int)( verticalScrollBar.fixedWidth + verticalScrollBar.margin.left + verticalScrollBar.margin.right);
		
		
		int contentHeight = config.stampButtonHeight * config.rowNumber 
				   + config.stampButtonMargin * (config.rowNumber - 1)
				   + config.scrollAreaPadding * 2;
		int contentWidth = config.stampButtonWidth * config.colNumber 
				   + config.stampButtonMargin * (config.colNumber -1)
				   + scrollWidth
				   + config.scrollAreaPadding * 2;
		
		
		config.windowHeight = contentHeight + 2 * config.windowPadding;
		
		config.windowWidth = contentWidth   + 2 * config.windowPadding;
		int windowLeft = config.centerX - config.windowWidth  / 2;
		int windowTop  = config.centerY - config.windowHeight / 2; 
		
		windowRect = new Rect(windowLeft,windowTop, config.windowWidth, config.windowHeight);
		int length = config.stampList.stampList.Count;
		stampButtonRect = new Rect[length];
		int colCounter = 0;
		int rowCounter = 0;
		int x;
		int y;
		for (int i = 0; i < length; i++) {
			x = config.scrollAreaPadding + (config.stampButtonWidth  + config.stampButtonMargin) * colCounter;
			y = config.scrollAreaPadding + (config.stampButtonHeight + config.stampButtonMargin) * rowCounter; 
			stampButtonRect[i] = new Rect(x,y,config.stampButtonWidth, config.stampButtonHeight);
			if (++colCounter == config.colNumber){
				colCounter =0;
				rowCounter++;
			}			
		}	
		
		
		scrollAreaRect = new Rect(config.windowPadding, config.windowPadding, contentWidth, contentHeight);
		viewRect = new Rect(0,0,
		                    contentWidth - scrollWidth, 
		                    rowCounter * (config.stampButtonHeight + config.stampButtonMargin) 
		                    	- config.stampButtonMargin + config.scrollAreaPadding * 2);
		
		
	}
	
	
	
}
