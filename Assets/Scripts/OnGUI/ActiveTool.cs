using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class ToolIcon{
	public ToolType type;
	public Texture2D icon;
	[NonSerialized]
	public GUIStyle buttonStyle;
	[NonSerialized]
	public Action   action;
	[NonSerialized]
	public Action drawIconAction;
}


[Serializable]
public class ActiveToolConfig{
	public int left;
	public int top;
	public int activeToolHeight = 120;
	public int width = 86;
	public int iconPadding = 12;
	public Texture2D regionOnIcon;
	public Texture2D regionOffIcon;
	
	public string activeToolStyleName = "activeTool";
	public GUIStyle activeToolStyle;
	public GUIStyle activePassiveTooLStyle;

	public List<ToolIcon> toolIcons;
}


[Serializable]
public class ActiveTool : GUIPart {
	public ActiveToolConfig config;
	Rect activeToolRect;
	Rect additionalButtonRect;
	Rect stampRect;
	Rect cloudRect;
	Dictionary<ToolType, ToolIcon> iconCache;
	
	public void setTopLeftPosition(int left, int top, GUISkin skin){
		config.activeToolStyle = skin.FindStyle(config.activeToolStyleName);
		config.activePassiveTooLStyle =  new GUIStyle(config.activeToolStyle);
		config.activePassiveTooLStyle.hover.background = null;
		config.activePassiveTooLStyle.active.background = null;
		config.top = top;
		config.left = left;
		activeToolRect = new Rect(left, top,config.width,config.activeToolHeight);

		stampRect = activeToolRect;
		stampRect.x+= config.iconPadding;
		stampRect.y+= config.iconPadding;
		stampRect.width = getActiveStampIcon().width;
		stampRect.height = getActiveStampIcon().height;
		PropertiesSingleton.instance.guiStampList.stampList[PropertiesSingleton.instance.activeStampId].releaseTextures();

		cloudRect = stampRect;
		cloudRect.width = config.regionOnIcon.width;
		cloudRect.height = config.regionOnIcon.height;
		setDictionaryCache();
	}
	
	void setDictionaryCache(){
		iconCache = new Dictionary<ToolType, ToolIcon>();
		for (int i = 0; i < config.toolIcons.Count; i++){
			iconCache.Add(config.toolIcons[i].type, config.toolIcons[i]);
			switch(config.toolIcons[i].type){
			case ToolType.BRUSH:
			case ToolType.BRUSH_LARGE:
			case ToolType.INK:
			case ToolType.ROLLER:
				config.toolIcons[i].action = WorkspaceEventManager.instance.onDrawWithinRegionClick;
				config.toolIcons[i].buttonStyle = config.activeToolStyle;
				config.toolIcons[i].drawIconAction = showCloudIcon;
				break;
			case ToolType.STAMP:
				config.toolIcons[i].action = WorkspaceEventManager.instance.onStampSelectButtonClick;
				config.toolIcons[i].buttonStyle = config.activeToolStyle;
				config.toolIcons[i].drawIconAction = showStampIcon;
				break;
			default:
				config.toolIcons[i].action = strangeToolAction;
				config.toolIcons[i].buttonStyle = config.activePassiveTooLStyle;
				break;		
			}
		} 
	}

	void strangeToolAction(){
		WorkspaceEventManager.instance.onWrongAction("strange tool action");
	}
	
	public override void OnGUI ()
	{		
		if (GUI.Button(activeToolRect,
		               iconCache[PropertiesSingleton.instance.activeTool].icon, 
		               iconCache[PropertiesSingleton.instance.activeTool].buttonStyle) 
		    && iconCache[PropertiesSingleton.instance.activeTool].action != null)
		{
			iconCache[PropertiesSingleton.instance.activeTool].action();
		}
		if (iconCache[PropertiesSingleton.instance.activeTool].drawIconAction != null)
			iconCache[PropertiesSingleton.instance.activeTool].drawIconAction();
	}

	void showCloudIcon(){
		GUI.DrawTexture(cloudRect, 
		                PropertiesSingleton.instance.drawWithinRegion ? config.regionOnIcon : config.regionOffIcon);
	}

	Color oldColor;
	void showStampIcon(){
		oldColor = GUI.contentColor;
		GUI.contentColor = PropertiesSingleton.instance.colorProperties.activeColor;
		GUI.DrawTexture(stampRect, getActiveStampIcon());
		GUI.contentColor = oldColor;
	}

	Texture2D getActiveStampIcon(){
		return PropertiesSingleton.instance.guiStampList.stampList[PropertiesSingleton.instance.activeStampId].iconTexture;
	}
}
