using UnityEngine;
using System.Collections;
using System;

public enum BottomButtonType{
	UNDO,
	REDO,
	REFLASH,
	SAVE,
	SOUND,
	OPEN,
	HELP
}

[Serializable]
public class BottomMenuConfig{	
	public int buttonHeight = 56;
	public int buttonWidth = 56;		
	public int buttonPadding = 12;
	public int left;
	public int top;
	public string styleName = "menuButton";
	public GUIStyle style;
	
	public Texture2D undoIcon;
	public Texture2D redoIcon;
	public Texture2D resetIcon;
	public Texture2D saveIcon;
	public Texture2D openNewIcon;
	public Texture2D soundIcon;
	public Texture2D soundMuteIcon;
	public Texture2D helpIcon;
}

[Serializable]
public class BottomMenu : GUIPart {
	public BottomMenuConfig config;
	Rect   buttonUndoRect;
	Rect   buttonRedoRect;
	Rect   buttonResetRect;
	Rect   buttonNewRect;
	Rect   buttonSaveRect;
	Rect   buttonSoundRect;
	Rect   buttonHelpRect;
	
	
	bool guiEnabledValue;
	public override void OnGUI () {
		
		guiEnabledValue = GUI.enabled;
		
		GUI.enabled = canvas.canvasBuffer.undoCount > 0;
		if (GUI.Button(buttonUndoRect,  config.undoIcon,  config.style) && WorkspaceEventManager.instance.onUndoClick != null)			
			canvas.undo();
		GUI.enabled = guiEnabledValue;
			
			
		GUI.enabled = canvas.canvasBuffer.redoCount > 0;
		if (GUI.Button(buttonRedoRect,  config.redoIcon,  config.style) && WorkspaceEventManager.instance.onRedoClick != null)
			canvas.redo();
		GUI.enabled = guiEnabledValue;
			

		GUI.enabled = canvas.canvasBuffer.undoCount > 0 || canvas.canvasBuffer.redoCount >0;
		if (GUI.Button(buttonResetRect, config.resetIcon, config.style) && WorkspaceEventManager.instance.onResetSheetClick !=null)
			canvas.clearActiveColors(true);
		GUI.enabled = guiEnabledValue;

		if (GUI.Button(buttonSaveRect,  config.saveIcon,  config.style) &&  WorkspaceEventManager.instance.onSavePictureClick != null)
			WorkspaceEventManager.instance.onSavePictureClick();
		
		if (GUI.Button(buttonNewRect,  config.openNewIcon, config.style) && WorkspaceEventManager.instance.onMenuNewPictureClick != null)
			WorkspaceEventManager.instance.onMenuNewPictureClick();
		
		if (GUI.Button(buttonSoundRect, PropertiesSingleton.instance.soundProperties.mute ? config.soundMuteIcon : config.soundIcon, config.style) 
			&& WorkspaceEventManager.instance.onSoundButtonClick!=null)
			WorkspaceEventManager.instance.onSoundButtonClick();
		/* disable it here
		if (GUI.Button(buttonHelpRect,  config.helpIcon,  config.style) && WorkspaceEventManager.instance.onInfoButtonClick!=null)
			WorkspaceEventManager.instance.onInfoButtonClick();
		*/
	}		
	
	
	
	CanvasController canvas;
	public void setTopLeftOffset(int x, int y, GUISkin skin){		
		canvas = PropertiesSingleton.instance.canvasWorkspaceController.canvas;
		setStyle(skin, config.styleName,out config.style);
		
		config.top  = y;
		config.left = x;
		recalculatePositions();
	}
	
	void recalculatePositions(){
		int x = config.left;
		int y = config.top;
		buttonUndoRect = new Rect(x, y, config.buttonWidth, config.buttonHeight);
		x+=(config.buttonWidth+config.buttonPadding);
		buttonRedoRect = new Rect(x, y, config.buttonWidth, config.buttonHeight);
		x+=(config.buttonWidth+config.buttonPadding);
		buttonResetRect = new Rect(x, y, config.buttonWidth, config.buttonHeight);
		x+=(config.buttonWidth+config.buttonPadding);
		buttonSaveRect = new Rect(x, y, config.buttonWidth, config.buttonHeight);		
		x+=(config.buttonWidth+config.buttonPadding);
		buttonNewRect = new Rect(x, y, config.buttonWidth, config.buttonHeight);
		x+=(config.buttonWidth+config.buttonPadding);
		buttonSoundRect = new Rect(x, y, config.buttonWidth, config.buttonHeight);
		x+=(config.buttonWidth+config.buttonPadding);
		buttonHelpRect = new Rect(x, y, config.buttonWidth, config.buttonHeight);
	}			
}