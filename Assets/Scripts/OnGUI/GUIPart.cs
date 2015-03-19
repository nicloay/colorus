using UnityEngine;
public abstract class GUIPart{	
	public abstract void OnGUI();


	//TODO - remove this part, there is skin.findStyle extension
	public static void setStyle(GUISkin skin, string styleName, out GUIStyle style){
		style = skin.FindStyle(styleName);
		if (style == null){
			Debug.LogWarning("cant find "+styleName+" style, will use button one");
			style = new GUIStyle();
		}		
	}
		
}