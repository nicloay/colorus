using UnityEngine;
using System.Collections;
using System;



public delegate void ActiveLayerReceivedTexture(LayerController activeLayerController);

public delegate void OnWorkspaceMouseOver (IntVector2 pixelCursorPosition, Vector3 globalCursorPosition);


public delegate void OnVerticalMouseScroll(float delta, IntVector2 pixelPosition, Vector2 globalPosition);

public delegate void OnCameraChangePosition(float zoomHRatio, float zoomVRatio, Vector2 position, float newSize);
public delegate void OnCameraChangeLocation(Vector2 location);

public delegate void OnScrollBarsClick(Vector2 position);
public delegate void OnToolChange(ToolType tool);

public delegate void OnActiveColorChange(Color32 newColor, Color32 oldColor);
public delegate void OnRandomColorEnable(bool status);
public delegate void OnColorClick(Color32 color, bool disableRandomColor=true);


public class WorkspaceEventManager : MonoSingleton<WorkspaceEventManager> {
	public Action<IntVector2>         onMouseLeftButtonDown        ;
	public Action<IntVector2>         onMouseScrollDown            ;
	
	public Action<IntVector2>         onMouseOverWithButton        ;
	public Action<IntVector2>         onMouseOverWithScroll        ;
	
	public Action<IntVector2>         onMouseOverWithButtonDone    ;
	public Action                     onMouseOverWithScrollDone    ;
	
	public ActiveLayerReceivedTexture onActiveLayerReceivedTexture ;
	
	
	
	public Action                     onMouseEnter                 ;
	public Action                     onMouseExit                  ;
	public OnWorkspaceMouseOver       onWorkspaceMouseOver         ;
	
	
	public Action<SheetObject>        onPicIconeClick              ;
	public Action                     onActivePicIconClick         ;
	public Action                     onStampWindowOpen            ;	
	public OnVerticalMouseScroll      onVerticalMouseScroll        ;
	public Action<String>             onWrongAction                ;
	public OnToolChange               onToolChange                 ;
	
	public OnCameraChangePosition     onCameraChangePosition       ;
	public OnCameraChangeLocation     onCameraChangeLocation       ;
	
	public OnActiveColorChange        onActiveColorChange          ;
	public OnRandomColorEnable        onRandomColorEnable          ;
	public OnColorClick               onColorClick                 ;
	
	public Action                     onReflashConfirm             ;
	
	
	
	public Action                     onMouseEnterScrollbar        ;
	public Action                     onMouseExitScrollbar         ;
	
	
	//new events start here
	public Action<SheetList>          onAlbumClick                 ;
	public Action<SheetObject>        onSheetChange                ;
	public Action<int>                onColorHistoryClick          ;
	public Action<int>                onPredefinedColorClick       ;
	public Action                     onPalleteOpenWindowClick     ;
	public Action<Color32>            onColorPickerClose           ;
	public Action                     onRandomActiveColorClick     ;
	public Action                     onDrawWithinRegionClick      ;
	public Action                     onStampSelectButtonClick     ;	
	public Action                     onMenuNewPictureClick        ;
	public Action<int>                onSelectAlbum                ;
	public Action<int>                onSelectPicture              ;
	public Action<Color32>            onSelectColor                ;
	public Action<Color32,Color32>    onColorChanged               ; //new color , oldColor
	
	public Action                     onExitByEscFromColorPicker   ;
	public Action                     onExitFromStampChooserWindow ;
	public Action                     onExitFromPicChooserWindow   ;
	public Action                     onExitFromInfoWindow         ;
	
	public Action                     onUndoClick                  ;
	public Action                     onRedoClick                  ;
	public Action                     onResetSheetClick            ;
	public Action                     onSavePictureClick           ;
	public Action<ToolType>           onToolButtonClick            ;
	public Action                     onInfoButtonClick            ;
	public Action<Touch, Touch>       onTouchBegin;
	public Action<Touch, Touch>       onTouch;
	public Action                     onTouchEnd;
	
	public Action<Color32>            onPipetteSelectedColor       ;

	public Action<int>                onStampClickInWindow         ;


	public Action                     onSoundButtonClick           ;
	public Action                     onExitFromSoundSettingsWindow;
	public Action                     onSoundWindowMuteClick       ;
	public Action                     onSoundWindowMusicMuteClick  ;
	public Action                     onSoundWindowSFXMuteClick    ;
	public Action<float>              onSoundWindowMusicLevelChange;
	public Action<float>              onSoundWindowSFXLevelChange  ;
}
