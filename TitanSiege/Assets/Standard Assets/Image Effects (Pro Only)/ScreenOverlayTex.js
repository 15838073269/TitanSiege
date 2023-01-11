
#pragma strict

@script ExecuteInEditMode
@script RequireComponent (Camera)
@script AddComponentMenu ("Image Effects/Screen Overlay")

class ScreenOverlayTex extends PostEffectsBase {
	
	enum OverlayBlendMode {
		Additive = 0,
		ScreenBlend = 1,
		Multiply = 2,
        Overlay = 3,
        AlphaBlend = 4,	
	}
	
	public var blendMode : OverlayBlendMode = OverlayBlendMode.Overlay;
	public var intensity : float = 1.0f;
	public var texture0 : Texture2D;
	public var texture1 : Texture2D;
	public var texture2 : Texture2D;

	public var index : int = 0;
			
	public var overlayShader : Shader;
	
	private var overlayMaterial : Material = null;
	
	function CheckResources () : boolean {
		CheckSupport (false);
		
		overlayMaterial = CheckShaderAndCreateMaterial (overlayShader, overlayMaterial);
		
		if(!isSupported)
			ReportAutoDisable ();
		return isSupported;
	}
	
	function OnRenderImage (source : RenderTexture, destination : RenderTexture) {		
		if(CheckResources()==false || index == 0) {
			Graphics.Blit (source, destination);
			return;
		}
		overlayMaterial.SetFloat ("_Intensity", intensity);

		if (index == 1)
			overlayMaterial.SetTexture ("_Overlay", texture0);
		else if (index == 2)
			overlayMaterial.SetTexture ("_Overlay", texture1);
		else if (index == 3)
			overlayMaterial.SetTexture ("_Overlay", texture2);

		Graphics.Blit (source, destination, overlayMaterial, blendMode);
	}
}