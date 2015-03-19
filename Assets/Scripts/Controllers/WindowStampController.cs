using UnityEngine;
using System.Collections;

public class WindowStampController : ControllerInterface {
	GameState previousGameState = GameState.IN_GAME;	
	#region ControllerInterface implementation

	public void init () {
		WorkspaceEventManager.instance.onStampSelectButtonClick += onStampSelectButtonClickListener;
		WorkspaceEventManager.instance.onExitFromStampChooserWindow += onExitFromStampChooserWindowListener;
		WorkspaceEventManager.instance.onStampClickInWindow +=onStampClickInWindowListener;
		setActiveStampId(Random.Range(0,PropertiesSingleton.instance.guiStampList.stampList.Count));
	}

	#endregion

	void setActiveStampId(int id) {
		PropertiesSingleton.instance.activeStampId = id;
		PropertiesSingleton.instance.activeStampTexture = PropertiesSingleton.instance.guiStampList.stampList [id].stampTexture;
	}

	void onStampClickInWindowListener(int id){
		PropertiesSingleton.instance.gameState = previousGameState;
		setActiveStampId (id);
	}

	void onStampSelectButtonClickListener(){
		previousGameState = PropertiesSingleton.instance.gameState;		
		PropertiesSingleton.instance.gameState = GameState.STAMP_SELECT;
	}
	void onExitFromStampChooserWindowListener(){
		PropertiesSingleton.instance.gameState = previousGameState;	
		releaseStampTextures();
	}

	void releaseStampTextures(){
		for (int i=0; i< PropertiesSingleton.instance.guiStampList.stampList.Count; i++){
			if (i != PropertiesSingleton.instance.activeStampId)
				PropertiesSingleton.instance.guiStampList.stampList[i].releaseTextures();
		}
	}
}
