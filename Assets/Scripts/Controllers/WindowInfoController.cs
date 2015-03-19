using UnityEngine;
using System.Collections;

public class WindowInfoController : ControllerInterface {
	GameState previousState = GameState.IN_GAME;
	#region ControllerInterface implementation
	public void init ()
	{
		WorkspaceEventManager.instance.onInfoButtonClick+=onInfoButtonClickListener;
		WorkspaceEventManager.instance.onExitFromInfoWindow+=onExitFromInfoWindowListener;
	}
	#endregion

	void onInfoButtonClickListener(){
		previousState = PropertiesSingleton.instance.gameState;
		PropertiesSingleton.instance.gameState = GameState.SHOW_INFO;
	}
	
	void onExitFromInfoWindowListener(){
		PropertiesSingleton.instance.gameState = previousState;
	}
}
