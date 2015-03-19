using UnityEngine;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BufferController : MonoBehaviour, EditorConstructionInterface {
	public static string BUFER_LAYER_SHADER_NAME="Custom/BufferLayerShader";
	public LayerQueue layerQueue;
	
	 
	
	public void initialize ()
	{
		updateBufferLayers();
	}

	public void reset ()
	{
		destroyLayersGO();
	}
	
	
	void Awake(){
		//initializeBufferQueue();	
	}	
	
	
	public LayerController getTopLayer(){
		return layerQueue.getCurrentLayer();	
	}
	
	
	
	
	public LayerController[] getAllLayers(){
		if (layerQueue==null)
			return null;
		return layerQueue.getLayerQueueArray();	
	}
	
	public void incrementZpositionForAll ()
	{	
		LayerController[] lcs=layerQueue.getVisibleLayersCurrentLayerFirst();		
		for (int i = 0; i < lcs.Length; i++) {			
			lcs[i].transform.position += new Vector3(0.0f,0.0f, PropertiesSingleton.instance.bufferZStep);			
		}		
	}
	
	public LayerController[] getVisibleLayersCurrentLayerFirst(){
		return layerQueue.getVisibleLayersCurrentLayerFirst();	
	}
	
	
	
	public void initializeBufferQueue(Color32[] tmpArray){
		layerQueue = new LayerQueue(gameObject.GetComponentsInChildren<LayerController>());
		clearAndDeactivateAllLayers(tmpArray);	
	}
	
	private void clearAndDeactivateAllLayers(Color32[] tmpArray){
		ColorUtil.clearPixels(tmpArray);
		foreach (LayerController lc in layerQueue.getLayerQueueArray()){
			lc.fastClearTexture(tmpArray);
			lc.deactivate();
		}
	}
	
	
#region newMethods
	public void undoCurrentLayer(){
		layerQueue.getCurrentLayer().deactivate();
		layerQueue.setCurrentLayerBelow();
	}
	
	public LayerController setCurrentLayerAbove(){
		return layerQueue.setCurrentLayerAbove();		
	}
	
	public LayerController fetchWorkingLayer(){
		return layerQueue.fetchFreeLayer();	
	}
	
	public LayerController getDownLayer(){
		return layerQueue.getDownLayer();	
	}
	
	public void releaseDownLayer(){
		layerQueue.releaseDownLoayer();
	}
	
	public bool hasFreeLayers(){
		return layerQueue.hasFreeLayers();	
	}
	
#endregion
	
	private void updateBufferLayers(){
		if (transform.childCount!=PropertiesSingleton.instance.bufferCount){
			destroyLayersGO ();
			
			for (int i=0;i<PropertiesSingleton.instance.bufferCount;i++){
				string name="layer"+i;
				Vector3 position = PropertiesSingleton.getLayerZPosition (i);
				
				GameObject go=LayerUtil.createBufferLayer (BUFER_LAYER_SHADER_NAME, name, position,transform);
				go.AddComponent<LayerController>();
			}
			
		}
	}
	
	private void destroyLayersGO ()
	{		
		for (int i=0;i<transform.childCount;i++){
			DestroyImmediate( transform.GetChild(i).gameObject) ;	
		}
	}
#if UNITY_EDITOR

	Vector2 scrollPosition;
	void OnDrawGizmos(){
		if (!Application.isPlaying)
			return;
		string[] status = layerQueue.getFullStatus().Split('\n');

		int height = 500;
		int width = 200;
		int y = Screen.height - height - 20;
		int rowHeight = 20;

		Handles.BeginGUI();
		GUI.BeginGroup(new Rect(10, y, width, height - 25),"BufferController", GUI.skin.window);

		scrollPosition = GUI.BeginScrollView(new Rect(5, 20, width - 10, height-50), scrollPosition, new Rect(0,0, width -25, rowHeight * status.Length));
		for (int i = 0; i < status.Length; i++) {
			GUI.Label(new Rect(0,rowHeight * i, width - 25, rowHeight), status[i]);
		}
		GUI.EndScrollView();
		GUI.EndGroup();
		Handles.EndGUI();
	}
#endif

}
