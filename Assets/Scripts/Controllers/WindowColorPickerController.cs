using UnityEngine;
using System.Collections;

public class WindowColorPickerController : ControllerInterface {
	GameState previousGameState = GameState.IN_GAME; 
	#region ControllerInterface implementation

	public void init () {
		WorkspaceEventManager.instance.onPalleteOpenWindowClick   += onColorPickerOpenListener;
		WorkspaceEventManager.instance.onExitByEscFromColorPicker += onExitFromColorPickerListener;
		WorkspaceEventManager.instance.onColorPickerClose         += onExitFromColorPickerListener;
	}

	#endregion

	void onColorPickerOpenListener(){
		previousGameState = PropertiesSingleton.instance.gameState;
		PropertiesSingleton.instance.gameState = GameState.COLOR_PICKER_ACTIVE;
	}
	
	void onExitFromColorPickerListener(){
		PropertiesSingleton.instance.gameState = previousGameState;
	}
	void onExitFromColorPickerListener(Color32 color){
		onExitFromColorPickerListener();
	}

}
