// by @torahhorse

using UnityEngine;
using System.Collections;

public class CameraFadeOnStart : MonoBehaviour
{
	public bool fadeInWhenSceneStarts = true;
	public bool WaitUntilGI = true;
	public Color fadeColor = Color.black;
	public float fadeTime = 5f;
	private bool GILoaded = false;
	private bool updateFunction = true;
	private GameObject fade;

	void Start ()
	{
		fade = GameObject.Find("CameraFade");
		if (fade == null){
			CameraFade.SetTemporaryScreen (fadeColor);
			print ("black screen");
			if (fadeInWhenSceneStarts && !WaitUntilGI)
			{
				print ("startfade");
				Fade();
			}
		}

	}
	
	public void Fade()
	{
		print ("fading");
		CameraFade.StartAlphaFade(fadeColor, true, fadeTime, 0, () =>  IsFading());
	}

	public void IsFading(){
	}

	void Update(){
		if (updateFunction && WaitUntilGI) {
			if (DynamicGI.isConverged) {
				print ("gi is converged");
				GILoaded = true;
				Fade();
				updateFunction = false;
			}
		}
	}

	void OnEnable (){
		Start ();
	}
}
