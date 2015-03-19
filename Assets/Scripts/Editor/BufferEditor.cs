using UnityEngine;
using System.Collections;
using UnityEditor;
using System;


[CustomEditor(typeof(BufferController))]
public class BufferEditor : Editor {

	public override void OnInspectorGUI(){
		DrawDefaultInspector();
		/*
		BufferController bc=((BufferController) target);
		if (GUILayout.Button("enque element")){
			//bc.enqueBottomLayout();	
		}
		*/
	}
	
	
	void OnSceneGUI(){
		/*
		Handles.color = Color.blue;
		BufferController bc=((BufferController) target);
		String info="";
		LayerController[] lcs=bc.getAllLayers();
		if (lcs==null){
			info="buffer empty";	
		} else {		
			info+="lastInQ="+bc.layerQueue.getLastElement().name+"\n";
			info+="firstInQ="+bc.layerQueue.getFirstElement().name+"\n";
			
			info+="topLayer="+ bc.getTopLayer().name + "\n";
			info+="bufferFull="+bc.layerQueue.isBufferFull() + "\n";
			info+="buferQueueSize="+bc.layerQueue.queSize+"\n";
			info+="offset="+PropertiesSingleton.instance.bufferZOffset+"\n";
			info+="step="+PropertiesSingleton.instance.bufferZStep+"\n";
			foreach(LayerController lc in lcs){
				if (lc!=null){
					Vector3 position=lc.gameObject.transform.position;
					info +=lc.gameObject.name+"  "+position+"\n";	
				} else {
					info +="null\n";
				}
			}
		}
		Vector3 offset=new Vector3(PropertiesSingleton.instance.width,-PropertiesSingleton.instance.height/2,0);
		Handles.Label(bc.transform.position -offset,
                info);
		*/
	}
}
