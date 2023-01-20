using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitySpawn: MonoBehaviour
{
    public List<GameObject> buildings;
    public Transform cityParent;
    public Quaternion rotation = Quaternion.Euler(0, 180, 0);
    public float threshold = -300;
    public float childNumber;
	Mesh originalMesh;

    public Material[] materials;


	private void Reset()
	{

		print("reset");
		buildings.Add(Resources.Load("Buildings/building 1") as GameObject);
		cityParent = transform.parent.parent.FindChild("CITY");

		foreach (Transform child in cityParent)
		{
			DestroyImmediate(child.gameObject);
		}


		//buildings = Resources.LoadAll("Buildings", typeof(GameObject)) as GameObject[];


		Mesh mesh = GetComponent<MeshFilter>().mesh;
		originalMesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;

		int i = 0;
		while (i < vertices.Length)
		{
			vertices[i] += Vector3.up * Time.deltaTime;
			i++;
		}
		mesh.vertices = vertices;
		mesh.RecalculateBounds();

		var matrix = transform.localToWorldMatrix;

		for (int vertId = 0; vertId < vertices.Length; vertId++)
		{
			//print(vertices[vertId].z);
			if (vertices[vertId].z > threshold)
			{
				GameObject building;
				building = Instantiate(buildings[Random.Range(0,buildings.Count)], matrix.MultiplyPoint3x4(vertices[vertId]), rotation, cityParent);
				building.transform.localScale = new Vector3(building.transform.localScale.x, Random.Range(building.transform.localScale.y, building.transform.localScale.y * 1.5f), building.transform.localScale.z);
			}
		}
		GetComponent<MeshFilter>().mesh = originalMesh;
	}


	//private void Update()
 //   {
 //       if (update && !Spawning)
 //       {
 //           if (Inside)
 //           {
 //               if (loop != null)
 //               {
 //                   StopCoroutine(loop);
 //               }
 //               loop = StartCoroutine(Loop(true));
 //               update = false;
 //           }
 //           else
 //           {
 //               if (loop != null)
 //               {
 //                   StopCoroutine(loop);
 //               }
 //               loop = StartCoroutine(Loop(false));
 //               update = false;
 //           }
 //       }
 //   }

 //   private void OnTriggerEnter(Collider other)
 //   {
 //       if (other.CompareTag("Player"))
 //       {
 //           Inside = true;
 //           update = true;
 //           if (!FinishedSpawning && !Spawning)
 //           {
 //               StartCoroutine(FirstSpawn());
 //           }
 //       }
 //   }

 //   private void OnTriggerExit(Collider other)
 //   {
 //       if (other.CompareTag("Player"))
 //       {
 //           Inside = false;
 //           update = true;
 //       }
 //   }

 //   IEnumerator FirstSpawn()
 //   {
 //       if (Spawning)
 //       {
 //           yield break;
 //       }

 //       Spawning = true;
 //       Mesh mesh = GetComponent<MeshFilter>().mesh;
 //       Vector3[] vertices = mesh.vertices;
 //       int i = 0;
 //       while (i < vertices.Length)
 //       {
 //           vertices[i] += Vector3.up * Time.deltaTime;
 //           i++;
 //       }
 //       mesh.vertices = vertices;
 //       mesh.RecalculateBounds();

 //       var matrix = transform.localToWorldMatrix;

 //       for (int vertId = 0; vertId < vertices.Length; vertId++)
 //       {
 //           //print(vertices[vertId].z);
 //           if (vertices[vertId].z > threshold)
 //           {
 //               GameObject dunaInstance;
 //               dunaInstance = Instantiate(duna, matrix.MultiplyPoint3x4(vertices[vertId]), rotation, duneParent);
 //               dunaInstance.GetComponent<DuneDeserto_Mod>().SetMaterial( materials[UnityEngine.Random.Range(0, materials.Length)]);
 //           }
 //           //yield return new WaitForEndOfFrame();
 //           yield return new WaitForEndOfFrame();
 //       }

 //       childNumber = duneParent.childCount;
 //       Spawning = false;
 //       FinishedSpawning = true;
 //       //gameObject.SetActive(false);
 //       yield break;
 //   }

 //   IEnumerator Loop(bool OnOff)
 //   {
 //       for (int i = 0; i < childNumber; i++)
 //       {
 //           duneParent.GetChild(i).gameObject.SetActive(OnOff);
 //           yield return new WaitForSeconds(0.025f);
 //           //yield return null;
 //       }

 //       yield break;
 //   }
}