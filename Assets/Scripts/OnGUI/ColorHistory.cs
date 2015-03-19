using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ColorHistoryConfig{
	public int right;
	public int top;
	public int itemWidth = 34;
	public int itemHeight = 56;
	public int margin = 11;
	public int itemNumber = 7;
	public string styleName = "colorHistory";
	public GUIStyle style;
	public Texture2D buttonBackground;
	
}


[Serializable]
public class ColorHistory : GUIPart {
	public ColorHistoryConfig config;
	public Rect[] itemsPosition;
	
	
	public void setTopRightPosition(int x, int y, GUISkin skin){		
		setStyle(skin,config.styleName,out config.style);
		config.top = y;
		config.right = x;
		recalculatePositions();
	}	

	public override void OnGUI ()
	{
		Color32 currentColor = GUI.color;
		for (int i = 0; i < config.itemNumber; i++) {
			GUI.color = PropertiesSingleton.instance.colorProperties.colorHistory[i];
			GUI.DrawTexture(itemsPosition[i],config.buttonBackground);
			GUI.color = currentColor;
			if (GUI.Button(itemsPosition[i],GUIContent.none, config.style) && WorkspaceEventManager.instance.onColorHistoryClick!=null)
				WorkspaceEventManager.instance.onColorHistoryClick(i);				
		}	
	}
	
	
	void recalculatePositions(){
		itemsPosition = new Rect[config.itemNumber];
		for (int i = 0; i < config.itemNumber; i++) {
			int x = config.right - config.itemWidth - i*(config.itemWidth+config.margin);
			itemsPosition[i] = new Rect(x,config.top,config.itemWidth,config.itemHeight);
		}
	}
	
}
