using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using localisation;

[Serializable]
public class CreditInfo{
	public Texture2D icon;
	public string text;
	public bool empty = false;
	[HideInInspector]
	public Rect iconRect;
	[HideInInspector]
	public Rect textRect;	
}

[Serializable]
public class InfoWindowConfig{
	public int width = 400;
	public int height = 300;
	
	public int creditIconWidth = 50;
	public int creditLineHeight = 50;
	public int creditWidth = 400;
	public int margin=5;	

	public Rect windowRect;
	public string windowCaption = "Info";
	public int centerX;
	public int centerY;
	
	public string creditStyleName  = "creditsText";
	public GUIStyle creditStyle;
}

[Serializable]
public class WindowInfo : GUIPart {
	public InfoWindowConfig config;
	public CreditInfo[] credits;	
	public GuiModalWindow          window;
	
	#region implemented abstract members of GUIPart
	public override void OnGUI ()
	{
		window.OnGUI(new GUIContent(config.windowCaption.Localized()));
	}
	#endregion
	
	
	public void setCenterPosition(int x, int y, GUISkin skin){
		config.centerX = x;
		config.centerY = y;
		setStyle(skin,config.creditStyleName, out config.creditStyle);
		
		recalculatePositions();
		window.setProperties(config.windowRect,new GUIContent(config.windowCaption.Localized()),skin,doMyWindow,
			WorkspaceEventManager.instance.onExitFromInfoWindow,
			WorkspaceEventManager.instance.onExitFromInfoWindow);
	}
	
	void doMyWindow(){
		for (int i = 0; i < credits.Length; i++) {
			if (!credits[i].empty){
				if (credits[i].icon!=null)
					GUI.DrawTexture(credits[i].iconRect, credits[i].icon);
				GUI.Label(credits[i].textRect, credits[i].text, config.creditStyle);
			}
		}
	}
	
	
	void recalculatePositions(){
		
		
		
		int yStep = config.creditLineHeight+config.margin;
		int y = 0;
		for (int i = 0; i < credits.Length; i++) {
			CreditInfo c = credits[i];
			if (!string.IsNullOrEmpty( c.text)){				
				int textLeft; 
				int textWidth;
				if (c.icon!=null){
					textLeft = config.creditIconWidth+config.margin;
					textWidth = config.creditWidth-textLeft;		
					c.iconRect = new Rect(0,y,config.creditIconWidth,config.creditLineHeight);
				} else {
					textLeft = 0;
					textWidth = config.creditWidth;				
				}
				c.textRect = new Rect(textLeft,y,textWidth,config.creditLineHeight);
			} else {
				c.empty = true;
			}
			y+=yStep;
		}
		config.height = y;		
		config.windowRect = new Rect(config.centerX - config.width/2, config.centerY - config.height/2, config.width,config.height);
	}
}
