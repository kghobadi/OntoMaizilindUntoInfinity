using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpawnFromMap : MonoBehaviour
{
    public Vector2 tiling = new Vector2(10, 10);
    public Vector2 size;
    public Texture2D tex;
    public Texture2D texHeight;
    public Vector2 heightRange = new Vector2(0, 5);
    float coorX;
    float coorY;
    float range;
    Color white = Color.white;
    Color red = Color.red;
    Color blue = Color.blue;
    Color green = Color.green;
    Color black = Color.black;
    Color pixel_colour;
    float pixel_height;
    List<Vector3> worldPos;
    List<Color> colors;
    public List<Quaternion> rots;
    List<Material> mats;

    public List<GameObject> buildings;
    public Transform cityParent;

    void Reset()
    {
        buildings.Add(Resources.Load("Buildings/building 1") as GameObject);
        buildings.Add(Resources.Load("Buildings/building 2") as GameObject);
        buildings.Add(Resources.Load("Buildings/building 3") as GameObject);
        buildings.Add(Resources.Load("Buildings/building 4") as GameObject);
        buildings.Add(Resources.Load("Buildings/building 5") as GameObject);
        buildings.Add(Resources.Load("Buildings/building 6") as GameObject);

        rots.Add(Quaternion.Euler(0, 0, 0));
        rots.Add(Quaternion.Euler(0, 90, 0));
        rots.Add(Quaternion.Euler(0, 180, 0));
        rots.Add(Quaternion.Euler(0, 270, 0));

        mats.Add(Resources.Load("Buildings/building mat") as Material);
        mats.Add(Resources.Load("Buildings/building mat 1") as Material);
        mats.Add(Resources.Load("Buildings/building mat 2") as Material);
        mats.Add(Resources.Load("Buildings/building mat 3") as Material);

        tex = Resources.Load("map grid") as Texture2D;
        texHeight = Resources.Load("map noise") as Texture2D;
        size.x = tex.width;
        size.y = tex.height;
        range = ((1 / size.x) * 10) / 2;

        cityParent = transform.FindChild("CITY");
        DestroyImmediate(cityParent.gameObject);
        GameObject city = new GameObject("CITY");
		city.transform.position = transform.position;
		city.transform.rotation = Quaternion.identity;
        city.name = "CITY";
        city.transform.SetParent(transform);
        cityParent = city.transform;

        worldPos.Clear();
        colors.Clear();

        SpawnGrid();
    }

    GameObject clone;

    void SpawnGrid()
    {
        Vector3 pos;

        for (int x = 0; x < tiling.x; x++)
        {
            for (int z = 0; z < tiling.y; z++)
            {
                coorX = (size.x * (x / tiling.x))+range;
                coorY = (size.y * (z / tiling.y))+range;
                pixel_colour = tex.GetPixel(x,z);

                float localX = (x / size.x) * 10 - 0.5f * 10 + range;
                float localY = (z / size.x) * 10 - 0.5f * 10 + range;

                pos = transform.TransformPoint(new Vector3(localX, 0, localY));
                worldPos.Add(pos);
                colors.Add(pixel_colour);

                if (pixel_colour == white)
                {
                    clone = PrefabUtility.InstantiatePrefab(buildings[Random.Range(0, buildings.Count)]) as GameObject;
                    clone.transform.rotation = rots[Random.Range(0, rots.Count)];
                }

                if (clone != null)
                {
                    //clone.transform.position = pos;
                    pixel_height = texHeight.GetPixel(x, z).a;
                    clone.transform.position = new Vector3(pos.x, pos.y + (Mathf.Lerp(heightRange.x, heightRange.y, pixel_height)), pos.z);
                    clone.transform.localScale = new Vector3(
                        transform.localScale.x,
                        transform.localScale.y * (float)System.Math.Round(Random.Range(0.7f, 1.2f), 1),
                        transform.localScale.z);
                    MeshRenderer[] rends = clone.GetComponentsInChildren<MeshRenderer>();
                    Material mat = mats[Random.Range(0, mats.Count)];
                    foreach (MeshRenderer mR in rends)
                    {
                        mR.material = mat;
                    }
                    clone.transform.SetParent(cityParent);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (worldPos.Count > 0)
        {
            for (int i = 0; i < worldPos.Count; i++)
            {
                Gizmos.color = colors[i];
                Gizmos.DrawSphere(worldPos[i], 0.3f);
            }
        }
    }
}