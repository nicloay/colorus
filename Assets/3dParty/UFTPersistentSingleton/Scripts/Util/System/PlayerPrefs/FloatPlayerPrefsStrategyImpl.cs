using System;
using UnityEngine;


public class FloatPlayerPrefsStrategyImpl : PlayerPrefsStrategyInterface
{
	#region PlayerPrefsStrategyInterface implementation
	public void writeToPlayerPrefs (string key, object obj)
	{
		PlayerPrefs.SetFloat(key,(float)obj);
	}

	public object readFromPlayerPrefs (string key)
	{
		return PlayerPrefs.GetFloat(key);
	}
	#endregion
	
}

