
public class ActiveToolController : ControllerInterface {
	#region ControllerInterface implementation

	public void init () {
		WorkspaceEventManager.instance.onDrawWithinRegionClick += onDrawWithinRegionClickListener;
	}

	#endregion
	void onDrawWithinRegionClickListener(){
		PropertiesSingleton.instance.drawWithinRegion = !PropertiesSingleton.instance.drawWithinRegion;
	}
}
