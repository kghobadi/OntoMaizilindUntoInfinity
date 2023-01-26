using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CitySpawn: MonoBehaviour
{
    public List<GameObject> buildings;
    public Transform cityParent;
    public Quaternion rotation = Quaternion.Euler(0, 180, 0);
	List<Quaternion> rots;
	List<Material> mats;
	public float threshold = -300;
    public float childNumber;
	Mesh originalMesh;



	private void Reset()
	{
		print("reset");
		buildings.Add(Resources.Load("Buildings/building 1") as GameObject);
		buildings.Add(Resources.Load("Buildings/building 2") as GameObject);
		buildings.Add(Resources.Load("Buildings/building 3") as GameObject);
		buildings.Add(Resources.Load("Buildings/building 4") as GameObject);

		rots.Add(Quaternion.Euler(0, 0, 0));
		rots.Add(Quaternion.Euler(0, 90, 0));
		rots.Add(Quaternion.Euler(0, 180, 0));
		rots.Add(Quaternion.Euler(0, 270, 0));

		mats.Add(Resources.Load("Buildings/building mat") as Material);
		mats.Add(Resources.Load("Buildings/building mat 1") as Material);
		mats.Add(Resources.Load("Buildings/building mat 2") as Material);
		mats.Add(Resources.Load("Buildings/building mat 3") as Material);

		cityParent = transform.parent.parent.FindChild("CITY");
		DestroyImmediate(cityParent.gameObject);

		GameObject city = new GameObject("CITY");
		city.transform.position = transform.position;
		city.transform.rotation = Quaternion.identity;
		city.name = "CITY";
		city.transform.SetParent(transform.parent.parent);
		cityParent = city.transform;


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
				GameObject clone = PrefabUtility.InstantiatePrefab(buildings[Random.Range(0, buildings.Count)]) as GameObject;
				clone.transform.position = matrix.MultiplyPoint3x4(vertices[vertId]);
				clone.transform.rotation = rots[Random.Range(0, rots.Count)];
				clone.transform.localScale = new Vector3(
					6,
					6 * (float)System.Math.Round(Random.Range(0.6f, 1.2f), 1),
					6);
				clone.transform.SetParent(cityParent);
				MeshRenderer[] rends = clone.GetComponentsInChildren<MeshRenderer>();
				Material mat = mats[Random.Range(0, mats.Count)];
				foreach (MeshRenderer mR in rends)
				{
					mR.material = mat;
				}
			}
		}
		GetComponent<MeshFilter>().mesh = originalMesh;
	}
}