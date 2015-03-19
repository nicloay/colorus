using UnityEngine;
using System.Collections;

public class ToolButtonsController : ControllerInterface {
	#region ControllerInterface implementation
	public void init ()
	{
		WorkspaceEventManager.instance.onToolButtonClick+=onToolButtonClickListener;
	}
	#endregion	
	
	void onToolButtonClickListener(ToolType clickedTool){
		PropertiesSingleton.instance.activeTool = clickedTool;
	}
}
