using System;


//TODO move that to main eventManager
public delegate void OnLayerQueueChange(int activeLayersCount, int redoLayersCount);

public class LayerQueueEventManager: MonoSingleton<LayerQueueEventManager>
{
	public OnLayerQueueChange onLayerQueueChange;
}


