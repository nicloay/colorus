using UnityEngine;
using System.Collections;

public class CanvasCollider : MonoBehaviour {
	
	public static CanvasCollider createCanvasCollider(IntVector2 canvasSize, float zPosition, GameObject parent, CanvasCamera camera){
		Mesh mesh = MeshUtil.createPlaneMesh(canvasSize);
		GameObject go = new GameObject("canvas collider");
		go.transform.parent =parent.transform;
		go.transform.position = new Vector3(0, 0, zPosition);
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;
		MeshCollider mc = go.AddComponent<MeshCollider>();
		mc.sharedMesh = mesh;
		CanvasCollider ccc = go.AddComponent<CanvasCollider>();
		ccc.canvasCamera = camera;
		return ccc;
	}




	static string AXIS_VERTICAL= "Mouse ScrollWheel";
	
	CanvasCamera  canvasCamera;
	IntVector2 pixelPosition;
	Vector3 globalPosition;	
	Vector2 screenPosition;
	void Update () {	
		if (!isActiveForHandleEvents())
			return;	
		
		handleScroll();
		updatePixelAndGlobalPosition ();	
		#if UNITY_IOS
		handleTouch();	
		#else
		handlePCInput();
		#endif
	}
	
	
#if !UNITY_IOS
	
	
	bool leftButtonPressed = false;
	bool midButtonPressed = false;
	bool mouseLeftButtonDown,mouseScrollDown;
	void handlePCInput(){
		if (leftButtonPressed){
			if (Input.GetMouseButtonUp(0) ){
				leftButtonPressed = false;
				if (CanvasController.events.onMouseOverWithButtonDone!=null)
					CanvasController.events.onMouseOverWithButtonDone(pixelPosition);
			}
			if (Input.GetMouseButton(0))
				if (CanvasController.events.onMouseOverWithButton!=null)
					CanvasController.events.onMouseOverWithButton(pixelPosition, globalPosition);				
		}
		if (Input.GetMouseButtonDown(0) && isMouseWithinCameraRect()){
			if (CanvasController.events.onMouseLeftButtonDown!=null)
				CanvasController.events.onMouseLeftButtonDown(pixelPosition);
			leftButtonPressed = true;
		}			 
		
		if (midButtonPressed){
			if (Input.GetMouseButtonUp(2)){
				midButtonPressed = false;
				if (CanvasController.events.onMouseOverWithScrollDone!=null)
					CanvasController.events.onMouseOverWithScrollDone();
			}
			if (Input.GetMouseButton(2))
				if (CanvasController.events.onMouseOverWithScroll!=null)
					CanvasController.events.onMouseOverWithScroll(globalPosition, pixelPosition,screenPosition );				
		}
		
		if (Input.GetMouseButtonDown(2) && isMouseWithinCameraRect()){
			if (CanvasController.events.onMouseScrollDown!=null)
				CanvasController.events.onMouseScrollDown(globalPosition, pixelPosition, screenPosition);
			midButtonPressed = true;
		}
		
	}


	void OnMouseEnter(){	
		if (isActiveForHandleEvents()){
			if (CanvasController.events.onMouseEnter!=null){
				CanvasController.events.onMouseEnter();			
			}
		}
	}
	
	void OnMouseExit(){
		if (isActiveForHandleEvents()){
			if (CanvasController.events.onMouseExit!=null)
				CanvasController.events.onMouseExit();

			if (mouseLeftButtonDown){
				if (CanvasController.events.onMouseOverWithButtonDone!=null)
					CanvasController.events.onMouseOverWithButtonDone(pixelPosition);
				mouseLeftButtonDown = false;
			}
			if (mouseScrollDown){
				mouseScrollDown  = false;
				if (CanvasController.events.onMouseOverWithScrollDone!=null)
					CanvasController.events.onMouseOverWithScrollDone();
			}
			
		}
	}
	
	void OnMouseOver(){
		if (isActiveForHandleEvents()){
			if (CanvasController.events.onWorkspaceMouseOver!=null)
				CanvasController.events.onWorkspaceMouseOver(pixelPosition,globalPosition);
		}
		
	}
	
#else
	
	enum GlobalTouchState{
		PASSIVE,
		ONE_FINGER,
		ONE_FINGER_STATIONARY,
		TWO_FINGERS			
	}
	
	GlobalTouchState touchState = GlobalTouchState.PASSIVE;
	Touch t1,t2;
	
