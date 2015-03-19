using UnityEngine;
using System.Collections;

public static class CameraExtension {

	public static float getOrthographicHorizontalSize(this Camera camera){
		return camera.orthographicSize * camera.aspect;
	}

}
