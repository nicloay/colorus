namespace colorpicker{	
	public class HSVColor{
		public float h;
		public float s;
		public float v;
		public HSVColor(){
			this.h = 0.0f;
			this.s = 0.0f;
			this.v = 0.0f;
		}
		
		public HSVColor(float h, float s, float v){
			this.h = h;
			this.s = s;
			this.v = v;
		}
		
		public override string ToString(){
			return "h:"+h.ToString()+" s:"+s.ToString()+" v:"+v.ToString();
		}
	}
}