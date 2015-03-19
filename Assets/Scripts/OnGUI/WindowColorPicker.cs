using UnityEngine;
using System.Collections;
using System;
using colorpicker;
using localisation;

[Serializable]
public class ColorPickerWindowConfig{	
	public int height = 250;	
	public int width = 250;
	public int sliderPaletteInterval = 10;
	public int sliderWidth = 37;
	public string windowCaption = "select color";
}


[Serializable]
public class WindowColorPicker : GUIPart {	
	public ColorPickerWindowConfig config;
	public GuiModalWindow          window;
	Rect contentWindowRect;
	Rect paletteRect;
	Rect sliderRect;
	ColorPicker colorPicker;
	
	public void setRightDownPosition(int x, int y, GUISkin skin){
		int windowWidth = config.width + config.sliderPaletteInterval + config.sliderWidth;
		
		contentWindowRect  = new Rect(x - windowWidth,
				       y - config.height,
					windowWidth,
					config.height);
		
		paletteRect = new Rect(0,0, config.width, config.height);
		sliderRect = new Rect(config.width + config.sliderPaletteInterval,
				      0,
				      config.sliderWidth,
				      config.height);
		
		if (colorPicker == null)
			colorPicker = new ColorPicker();
		colorPicker.initialize(paletteRect,sliderRect);
		
		window.setProperties(contentWindowRect, new GUIContent(config.windowCaption.Localized()), skin, doMyWindow, 
			closeWindow,
			closeWindow);
		
	}

	public override void OnGUI (){		
		window.OnGUI(new GUIContent(config.windowCaption.Localized()));		
	}

	public void doMyWindow ()
	{
		colorPicker.OnGUI();				
	}
	
	public void closeWindow(){
		if (WorkspaceEventManager.instance.onColorPickerClose != null)
			WorkspaceEventManager.instance.onColorPickerClose(colorPicker.getRGB());
	}

	public void setActiveColor(){
		colorPicker.setColor(PropertiesSingleton.instance.colorProperties.activeColor);
	}
	
}
