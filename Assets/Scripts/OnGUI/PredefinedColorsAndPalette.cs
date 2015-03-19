using UnityEngine;
using System.Collections;
using System;


[Serializable]
public class PredefinedColorsConfig{
	public int left;
	public int down;
	public int totalWidth = 86;
	public int itemWidth;
	public int itemHeight = 37;
	public int colorsNumber = 12;
	public int colorPerRow = 2;
	public int marginY= 0;
	
	public string paletteButtonStyleName = "paletteButton";
	public GUIStyle paletteButtonStyle;
	
	public string leftButtonStyleName = "predefinedColorLeft";
	public Texture2D leftButtonBG;
	public GUIStyle leftButtonStyle;


	public string rightButtonStyleName = "predefinedColorRight";
	public Texture2D rightButtonBG;
	public GUIStyle rightButtonStyle;
	
	public string downLeftButtonStyleName = "predefinedColorDownLeft";
	public Texture2D downLeftButtonBG;

	public GUIStyle downLeftButtonStyle;
	public string downRightButtonStyleName = "predefinedColorDownRight";
	public Texture2D downRightButtonBG;
	public GUIStyle downRightButtonStyle;
	
}


[Serializable]
public class PredefinedColorsAndPalette : GUIPart {
	public PredefinedColorsConfig config;
	public Rect[] itemPositions;
	public Texture2D[]  bgs;
	public Rect palleteButtonRect;
	public GUIStyle[] styles;
	
	
	public void setDownLeftPosition(int x, int y, GUISkin skin){
		setStyle(skin, config.paletteButtonStyleName,   out config.paletteButtonStyle);
		setStyle(skin, config.leftButtonStyleName,      out config.leftButtonStyle);
		setStyle(skin, config.rightButtonStyleName,     out config.rightButtonStyle);
		setStyle(skin, config.downLeftButtonStyleName,  out config.downLeftButtonStyle);
		setStyle(skin, config.downRightButtonStyleName, out config.downRightButtonStyle);
		config.left = x;
		config.down = y;
		recalculatePositions();
	}
	
	
	public void recalculatePositions(){
		itemPositions = new Rect[config.colorsNumber];
		styles = new GUIStyle[config.colorsNumber];
		bgs = new Texture2D[config.colorsNumber];
		config.itemWidth = config.totalWidth / config.colorPerRow;		
		int rowNumber = config.colorsNumber / config.colorPerRow;
		int id=0;
		int y=-1;
		int x=-1;
		bool left = true;
		for (int i = 0; i < rowNumber; i++) {
			y = config.down - config.itemHeight - i*(config.itemHeight + config.marginY );
			for (int j = 0; j < config.colorPerRow; j++) {
				x = config.left + config.itemWidth*j;
				styles[id] = left ? config.leftButtonStyle : config.rightButtonStyle;
				bgs   [id] = left ? config.leftButtonBG    : config.rightButtonBG   ;
				itemPositions[id++]= new Rect(x,y,config.itemWidth,config.itemHeight);	
				left = !left;
			}
		}
		
		palleteButtonRect = new Rect(config.left, y-config.itemHeight,config.totalWidth,config.itemHeight);
		styles[0] = config.downLeftButtonStyle;
		styles[1] = config.downRightButtonStyle;
 		bgs[0] = config.downLeftButtonBG;
		bgs[1] = config.downRightButtonBG;
 	}
	
	
	int i;
	public override void OnGUI (){
		Color previousColor = GUI.color;
		for (i = 0; i < config.colorsNumber; i++) {
			GUI.color = PropertiesSingleton.instance.colorProperties.predefinedColors[i];
			GUI.DrawTexture(itemPositions[i],bgs[i]);
			GUI.color = previousColor;
			if (GUI.Button(itemPositions[i],GUIContent.none, styles[i]) && WorkspaceEventManager.instance.onPredefinedColorClick!=null)
				WorkspaceEventManager.instance.onPredefinedColorClick(i);				
		}
		if (GUI.Button(palleteButtonRect,GUIContent.none, config.paletteButtonStyle) && WorkspaceEventManager.instance.onPalleteOpenWindowClick!=null)
			WorkspaceEventManager.instance.onPalleteOpenWindowClick();
		
	}	
	
	public void setPosition (Rect rect)
	{		
	}	
}
