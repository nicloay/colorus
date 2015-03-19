using UnityEngine;
using System.Collections;

public class UndoRedoController : ControllerInterface {
	#region ControllerInterface implementation
	public void init ()
	{
		PropertiesSingleton.instance.undoEnabled = false;
		PropertiesSingleton.instance.redoEnabled = false;
		LayerQueueEventManager.instance.onLayerQueueChange+=onLayerQueueChangeListener;		
	}
	#endregion
	
	
	void onLayerQueueChangeListener(int activeLayersCount, int redoLayersCount){
		PropertiesSingleton.instance.undoEnabled = (activeLayersCount > 0);
		PropertiesSingleton.instance.redoEnabled = (redoLayersCount   > 0);		
	}
	
}
