using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class SheetList : ScriptableObject {
	public string nameKey;
	public SheetObject[] sheetList;
}