	void handleTouch(){
		if (Input.touchCount == 0 ){	
			finalizeTouch();
			touchState = GlobalTouchState.PASSIVE;
		} else if (Input.touchCount == 1) {
			if (touchState == GlobalTouchState.TWO_FINGERS)
				return;
			t1 = Input.GetTouch(0);
			handleOneTouch(t1);			
		} else if (Input.touchCount == 2) {
			t1 = Input.GetTouch(0);
			if (touchState == GlobalTouchState.ONE_FINGER){
				handleOneTouch(t1);
				return;
			}
			t2 = Input.GetTouch(1);
			//touchState == GlobalTouchState.PASSIVE){//
			//t1.phase == TouchPhase.Began || t2.phase == TouchPhase.Began
			if ( touchState == GlobalTouchState.PASSIVE || touchState == GlobalTouchState.ONE_FINGER_STATIONARY){
				if (CanvasController.events.onDblTouchBegin != null)
					CanvasController.events.onDblTouchBegin(t1, t2);
				touchState = GlobalTouchState.TWO_FINGERS;
			} 
			if (touchState != GlobalTouchState.TWO_FINGERS)
				return;
			if (t1.phase == TouchPhase.Moved || t2.phase == TouchPhase.Moved){
				if (CanvasController.events.onDblTouch !=null)
					CanvasController.events.onDblTouch(t1, t2);
			}
		} 
	}
	
	IntVector2 startPixelPosition;
	bool touchWithinWorkspace = false;
	IntVector2 lastPixelPosition;
	bool lastWithinCamArea;
	void handleOneTouch(Touch t){		
		canvasCamera.getCoordinates(t.position, ref globalPosition, ref pixelPosition);
		
		switch (t.phase){
		case (TouchPhase.Began):
			touchState = GlobalTouchState.ONE_FINGER_STATIONARY;
			if (isMouseWithinCameraRect())
				if (CanvasController.events.onTouchStart !=null)
					CanvasController.events.onTouchStart(pixelPosition, globalPosition);
			break;
		case(TouchPhase.Stationary):
		case (TouchPhase.Moved):		
			if (touchState == GlobalTouchState.ONE_FINGER_STATIONARY){
				if (isMouseWithinCameraRect()){
					if (CanvasController.events.onTouchStart !=null)
						CanvasController.events.onTouchStart(pixelPosition, globalPosition);
					lastPixelPosition = new IntVector2( pixelPosition);
					lastWithinCamArea = true;
				}
			}

			if (isMouseWithinCameraRect()){
				if (lastWithinCamArea){
					if (CanvasController.events.onTouchOver != null)
						CanvasController.events.onTouchOver(pixelPosition, globalPosition);
				} else {
					if (CanvasController.events.onTouchStart != null)
						CanvasController.events.onTouchStart(pixelPosition, globalPosition);
					lastWithinCamArea = true;
				}
				lastPixelPosition = new IntVector2( pixelPosition);
			} else {
				if (lastWithinCamArea){
					finalizeTouch();
				} 
			}
			touchState = GlobalTouchState.ONE_FINGER;
			break;
		case (TouchPhase.Canceled):
		case(TouchPhase.Ended):
			finalizeTouch();
			break;
		}
	}
	
	
	void finalizeTouch(){		
		if (touchWithinWorkspace)
			touchWithinWorkspace = false;
		switch (touchState){
		case GlobalTouchState.ONE_FINGER:			
		case GlobalTouchState.TWO_FINGERS:
			if (CanvasController.events.onTouchEnd!=null)
				CanvasController.events.onTouchEnd();
			break;
		case GlobalTouchState.PASSIVE:
			break;
		}
		touchState = GlobalTouchState.PASSIVE;
	}
#endif
	
	float delta;
	void handleScroll (){			
		delta = Input.GetAxis(AXIS_VERTICAL);			
		if (delta!=0){
			onVerticalScroll(delta);
		}			
	}
	
	
	public bool isActiveForHandleEvents(){	

		return ( GUIUtility.hotControl == 0 && PropertiesSingleton.instance.gameState == GameState.IN_GAME);
	}
	
	void onMouseScrollButtonListenerAction() {	
		if (CanvasController.events.onMouseScrollDown!=null)
			CanvasController.events.onMouseScrollDown (globalPosition, pixelPosition, screenPosition);
	}
	
	
	
	void onVerticalScroll (float delta){						
		if (!isActiveForHandleEvents())
			return;
		if (CanvasController.events.onVerticalMouseScroll!=null)
			CanvasController.events.onVerticalMouseScroll(delta, pixelPosition, globalPosition);
	}
	
	
	
	
	private void updatePixelAndGlobalPosition () {		
#if !UNITY_IPHONE
		screenPosition = Input.mousePosition;
		canvasCamera.getCoordinates(screenPosition, ref globalPosition, ref pixelPosition);
#else
		if (Input.touches.Length > 0) 
			canvasCamera.getCoordinates(Input.touches[0].position, ref globalPosition, ref pixelPosition);		
#endif
	}
	
	public bool isMouseWithinCameraRect(){
		return canvasCamera.camera.pixelRect.Contains(Input.mousePosition);
	}
}
