using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using localisation;

public delegate IEnumerator CoroutineMethod();

public class MainGameController : MonoBehaviour {
	
	ControllerInterface[] controllers;

	ControllerUpdateInterface[] updateControllers = new ControllerUpdateInterface[]{
		new WrongActionController()
	};

	void Awake ()
	{
#if UNITY_IOS
		Handheld.SetActivityIndicatorStyle(iOSActivityIndicatorStyle.WhiteLarge);
		WorkspaceEventManager.instance.onSavePictureClick += onSavePictureClickListener;
#endif
		initControllers();
	}

	void Start(){
		setInitPicture();
	}

	void Update(){
		for (int i=0; i< updateControllers.Length;i++){
			updateControllers[i].update();
		}
	}
	
	void initControllers(){		
		controllers = new ControllerInterface[]{						
			new ActiveColorButtonController(),
			new ActiveToolController(),
			new UndoRedoController(),
			new ToolButtonsController(),	
			new MainColorSelectController(),
			new WindowPictureController(),
			new WindowInfoController(),			
			new WindowColorPickerController(),
			new WindowStampController(),
			new WindowSoundController(),
			new LanguageController(),
			new SnowThemeController()
		};	
		for (int i=0; i< controllers.Length;i++){
			controllers[i].init();
		}
		for (int i=0; i< updateControllers.Length;i++){
			updateControllers[i].init();
		}
	}


	void setInitPicture(){
		#if UNITY_WEBPLAYER
		string playerPath=Application.srcValue;
		string[] parts = playerPath.Split('?');
		if (parts.Length>1){			
			string pictureName=parts[1];
			string name = pictureName.Substring(7);
			if (pictureName.StartsWith("picture")){		
				foreach(Album album in PropertiesSingleton.instance.albums.album){
					if (album.sheetList == null)
						continue;
					foreach(SheetObject so in album.sheetList.sheetList){						
						if (so.name.Equals (name,StringComparison.InvariantCultureIgnoreCase) ){
							PropertiesSingleton.instance.setSheet(so);		
							return;
						}
					}
				}
			}
		}
		SheetObject[] list = PropertiesSingleton.instance.defultSheetList.sheetList;		
		PropertiesSingleton.instance.setSheet(list[UnityEngine.Random.Range (0,list.Length-1)]);		
		
		#else
		SheetObject[] list = PropertiesSingleton.instance.defultSheetList.sheetList;		
		PropertiesSingleton.instance.setSheet(list[UnityEngine.Random.Range (0,list.Length-1)]);		
		#endif
		
	}

	void onSavePictureClickListener () {
		StartCoroutine(saveScreenShot());
	}

	IEnumerator saveScreenShot(){
		Camera[] cameras = GameObject.FindObjectsOfType(typeof(Camera)) as Camera[];
		LayerMask nothingLayerMask = 0;
		GameState oldState = PropertiesSingleton.instance.gameState;
		PropertiesSingleton.instance.gameState = GameState.SAVE_PIC;
		Dictionary<Camera, Color32> camColors = new Dictionary<Camera, Color32>();
		Dictionary<Camera, LayerMask> camMask = new Dictionary<Camera, LayerMask>();
		foreach(Camera cam in cameras){
			camColors.Add(cam, cam.backgroundColor);
			cam.backgroundColor = PropertiesSingleton.instance.screenShotInProgressColor;
			camMask.Add(cam, cam.cullingMask);
			cam.cullingMask = nothingLayerMask;
		}
		yield return null;
#if UNITY_EDITOR
		string path =   "Temp/" + ScreenshotManager.getName(PropertiesSingleton.instance.activeSheet.name.Localized());
#else
		string path = Application.persistentDataPath + "/" + ScreenshotManager.getName(PropertiesSingleton.instance.activeSheet.name.Localized());
#endif
		Texture2D texture = PropertiesSingleton.instance.canvasWorkspaceController.canvas.getResultTexture();
		System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());
		yield return StartCoroutine(ScreenshotManager.SaveExisting(path));


		yield return new WaitForSeconds(PropertiesSingleton.instance.screenShotTimeout);
		UnityEngine.Object.DestroyImmediate(texture);
		Resources.UnloadUnusedAssets();
		System.GC.Collect();

		yield return null;

		foreach(Camera cam in cameras){
			cam.backgroundColor = camColors[cam];
			cam.cullingMask = camMask[cam];
		}
		PropertiesSingleton.instance.gameState = oldState;

	}
}



