using System;


public class TriangleSideLineMeshTypeImpl : LineMeshTypeInt {
	#region MeshLineTypeInterface implementation
	public UnityEngine.Mesh GetLineMesh (IntVector2 pointA, IntVector2 pointB, float width,float height) {
		return MeshUtil.createTriangleSideLineMesh(pointA,pointB,width, height);
	}

	public UnityEngine.Mesh GetPointMesh (IntVector2 point, float width, float height) {
		return MeshUtil.createLineMeshPoint (point, width, height);
	}
	#endregion
}
