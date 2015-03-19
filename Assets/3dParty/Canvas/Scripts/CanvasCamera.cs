using UnityEngine;
using System.Collections;


public class CanvasCameraRelativePosition{
	public float camMinLeft;
	public float camMaxRight;
	
	public float camMinBottom;
	public float camMaxTop;
	
	public float zoomRatio;
}

public class CanvasCamera {
	public Camera camera;
	public CanvasCameraRelativePosition camRelativePosition;

	public bool canZoomIn{
		get{
			return camera.orthographicSize > minSize;
		}
	}

	public bool canZoomOut{
		get{
			return camera.orthographicSize < maxSize;
		}
	}

	public Vector3 globalPosition{
		get{
			return camera.transform.position;
		}
	}

	public float orthographicSize{
		get{
			return camera.orthographicSize;
		}
	}

	public Vector2 cameraExtents{
		get{
			return _cameraExtents;
		}
	}

	public Vector2 cameraSize{
		get{
			return _cameraSize;
		}
	}

	public Vector2 cameraGlobalBottomLeftCornerPosition{
		get {
			Vector3 result = globalPosition;
			result.x      -= cameraExtents.x;
			result.y      -= cameraExtents.y;
			return result;
		} 
		set {

			camera.transform.position = new Vector3(value.x + cameraExtents.x, value.y + cameraExtents.y, camera.transform.position.z);
			fixCameraOverlapCanvasBounds();
		}
	}


	GameObject go;
	float minSize;
	float maxSize;
	Vector2 _cameraExtents;
	Vector2 _cameraSize;
	Vector2 canvasSize;
	float   canvasAspect;
	Vector2 canvasExtents;
	public CanvasCamera(CanvasCameraConfig camConfig, IntVector2 canvasSize, GameObject parent){
		camRelativePosition = new CanvasCameraRelativePosition();
		go = new GameObject("canvas camera");
		go.transform.parent = parent.transform;
		go.transform.position = Vector3.zero;
		camera = go.AddComponent<Camera>();
		camera.orthographic = true;
		this.canvasSize = canvasSize.toVector2();
		updateCameraConfig(camConfig);
		updateCameraParameters(camConfig.screenPixelRect, canvasSize);
	}

	public void updateCameraConfig(CanvasCameraConfig camConfig){
		camera.backgroundColor = camConfig.bgColor;
		camera.depth = camConfig.cameraDepth;
		minSize = camConfig.camMinSize;
		camera.backgroundColor = camConfig.bgColor;
		camera.depth = camConfig.cameraDepth;
		updateCameraParameters(camConfig.screenPixelRect, canvasSize);
	}

	public void updateCameraParameters(Rect screenRect, IntVector2 canvasSize){
		updateCameraParameters(screenRect, canvasSize.toVector2());
	}

	public void updateCameraParameters(Rect screenRect, Vector2 canvasSize){

		canvasAspect = (float)canvasSize.x / (float)canvasSize.y;
		canvasExtents = new Vector2(canvasSize.x / 2 , canvasSize.y / 2);
		camera.aspect =  screenRect.width / screenRect.height ;
		camera.pixelRect = screenRect;
		maxSize = canvasAspect > camera.aspect ? canvasExtents.x / camera.aspect : canvasExtents.y;
		camera.orthographicSize = maxSize;
		fixCameraOverlapCanvasBounds();
	}



	public void zoom(float amount){
		camera.orthographicSize = Mathf.Clamp( camera.orthographicSize+amount, minSize, maxSize);
		fixCameraOverlapCanvasBounds();
	}


	public void zoom(float amount, IntVector2 pixelPosition, Vector3 globalPosition){			
		Vector2 screenCoords = camera.WorldToScreenPoint(globalPosition);
		camera.orthographicSize = Mathf.Clamp( camera.orthographicSize+amount, minSize, maxSize);
		Vector3 newGlobalPosition = camera.ScreenToWorldPoint(screenCoords);
		Vector2 diff = newGlobalPosition - globalPosition;
		camera.transform.position -= (Vector3)diff;
		fixCameraOverlapCanvasBounds();
	}

