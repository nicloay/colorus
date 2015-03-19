using UnityEngine;
using UnityEditor;
using System.Collections;
/*
[CustomEditor(typeof(ColorHistoryController))]
public class ColorHistoryControllerEditor : Editor {

	public override void OnInspectorGUI(){
		base.OnInspectorGUI();
		if (GUILayout.Button("allign items"))
			alligntItems();
	}

	void alligntItems ()
	{
		ColorHistoryController chc= (ColorHistoryController)target;
		float size = chc.items[0].getXWidth();
		float offset=0;
		for (int i = 0; i < chc.items.Count; i++) {
			chc.items[i].transform.localPosition =  new Vector3(offset,0,0);
			offset-=size;
		}
	}
}
 */