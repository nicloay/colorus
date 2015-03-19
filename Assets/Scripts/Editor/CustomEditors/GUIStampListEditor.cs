using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GUIStampList))]
public class GUIStampListEditor : Editor {
	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		if (GUILayout.Button("set paths")){
			GUIStampList sl = (GUIStampList)target;
			foreach (GUIStamp stamp in sl.stampList) {
				stamp.iconPath = stamp.iconTexture.name;
				stamp.stampPath = stamp.stampTexture.name;
			}
		}
	}
}
