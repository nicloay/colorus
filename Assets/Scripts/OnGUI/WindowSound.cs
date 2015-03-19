using UnityEngine;
using System.Collections;
using System;
using localisation;

[Serializable]
public class WindowSoundConfig{
	[HideInInspector]
	public int contentWidth;
	[HideInInspector]
	public int contentHeight;
	[HideInInspector]
	public int scrollHeight;


	public int buttonWidth = 56;
	public int margin = 12;
	public Texture2D soundIconTexture;
	public Texture2D sfxIconTexture;

	public Texture2D buttonMainTexture;
	public Texture2D buttonMainMuteTexture;
	
	public string windowCaption = "Sound";
	[HideInInspector]
	public Rect windowRect;


	public string smallButtonStyleName = "toolButton";
	public GUIStyle smallButtonStyle;

	public string bigButtonStyleName = "menuButton";
	public GUIStyle bigButtonStyle;


	public string levelBGStyleName = "soundLevelBG";
	public GUIStyle levelBGStyle;

	public string thumbStyleName = "soundThumb";
	public GUIStyle thumbStyle;

	[HideInInspector]
	public Rect soundButtonRect;
	[HideInInspector]
	public Rect sfxButtonRect;
	[HideInInspector]
	public Rect soundLevelRect;
	[HideInInspector]
	public Rect sfxLevelRect;
	[HideInInspector]
	public Rect muteButtonRect;

	public Color32 disabledTintColor=Color.gray;

}



[Serializable]
public class WindowSound : GUIPart {
	public WindowSoundConfig config;
	
	public GuiModalWindow          window;

	#region implemented abstract members of GUIPart
	public override void OnGUI () {
		window.OnGUI(new GUIContent(config.windowCaption.Localized()));
	}
	#endregion


	public void setSoundButtonTopLeftPosition(int left, int top, GUISkin skin){

		setStyle(skin, config.smallButtonStyleName, out config.smallButtonStyle);
		setStyle(skin, config.bigButtonStyleName  , out config.bigButtonStyle);
		setStyle(skin, config.levelBGStyleName    , out config.levelBGStyle);
		setStyle(skin, config.thumbStyleName      , out config.thumbStyle);


		config.contentWidth = config.margin * 2 + config.buttonWidth * 3;
		config.contentHeight  = config.margin* 2 + config.buttonWidth * 2;
		config.scrollHeight = config.buttonWidth / 2;

		int windowLeft = left - config.buttonWidth - config.margin;
		int windowTop  = top - config.contentHeight - config.margin*2;

		config.windowRect = new Rect (windowLeft,windowTop,config.contentWidth, config.contentHeight);

		window.setProperties(config.windowRect,
		                     new GUIContent(config.windowCaption.Localized()),
		                     skin,
		                     doMyWindow,
		                     onExitFromWindow,
		                     onExitFromWindow);

		int scrollWidth = config.contentWidth - config.scrollHeight - config.margin;
		int scrollLeft = config.scrollHeight + config.margin;
		config.soundButtonRect = new Rect(0         , 0, config.scrollHeight, config.scrollHeight);
		config.soundLevelRect = new Rect (scrollLeft, 0, scrollWidth        , config.scrollHeight);
		config.sfxButtonRect  = new Rect(0       , config.scrollHeight + config.margin, config.scrollHeight, config.scrollHeight);
		config.sfxLevelRect = new Rect(scrollLeft, config.scrollHeight + config.margin, scrollWidth        , config.scrollHeight);


		config.muteButtonRect = new Rect (0, config.buttonWidth+ config.margin*2, config.contentWidth, config.buttonWidth);
	}

	Color32 cachedColor;
	Texture2D buttonTexture;
	float newSFXLevel;
	float newMusicLevel;
	void doMyWindow(){
		cachedColor = GUI.color;

		if (PropertiesSingleton.instance.soundProperties.mute || PropertiesSingleton.instance.soundProperties.musicMute)
			GUI.color = config.disabledTintColor;

		if (GUI.Button(config.soundButtonRect, config.soundIconTexture, config.smallButtonStyle) 
	    		&& WorkspaceEventManager.instance.onSoundWindowMusicMuteClick != null)
			WorkspaceEventManager.instance.onSoundWindowMusicMuteClick();

		newMusicLevel = GUI.HorizontalSlider(config.soundLevelRect, 
							PropertiesSingleton.instance.soundProperties.musicLevel,
							0,
							1, 
							config.levelBGStyle, config.thumbStyle);
		if (newMusicLevel != PropertiesSingleton.instance.soundProperties.musicLevel 
	    		&& WorkspaceEventManager.instance.onSoundWindowMusicLevelChange != null)
			WorkspaceEventManager.instance.onSoundWindowMusicLevelChange(newMusicLevel);
		GUI.color = cachedColor;


		if (PropertiesSingleton.instance.soundProperties.mute || PropertiesSingleton.instance.soundProperties.sfxMute)
			GUI.color = config.disabledTintColor;

		if (GUI.Button(config.sfxButtonRect, config.sfxIconTexture, config.smallButtonStyle)
			&& WorkspaceEventManager.instance.onSoundWindowSFXMuteClick !=null)
			WorkspaceEventManager.instance.onSoundWindowSFXMuteClick();
		newSFXLevel = GUI.HorizontalSlider(config.sfxLevelRect,
							PropertiesSingleton.instance.soundProperties.sfxLevel,
							0,
							1,
							config.levelBGStyle, config.thumbStyle);
		if (newSFXLevel != PropertiesSingleton.instance.soundProperties.sfxLevel
		    	&& WorkspaceEventManager.instance.onSoundWindowSFXLevelChange != null)
			WorkspaceEventManager.instance.onSoundWindowSFXLevelChange(newSFXLevel);

		GUI.color = cachedColor;
		buttonTexture = !PropertiesSingleton.instance.soundProperties.mute ? config.buttonMainTexture : config.buttonMainMuteTexture;

		if (PropertiesSingleton.instance.soundProperties.mute)
			GUI.color = config.disabledTintColor;
		if (GUI.Button(config.muteButtonRect, buttonTexture, config.bigButtonStyle) 
		    && WorkspaceEventManager.instance.onSoundWindowMuteClick != null)
			WorkspaceEventManager.instance.onSoundWindowMuteClick();
		GUI.color = cachedColor;
	}

	void onExitFromWindow(){
		if (WorkspaceEventManager.instance.onExitFromSoundSettingsWindow!=null)
			WorkspaceEventManager.instance.onExitFromSoundSettingsWindow();
	}
}
