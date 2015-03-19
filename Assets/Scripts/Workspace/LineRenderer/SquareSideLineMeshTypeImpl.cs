using System;


public class SquareSideLineMeshTypeImpl : LineMeshTypeInt {
	#region MeshLineTypeInterface implementation
	public UnityEngine.Mesh GetLineMesh (IntVector2 pointA, IntVector2 pointB, float width, float height) {
		return MeshUtil.createSquareSideLineMesh(pointA,pointB,width);
	}

	public UnityEngine.Mesh GetPointMesh (IntVector2 point, float width, float height) {
		return MeshUtil.createLineMeshPoint (point, width, height);
	}
	#endregion	
}


