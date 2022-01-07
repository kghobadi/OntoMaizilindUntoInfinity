using System.Collections;
using System.Collections.Generic;
using NPC;
using UnityEngine;

/// <summary>
/// Staggers  activation of the citizens in the city so the NavMesh doesn't get overloaded. 
/// </summary>
public class CitizenBlockActivation : MonoBehaviour
{
	public Movement[] npcCitizens;
	void Awake()
	{
		npcCitizens = GetComponentsInChildren<Movement>();
	}
	
	public void ActivateAll()
	{
		//deactivate each person in the street group
		for (int i = 0; i < npcCitizens.Length; i++)
		{
			npcCitizens[i].gameObject.SetActive(true);
		}
	}
	
	public void DeactivateAll()
	{
		//deactivate each person in the street group
		for (int i = 0; i < npcCitizens.Length; i++)
		{
			npcCitizens[i].gameObject.SetActive(false);
		}
	}
	
	public void BeginActivation(float timeBetween)
	{
		StartCoroutine(ActivateCitizensStaggered(timeBetween));
	}

	IEnumerator ActivateCitizensStaggered(float timeBetween)
	{
		//deactivate each person in the street group
		for (int i = 0; i < npcCitizens.Length; i++)
		{
			//activate game obj
			npcCitizens[i].gameObject.SetActive(true);
			//set movement again!
			npcCitizens[i].ResetMovement(npcCitizens[i].startBehavior);
			//set idle state again to clear up any issues. 
			npcCitizens[i].SetIdle();
			
			yield return new WaitForSeconds(timeBetween);
		}
	}
	
}