	float fixedX, fixedY;
	Vector3 camPosition;
	void fixCameraOverlapCanvasBounds(){
		updateCameraRelativeData();
		camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minSize, _camMaxSize);
		fixOverlapHorizontal(ref fixedX);
		fixOverlapVertical(ref fixedY);
		camPosition = camera.transform.position;
		camPosition.x = fixedX;
		camPosition.y = fixedY;
		camera.transform.position = camPosition;
	}

	void fixOverlapHorizontal(ref float fixedValue){
		if (cameraSize.x > canvasSize.x){
			fixedValue = 0;
		} else {
			fixedValue = Mathf.Clamp(camera.transform.position.x, 
			                         -canvasExtents.x + _cameraExtents.x,
			                         canvasExtents.x - _cameraExtents.x);
		}
	}

	void fixOverlapVertical(ref float fixedValue){
		if (cameraSize.y > canvasSize.y){
			fixedValue = 0;
		} else {
			fixedValue = Mathf.Clamp(camera.transform.position.y, 
			                         -canvasExtents.y + _cameraExtents.y,
			                         canvasExtents.y - _cameraExtents.y);
		}
	}
	
	public Vector3 screenPointToWorld(Vector2 point){
		return camera.ScreenToWorldPoint(point);
	}

	Vector3 offset;

	public void syncScreenPointWithWorld(Vector2 point, Vector3 worldPoint){
		offset = worldPoint - camera.ScreenToWorldPoint(point);
		camera.transform.position+=offset;

		fixCameraOverlapCanvasBounds();
	}
	
	IntVector2 tmpIV2;
	Vector3 firstCenterPoint, newCenterPoint;
	public void syncTwoPointsToWorld(Vector2 point1, Vector2 initScreenPoint1, Vector3 initWorldPoint1, 
	                                 Vector2 point2, Vector2 initScreenPoint2, Vector3 initWorldPoint2, 
	                                 float cameraStartSize){


		// sync camera size
		if (initWorldPoint1 != initWorldPoint2){
			float zoomRatio = Vector2.Distance (point1, point2)/  Vector2.Distance (initScreenPoint1, initScreenPoint2);
			camera.orthographicSize = Mathf.Clamp( cameraStartSize / zoomRatio, minSize, maxSize);
		}

		// sync position
		Vector2 screenCenterPoint = Vector2.Lerp(point1, point2, 0.5f);
		getCoordinates(screenCenterPoint, ref newCenterPoint, ref tmpIV2);
		Vector3 diff = newCenterPoint - Vector3.Lerp(initWorldPoint1, initWorldPoint2, 0.5f);
		camera.transform.position -= diff;
		fixCameraOverlapCanvasBounds();
	}


	public Vector3 getGlobalPosition(){
		return camera.transform.position;
	}

	public void setGlobalPosition(Vector3 v){
		camera.transform.position = v;
		fixCameraOverlapCanvasBounds();
	}
	
	float _camMaxSize;
	public void updateCameraRelativeData(){
		//canvasAspect;//normal aspect;
		//canvasSize;//canvasSize;

		_cameraExtents.x = camera.orthographicSize * camera.aspect;
		_cameraExtents.y = camera.orthographicSize;
		_cameraSize.x = _cameraExtents.x * 2;
		_cameraSize.y = _cameraExtents.y * 2;
		if (canvasAspect > camera.aspect){
			//canvas = [ ] camera = []
			maxSize = canvasExtents.x / camera.aspect;
			_camMaxSize = maxSize;
			camRelativePosition.camMinLeft = -canvasExtents.x + _cameraExtents.x;
			camRelativePosition.camMaxRight = -camRelativePosition.camMinLeft;

			float maxY = canvasExtents.y / camera.aspect;
			camRelativePosition.camMinBottom = -maxY + camera.orthographicSize;
			camRelativePosition.camMaxTop = camRelativePosition.camMinBottom;
			camRelativePosition.zoomRatio = _cameraExtents.x / canvasExtents.x;
		} else {
			//canvas = [ ] camera = [  ]
			maxSize = canvasExtents.y;
			_camMaxSize = canvasExtents.y;
			camRelativePosition.camMinLeft = -canvasExtents.y * camera.aspect;// + _cameraExtents.x;
			camRelativePosition.camMaxRight = - camRelativePosition.camMinLeft;

			camRelativePosition.camMinBottom = -canvasExtents.y + camera.orthographicSize;
			camRelativePosition.camMaxTop = camRelativePosition.camMinBottom;

			camRelativePosition.zoomRatio = (camera.orthographicSize - minSize) / (maxSize - minSize);
		}

	}

	public void updateAspect(float aspect){
		camera.aspect = aspect;
		fixCameraOverlapCanvasBounds();
	}


	bool statusOk;
	RaycastHit hit;
	Vector2 pixelUV;
	public bool getCoordinates(Vector2 screenCoordinates, ref Vector3 globalPosition, ref IntVector2 pixelPosition ){
		
		statusOk = Physics.Raycast (camera.ScreenPointToRay (screenCoordinates), out hit);
		if (!statusOk) {
			return false;
		}

		globalPosition = hit.point;				
		pixelUV = hit.textureCoord;
		
		pixelUV.x = pixelUV.x * canvasSize.x;
		pixelUV.y = pixelUV.y * canvasSize.y;
		pixelPosition = new IntVector2 (pixelUV);
		return true;
	}


#region ManualDragin camera through code

	public void doScrolling(){
		
	}
#endregion
}

