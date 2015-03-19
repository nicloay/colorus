using UnityEngine;
using System.Collections;

public class SnowThemeController : ControllerInterface {
	WinterGenerator wg;
	#region ControllerInterface implementation

	public void init () {
		ScreenSizeController.onResolutionChange += onResolutionChangeListener;
		wg = PropertiesSingleton.instance.winterGenerator;
	}

	#endregion

	void onResolutionChangeListener(IntVector2 resolution, Vector2 scale){
		wg.cam.orthographicSize = (float)resolution.y /2;
		//wg.camera.aspect = (float)resolution.x / (float)resolution.y;
		generateWinter();
	}

	void generateWinter(){
		wg.generateSnowDrifts();
		wg.snowFall.GenerateSnowFall();
	}


}
