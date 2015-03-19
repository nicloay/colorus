using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class ActiveColorConfig{
	public int left;
	public int top;	
	public int width=86;
	public int height=56;
	public int innerButtonGap = 11;
	public Texture2D randomColorTexture;
	public Texture2D staticColorTexture;
	public Texture2D inerRandomColorTexture;
	public Texture2D inerStaticColorTexture;


	// new styles here
	public int colorBGWidth  = 45;
	public int colorBGHeight = 45;
	public string activeColorStyleName = "activeColor";
	public GUIStyle activeColorStyle;
	public Texture2D bgTexture;
}

[Serializable]
public class ActiveColor : GUIPart {
	public ActiveColorConfig config;
	Rect activeColorRect;
	Rect inerButtonRect;
	
	public void setTopLeftPosition(int left, int top, GUISkin skin){
		config.activeColorStyle = skin.FindStyle(config.activeColorStyleName);
		config.left = left;
		config.top = top;
		recalculatePositions();
	}
	
	
	void recalculatePositions(){
		activeColorRect = new Rect(config.left,config.top,config.width,config.height);
		inerButtonRect = activeColorRect;
		inerButtonRect.width = config.colorBGWidth;
		inerButtonRect.height = config.colorBGHeight;		
	}

	int nextControlId;
	bool newValue;
	bool colorPressed;
	Color32 oldColor;
	public override void OnGUI (){
		colorPressed = (GUIUtility.hotControl == nextControlId);
		if ((!PropertiesSingleton.instance.colorProperties.randomEnabled && !colorPressed ) 
		    || (PropertiesSingleton.instance.colorProperties.randomEnabled && colorPressed)
		    )
		{
			oldColor = GUI.color;
			GUI.color = PropertiesSingleton.instance.colorProperties.activeColor;
			GUI.DrawTexture(inerButtonRect, config.bgTexture);
			GUI.color = oldColor;
		}
		nextControlId = GUIUtility.GetControlID(FocusType.Passive) +1;
		PropertiesSingleton.instance.colorProperties.randomEnabled = GUI.Toggle(activeColorRect,PropertiesSingleton.instance.colorProperties.randomEnabled, GUIContent.none, config.activeColorStyle);



	}
	
	public void setPosition (Rect rect)
	{	
	}

	
}
