using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomber : MonoBehaviour {

    public GameObject bombPrefab;

	void Start () {
		
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(SpawnBombs());
        }
	}

    //spawn a random count of bombs to drop
    IEnumerator SpawnBombs()
    {
        int randomBcount = Random.Range(2, 5);

        //Debug.Log("Spawning " + randomBcount + " bombs");

        for(int i = 0; i < randomBcount; i++)
        {
            Vector3 spawnPos = transform.position - new Vector3(0, 7, 0) + Random.insideUnitSphere * 5;
            GameObject bomb = Instantiate(bombPrefab,spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(0.5f);
        }
    }
}
