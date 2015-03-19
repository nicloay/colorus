using System;
using UnityEngine;


public interface EditorConstructionInterface
{
	// in this method you will create subobjects and so on
	void initialize();
	
	// call this method when you want to destroy object created in initialize()
	void reset();
}


