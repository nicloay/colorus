using System;


public class PlayerPrefsContext
{
	private PlayerPrefsStrategyInterface playerPrefsStrategyInterface;
	
	
	
	public PlayerPrefsContext (PlayerPrefsStrategyInterface playerPrefsStrategyInterface)
	{
		this.playerPrefsStrategyInterface = playerPrefsStrategyInterface;
	}

	
	
	public void writeToPlayerPrefs (string key, object obj)
	{
		playerPrefsStrategyInterface.writeToPlayerPrefs(key,obj);	
	}

	public object readFromPlayerPrefs (string key)
	{
		return playerPrefsStrategyInterface.readFromPlayerPrefs(key);
	}	
}


