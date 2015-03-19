using System;


public interface PlayerPrefsStrategyInterface
{
	void writeToPlayerPrefs(string key, object obj);
	object readFromPlayerPrefs(string key);
}


