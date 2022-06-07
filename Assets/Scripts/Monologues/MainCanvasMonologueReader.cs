using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls creation and distribution of Monologue Readers on the MainCanvas.
/// Worldspace monologue readers will call to me and generate one when they need it (i.e. when they are not visible). 
/// </summary>
public class MainCanvasMonologueReader : MonoBehaviour
{
	public GameObject mcReaderPrefab;

	public void GenerateReader(MonologueReader worldReader)
	{
		// instantiate mcreader prefab as child of this object
		GameObject mcReader = Instantiate(mcReaderPrefab, transform);
		//get screen reader script
		ScreenReader screenReader = mcReader.GetComponent<ScreenReader>();
		// set mono reader refs (in screen reader and in monoreader)
		screenReader.SetReader(worldReader, worldReader.monoManager.textBack.transform);
		//worldReader.SetScreenReader(screenReader);
		//activate
		screenReader.Activate();
	}
}
