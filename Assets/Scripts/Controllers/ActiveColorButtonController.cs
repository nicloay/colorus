using UnityEngine;
using System.Collections;

public class ActiveColorButtonController : ControllerInterface {

	#region ControllerInterface implementation
	public void init ()
	{
		CanvasController.events.onActiveReceiveNewColors += onActiveReceiveNewColorsListener;

	}
	#endregion

	void onActiveReceiveNewColorsListener () {
		if (PropertiesSingleton.instance.colorProperties.randomEnabled)
			setNewRandomColor();
	}

	void setNewRandomColor(){
		Color32 newRandomColor = ColorUtil.getRandomColor();
		if (WorkspaceEventManager.instance.onSelectColor!=null)
			WorkspaceEventManager.instance.onSelectColor(newRandomColor);
	}
}
