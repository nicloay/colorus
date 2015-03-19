using System;
using UnityEngine;


public enum LineMeshType{
	SQUARE_SIDES,
	TRIANGLE_SIDES
}

public interface LineMeshTypeInt {
	Mesh GetLineMesh(IntVector2 pointA, IntVector2 pointB, float width, float height);
	Mesh GetPointMesh(IntVector2 point, float width, float height);
}


