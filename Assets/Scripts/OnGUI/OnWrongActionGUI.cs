using UnityEngine;
using System.Collections;
using System;


[Serializable]
public class OnWrongActionGUIConfig{
	public Color32 borderColor = Color.red;
	public int borderSize = 3;
	public Texture2D texture;
}

[Serializable]
public class OnWrongActionGUI : GUIPart {
	public OnWrongActionGUIConfig config;
	PropertiesSingleton props;
	Rect left;
	Rect right;
	Rect top;
	Rect bottom;

	public void setScreenSize(MainMenuConfig mainMenuConfig){
		props = PropertiesSingleton.instance;
		int width = mainMenuConfig.actualWidth;
		int height = mainMenuConfig.actualHeight;
		left = right = new Rect(0,0, config.borderSize, height);
		right.x = width - config.borderSize;
		top = bottom = new Rect (config.borderSize, 0, width - config.borderSize*2, config.borderSize);
		bottom.y = height - config.borderSize;
	}

	Color32 previousColor;

	public override void OnGUI () {
		if (!props.wrongActionShowFrame)
			return;
		previousColor = GUI.color;
		GUI.color = config.borderColor;
		GUI.DrawTexture(left, config.texture);
		GUI.DrawTexture(right, config.texture);
		GUI.DrawTexture(top, config.texture);
		GUI.DrawTexture(bottom, config.texture);
		GUI.color = previousColor;
	}


}
