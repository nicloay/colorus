using System;
using UnityEngine;
using System.Collections;

public delegate void OnResolutionChange(IntVector2 resolution, Vector2 scale);

public class ScreenSizeController : MonoBehaviour {
	public static OnResolutionChange onResolutionChange;
	public int lastHeight=0;
	public int lastWidth=0;	
	public bool lastFullScreen = false;
	

	public void setNewSize(int width,int height){
		Screen.SetResolution (width,height,false);		
		Update();
	}
	
	void Update () {	
		
		// have to work with resolution here not width/height
		if ((Screen.width != lastWidth || Screen.height != lastHeight || Screen.fullScreen != lastFullScreen) && onResolutionChange != null) {						
			if (Screen.fullScreen && lastFullScreen)
				return;
			
			setResolutionToActualSizeAndSendEvent();
		}
	}

	void setResolutionToActualSizeAndSendEvent () {				
		Debug2.LogDebug("setResolutionToActualSizeAndSendEvent current resolution	" + Screen.currentResolution);
		lastWidth = Screen.width;
		lastHeight = Screen.height;
		lastFullScreen = Screen.fullScreen;
		if (Screen.width < PropertiesSingleton.instance.screen.minWidth || Screen.height < PropertiesSingleton.instance.screen.minHeight){
			scaleScreenAndSendEvent ();
		}else{
			IntVector2 resolution;
			if(Screen.fullScreen){
				Resolution maxRes= Screen.resolutions[Screen.resolutions.Length-1];								
				Screen.SetResolution (maxRes.width,maxRes.height,true);
				Debug2.LogDebug("trying to change to full screen");	
				resolution = new IntVector2(maxRes.width, maxRes.height);
			} else {
				Screen.SetResolution(Screen.width,Screen.height,false);
				resolution=new IntVector2 (Screen.width,Screen.height);				
			}
			sendEventOnResolutionChange (resolution, new Vector2(1.0f, 1.0f));
		}
	}

	void scaleScreenAndSendEvent () {
		Debug2.LogDebug("scaleScreenAndSendEvent ");
		float ratio;
		Resolution maxRes = new Resolution();
		if (Screen.fullScreen) {
			maxRes = Screen.resolutions [Screen.resolutions.Length - 1];
			ratio = (float)maxRes.width / (float)maxRes.height;
		} else
			ratio = (float)Screen.width / (float)Screen.height;

		float normalRatio = PropertiesSingleton.instance.screen.normalAspect;
		IntVector2 newResolution;
		if (ratio < normalRatio){
			float newWidth= ratio * (float)PropertiesSingleton.instance.screen.minHeight;
			float newHeight = PropertiesSingleton.instance.screen.minHeight;
			if (newWidth < PropertiesSingleton.instance.screen.minWidth){
				newWidth = PropertiesSingleton.instance.screen.minWidth;
				newHeight = PropertiesSingleton.instance.screen.minWidth / ratio; 
			}			
			newResolution = new IntVector2 (newWidth,newHeight);			
		} else {
			float newWidth = PropertiesSingleton.instance.screen.minWidth;
			float newHeight = PropertiesSingleton.instance.screen.minWidth / ratio;
			if (newHeight < PropertiesSingleton.instance.screen.minHeight){
				newHeight = PropertiesSingleton.instance.screen.minHeight;
				newWidth  = newHeight * ratio;
			}			
			newResolution = new IntVector2 (newWidth, newHeight);			
		}
		Vector2 scale; 
		if (Screen.fullScreen)
			scale = new Vector2 ( (float)maxRes.width  / (float)newResolution.x,
					      (float)maxRes.height / (float)newResolution.y);
		else
			scale = new Vector2 ( (float)Screen.width  / (float)newResolution.x,
					      (float)Screen.height / (float)newResolution.y);
		Screen.SetResolution (newResolution.x, newResolution.y, Screen.fullScreen);
		sendEventOnResolutionChange(newResolution,scale);
	}
	
	void sendEventOnResolutionChange(IntVector2 resolution, Vector2 scale){
		Debug2.LogDebug("sendEventOnResolutionChange  resolution = "+resolution.ToString()+"   scale ="+scale);
		if (onResolutionChange!=null)
			onResolutionChange(resolution,scale);
	}
}
