using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Staggers  activation of the citizens in the city so the NavMesh doesn't get overloaded. 
/// </summary>
public class CitizenBlockActivation : MonoBehaviour
{

	public void DeactivateAll()
	{
		//deactivate each person in the street group
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}
	}

	public void BeginActivation(float timeBetween)
	{
		StartCoroutine(ActivateCitizensStaggered(timeBetween));
	}

	IEnumerator ActivateCitizensStaggered(float timeBetween)
	{
		//deactivate each person in the street group
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(true);
			
			yield return new WaitForSeconds(timeBetween);
		}
	}
	
}
