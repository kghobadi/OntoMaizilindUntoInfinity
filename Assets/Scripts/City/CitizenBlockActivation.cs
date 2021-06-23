using System.Collections;
using System.Collections.Generic;
using NPC;
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
			//activate game obj
			transform.GetChild(i).gameObject.SetActive(true);
			//get movement comp
			Movement npcMover = transform.GetChild(i).GetComponent<Movement>();
			//set movement again!
			npcMover.ResetMovement(npcMover.startBehavior);
			//set idle state again to clear up any issues. 
			npcMover.SetIdle();
			
			yield return new WaitForSeconds(timeBetween);
		}
	}
	
}
