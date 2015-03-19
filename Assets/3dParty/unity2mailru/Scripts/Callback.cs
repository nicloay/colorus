using System;

public enum CallbackType{
	UNKNOWN,
	DISPOSABLE,
	PERMANENT
}

public class Callback {
	public static int UNSET_HTML_EVENT_ID=-100;
	
	public Action<object,Callback> action;
	public long                    mailruEventId;
	public CallbackType            type;
	
	public long id {
		get {
			return _id;
		}
	}
	
	static long idCounter=0;
	long _id;
	
	public Callback(){
		_id=idCounter++;
		reset();
	}
	
	public void reset(){
		Debug2.LogDebug("reset callback id="+id);
		type        = CallbackType.UNKNOWN;
		mailruEventId = UNSET_HTML_EVENT_ID;
		action      = null;
	}
}
