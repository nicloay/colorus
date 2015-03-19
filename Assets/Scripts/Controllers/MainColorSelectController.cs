using UnityEngine;
using System.Collections;
using System;

public class MainColorSelectController : ControllerInterface {
	Color32[] historyColors;

	#region ControllerInterface implementation

	public void init () {
		WorkspaceEventManager.instance.onPredefinedColorClick+=onPredefinedColorClickListener;

		historyColors = PropertiesSingleton.instance.colorProperties.colorHistory;
		WorkspaceEventManager.instance.onColorChanged      += onColorChangedListener;
		WorkspaceEventManager.instance.onColorHistoryClick += onColorHistoryClickListener;

		WorkspaceEventManager.instance.onColorPickerClose += onColorPickerCloseListener;

		WorkspaceEventManager.instance.onPipetteSelectedColor += onPipetteSelectedColorListener;

		WorkspaceEventManager.instance.onSelectColor += onSelectColorListener;
		updateMaterials();
	}

	#endregion

	static void disableRandomIfEnabled () {
		PropertiesSingleton.instance.colorProperties.randomEnabled = false;
	}

	void onColorPickerCloseListener(Color32 selectedColor){
		disableRandomIfEnabled ();
		if (WorkspaceEventManager.instance.onSelectColor!=null)
			WorkspaceEventManager.instance.onSelectColor(selectedColor);
	}
	

	void onPredefinedColorClickListener(int id){
		disableRandomIfEnabled ();
		Color32  c = PropertiesSingleton.instance.colorProperties.predefinedColors[id];
		if (WorkspaceEventManager.instance.onSelectColor!=null)
			WorkspaceEventManager.instance.onSelectColor(c);
	}

	void onPipetteSelectedColorListener(Color32 color){
		PropertiesSingleton.instance.colorProperties.randomEnabled = false;
		if (WorkspaceEventManager.instance.onSelectColor!=null)
			WorkspaceEventManager.instance.onSelectColor(color);
	}

#region colorhistory
	void onColorHistoryClickListener (int colorId){
		disableRandomIfEnabled ();
		PropertiesSingleton.instance.colorProperties.randomEnabled = false;
		Color32 clickedColor = historyColors[colorId];
		
		Array.Copy(historyColors,colorId +1, historyColors, colorId,(historyColors.Length-colorId-1));
		
		if (WorkspaceEventManager.instance.onSelectColor!=null)
			WorkspaceEventManager.instance.onSelectColor(clickedColor);
	}
	
	void onColorChangedListener(Color32 newColor, Color32 oldColor){
		Array.Copy(historyColors,0,historyColors,1,(historyColors.Length-1));
		historyColors[0] = oldColor;
	}
#endregion

#region game color change logic
	void onSelectColorListener(Color32 color){
		if (!color.Equals(PropertiesSingleton.instance.colorProperties.activeColor)){
			Color32 oldColor = PropertiesSingleton.instance.colorProperties.activeColor;
			PropertiesSingleton.instance.colorProperties.activeColor = color;
			if (WorkspaceEventManager.instance.onColorChanged!=null)
				WorkspaceEventManager.instance.onColorChanged(color,oldColor);
		}	
		
		updateMaterials();
	}


	static int shaderMainColor = Shader.PropertyToID("_Color");
	static int shaderSecondColor = Shader.PropertyToID("_SecondColor");

	void updateMaterials(){
		PropertiesSingleton.instance.colorProperties.secondColor
			= ColorUtil.getLightReverseColor(PropertiesSingleton.instance.colorProperties.activeColor);

		PropertiesSingleton.instance.cursors.cursorStampMaterial.color = PropertiesSingleton.instance.colorProperties.activeColor;
		PropertiesSingleton.instance.lineRendererMaterial.color = PropertiesSingleton.instance.colorProperties.activeColor;

		PropertiesSingleton.instance.stampCursorMaterial.SetColor(shaderMainColor, PropertiesSingleton.instance.colorProperties.activeColor);
		PropertiesSingleton.instance.stampCursorMaterial.SetColor(shaderSecondColor, PropertiesSingleton.instance.colorProperties.secondColor);
	}
#endregion
}
