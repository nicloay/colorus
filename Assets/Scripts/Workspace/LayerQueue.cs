using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class LayerQueue
{
	LayerController[] lcArray                       ;
	int               capacity                      ;
	int               downLayerId                   ;
	int               currentLayerId                ;
	int               activeLayersCount             ;
	int               redoLayersCount               ;
	bool              initializedFirstLayer = false ;
	
	public LayerQueue(LayerController[]  layerControllerArray){	 	
		capacity=layerControllerArray.Length;		
		lcArray=sortList (layerControllerArray);	
		for (int i = 0; i < lcArray.Length; i++) {
			lcArray[i].deactivate();			
		}		
		activeLayersCount     = 0     ;
		redoLayersCount       = 0     ;
		initializedFirstLayer = false ;
	}
	
	
	public bool hasFreeLayers(){
		return (activeLayersCount < (capacity-1));		
	}
	
	public LayerController fetchFreeLayer(){
		if (! hasFreeLayers())
			throw new System.Exception("don't have free element in queue");
		if (!initializedFirstLayer){
			initializedFirstLayer = true;
			currentLayerId = downLayerId = capacity-1;			
		} else {
			currentLayerId = getPreviousIndex(currentLayerId);
		}
		activeLayersCount++;
		redoLayersCount = 0;
		sendNotification();
		return lcArray[currentLayerId];		
	}
	
	public LayerController getDownLayer(){
		return lcArray[downLayerId];	
	}
	
	public void releaseDownLoayer(){
		downLayerId = getPreviousIndex(downLayerId);
		activeLayersCount -- ;
	}
	
	public bool hasLayerInQueue(){
		return (activeLayersCount > 0);	
	}
	
	public LayerController getCurrentLayer(){
		return lcArray[currentLayerId];	
	}
	
	

	public bool hasRedoLayerAbove(){
		return (redoLayersCount > 0);	
	}
	
	public LayerController setCurrentLayerAbove(){
		if (!hasRedoLayerAbove())
			throw new System.Exception("don't have redo layers above");
		currentLayerId = getPreviousIndex(currentLayerId);
		redoLayersCount   -- ;
		activeLayersCount ++ ;
		sendNotification();
		return lcArray[currentLayerId];
	}
	
	public bool hasUndoLayerBelow(){
		return (activeLayersCount > 0);	
	}
	
	public LayerController setCurrentLayerBelow(){
		if (!hasUndoLayerBelow())
			throw new System.Exception("don't have undo layers below");
		currentLayerId = getNextIndex(currentLayerId);
		activeLayersCount -- ;
		redoLayersCount   ++ ;
		sendNotification();
		return lcArray[currentLayerId];
	}
	
	public int getPreviousIndex (int index)
	{
		index--;
		if (index<0)
			index=capacity-1;
		return index;
	}
	
	private int getNextIndex(int index){
		index++;
		if (index==capacity)
			index=0;
		return index;
	}
	
	public LayerController[] getLayerQueueArray(){
		return lcArray;	
	}
	
	public int getVisibleLayerCount(){
		return activeLayersCount;	
	}
	
	
	public LayerController[] getVisibleLayersCurrentLayerFirst(){		
		LayerController[] ret=new LayerController[getVisibleLayerCount()];
		int layerId=currentLayerId;
		
		for (int i = 0; i < ret.Length; i++) {
			ret[i] = lcArray[layerId];
			layerId++;
			if (layerId == capacity)
				layerId = 0;
		}		
		return ret;
	}
	
	private void sendNotification(){
		if (LayerQueueEventManager.instance.onLayerQueueChange != null )
			LayerQueueEventManager.instance.onLayerQueueChange(activeLayersCount, redoLayersCount);
	}
	
	
	private LayerController[] sortList (LayerController[]  layerControllerArray)
	{
		return layerControllerArray.Select(layerController=>layerController)
		.OrderBy(layerController=>layerController.gameObject.transform.position.z)
		.ToArray();;
	}
	
	public string getFullStatus(){
		string result = "LayerQueue:\n";
		
		//LayerController[] lcArray                       ;
		result += "capacity              = " + capacity          + "\n" +
			  "downLayerId           = " + downLayerId       + "\n" +
			  "currentLayerId        = " + currentLayerId    + "\n" +
			  "activeLayersCount     = " + activeLayersCount + "\n" +
			  "redoLayersCount       = " + redoLayersCount   + "\n" +
			  "initializedFirstLayer = " + initializedFirstLayer	;
		result += "\nhasLayerInQueue     = " + hasLayerInQueue()   ;
		result += "\nhasFreeLayers       = " + hasFreeLayers()     ;
		result += "\nhasRedoLayerAbove   = " + hasRedoLayerAbove() ;
		result += "\nhasUndoLayerBelow   = " + hasUndoLayerBelow() ;
			
			
		
			
		for (int i = 0; i < lcArray.Length; i++) {
			string optional="";
			if (i==currentLayerId)
				optional = "<Current>";			
			result+=String.Format("\n[{0,2}] {1,15} {2}",i,lcArray[i].name,optional);
		}
		return result;
	}
}

