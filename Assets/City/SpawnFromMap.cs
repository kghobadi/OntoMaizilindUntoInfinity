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
    float range;
    Color white = Color.white;
    Color red = Color.red;
    Color blue = Color.blue;
    Color green = Color.green;
    Color black = Color.black;
    Color grey = Color.grey;
    Color pixel_colour;
    List<Vector3> worldPos;
    List<Color> colors;
    public List<Quaternion> rots;
    public List<Quaternion> rots2;
    public List<Material> mats;

    public List<GameObject> buildings;
    public List<GameObject> interiors;
    public Transform cityParent;
    public GameObject terrain;
    public GameObject landScape;
    int oldlayer;
    public List<GameObject> interiorsObj;
    public List<Vector3> buildingPos;
    public List<Vector3> interiorPos;
    public bool InstantiateOnStart = false;
    public bool ActivateInteriorsLoop = false;
#if UNITY_EDITOR
    private void Start()
    {
        if (ActivateInteriorsLoop)
        {
            StartCoroutine(ActivateInteriors());
        }

        if (InstantiateOnStart)
        {
            StartCoroutine(SpawnBuildingsLoop());
        }
    }

    IEnumerator ActivateInteriors()
    {
        print("start coroutine");
        Transform cam = Camera.main.transform;
        int i = 0;
        while (true)
        {
            if (i < interiorsObj.Count)
            {
                print("activate game object");
                interiorsObj[i].SetActive(true);
                i++;
            }
            else
            {
                yield break;
            }
            yield return null;
        }
    }
    
    [ContextMenu("Spawn City")]
    void SpawnCity()
    {
        interiorsObj.Clear();

        terrain = GameObject.Find("terrain");
        oldlayer = terrain.layer;
        terrain.layer = 31;
        MeshCollider mCollider = terrain.AddComponent<MeshCollider>();

        landScape = GameObject.Find("LandScape");
        oldlayer = landScape.layer;
        landScape.layer = 31;
        MeshCollider mCollider2 = landScape.AddComponent<MeshCollider>();

        size.x = tex.width;
        size.y = tex.height;
        range = ((1 / size.x) * 10) / 2;

        cityParent = transform.Find("CITY");
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

        terrain.layer = oldlayer;
        landScape.layer = oldlayer;
        DestroyImmediate(mCollider);
        DestroyImmediate(mCollider2);
    }

    [ContextMenu("Find Building Positions")]
    void FindPositions()
    {
        interiorsObj.Clear();
        buildingPos.Clear();
        interiorPos.Clear();

        terrain = GameObject.Find("terrain");
        oldlayer = terrain.layer;
        terrain.layer = 31;
        MeshCollider mCollider = terrain.AddComponent<MeshCollider>();

        landScape = GameObject.Find("LandScape");
        oldlayer = landScape.layer;
        landScape.layer = 31;
        MeshCollider mCollider2 = landScape.AddComponent<MeshCollider>();

        size.x = tex.width;
        size.y = tex.height;
        range = ((1 / size.x) * 10) / 2;

        Vector3 pos;
        RaycastHit hit;
        float hitY;
        int layerMask = 1 << 31;

        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                pixel_colour = tex.GetPixel(x, z);

                float localX = ((x / size.x) * 10 - 0.5f * 10 + range) * size.x / 10;
                float localY = ((z / size.x) * 10 - 0.5f * 10 + range) * size.x / 10;

                pos = transform.TransformPoint(new Vector3(localX, 0, localY));
                worldPos.Add(pos);
                colors.Add(pixel_colour);

                if (pixel_colour == white)
                {
                    SetBuilding(x, z, false);
                }
            }
        }

        terrain.layer = oldlayer;
        DestroyImmediate(mCollider);
        DestroyImmediate(mCollider2);
    }

    [ContextMenu("Deactivate Interiors")]
    void DeactivateInteriors()
    {
        foreach (GameObject obj in interiorsObj)
        {
            obj.SetActive(false);
        }
    }

    void Reset()
    {
        buildings.Add(Resources.Load("Buildings/building 1") as GameObject);
        buildings.Add(Resources.Load("Buildings/building 2") as GameObject);
        buildings.Add(Resources.Load("Buildings/building 3") as GameObject);
        buildings.Add(Resources.Load("Buildings/building 4") as GameObject);
        buildings.Add(Resources.Load("Buildings/building 5") as GameObject);
        buildings.Add(Resources.Load("Buildings/building 6") as GameObject);
        interiors.Add(Resources.Load("Buildings/interior 2") as GameObject);

        rots.Add(Quaternion.Euler(0, 0, 0));
        rots.Add(Quaternion.Euler(0, 90, 0));
        rots.Add(Quaternion.Euler(0, 180, 0));
        rots.Add(Quaternion.Euler(0, 270, 0));

        mats.Add(Resources.Load("Buildings/building mat") as Material);
        mats.Add(Resources.Load("Buildings/building mat 1") as Material);
        mats.Add(Resources.Load("Buildings/building mat 2") as Material);
        //mats.Add(Resources.Load("Buildings/building mat 3") as Material);

        terrain = GameObject.Find("terrain");
        oldlayer = terrain.layer;
        terrain.layer = 31;
        MeshCollider mCollider = terrain.AddComponent<MeshCollider>();

        landScape = GameObject.Find("LandScape");
        oldlayer = landScape.layer;
        landScape.layer = 31;
        MeshCollider mCollider2 = landScape.AddComponent<MeshCollider>();

        tex = Resources.Load("city grid") as Texture2D;
        //texHeight = Resources.Load("heightmap") as Texture2D;
        size.x = tex.width;
        size.y = tex.height;
        range = ((1 / size.x) * 10) / 2;

        cityParent = transform.Find("CITY");
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

        terrain.layer = oldlayer;
        DestroyImmediate(mCollider);
        DestroyImmediate(mCollider2);
    }

    GameObject clone;

    void SpawnGrid()
    {
        Vector3 pos;
        RaycastHit hit;
        float hitY;
        int layerMask = 1 << 31;

        for (int x = 0; x < size.x; x++)
        {
            for (int z = 0; z < size.y; z++)
            {
                pixel_colour = tex.GetPixel(x,z);

                float localX = ((x / size.x) * 10 - 0.5f * 10 + range)*size.x/10;
                float localY = ((z / size.x) * 10 - 0.5f * 10 + range)*size.x / 10;

                pos = transform.TransformPoint(new Vector3(localX, 0, localY));
                worldPos.Add(pos);
                colors.Add(pixel_colour);

                if (pixel_colour == white)
                {
                    SetBuilding(x,z, true);
                }
                else if (pixel_colour == blue)
                {
                    print("blue");
                    clone = PrefabUtility.InstantiatePrefab(interiors[Random.Range(0, interiors.Count)]) as GameObject;
                    clone.transform.rotation = rots[Random.Range(0, rots.Count)];
                    //clone.name = "Blue - LocalPos: " + localX as string + "," + localY as string + "; Pixel: " + x as string + "," + z as string;
                    clone.transform.localScale = new Vector3(
                        transform.localScale.x,
                        transform.localScale.y * (float)System.Math.Round(Random.Range(1f, 1.2f), 1),
                        transform.localScale.z);
                }
                else
                {
                    clone = null;
                }

                if (clone != null)
                {
                    //clone.transform.position = pos;
                    if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, layerMask))
                    {
                        hitY = hit.point.y;
                        clone.transform.position = new Vector3(pos.x, hitY, pos.z);
                    }
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

    void SetBuilding(int x, int y, bool instantiate)
    {
        int i = 0;
        Color adjacentPixel = tex.GetPixel(x, y + 1);
        if (adjacentPixel != black && adjacentPixel != grey)
        {
            i++;
        }
        adjacentPixel = tex.GetPixel(x, y - 1);
        if (adjacentPixel != black && adjacentPixel != grey)
        {
            i++;
        }
        adjacentPixel = tex.GetPixel(x + 1, y);
        if (adjacentPixel != black && adjacentPixel != grey)
        {
            i++;
        }
        adjacentPixel = tex.GetPixel(x - 1, y);
        if (adjacentPixel != black && adjacentPixel != grey)
        {
            i++;
        }
        if (i == 4)
        {
            print("interior");
            //if all adjacent pixels are buildings, instantiate a "interior" building prefab
            if (instantiate)
            {
                clone = PrefabUtility.InstantiatePrefab(interiors[Random.Range(0, interiors.Count)]) as GameObject;
                clone.transform.rotation = rots[Random.Range(0, rots.Count)];
                if (Random.value > 0.5f)
                {
                    clone.transform.GetChild(0).rotation = Quaternion.Euler(90, 0, 0);
                }
                clone.transform.localScale = new Vector3(
                    transform.localScale.x,
                    transform.localScale.y * Random.Range(9, 11) / 10,
                    transform.localScale.z);
                interiorsObj.Add(clone);
            }
            else
            {
                Vector3 pos;
                RaycastHit hit;
                float hitY;
                int layerMask = 1 << 31;
                float localX = ((x / size.x) * 10 - 0.5f * 10 + range) * size.x / 10;
                float localY = ((y / size.x) * 10 - 0.5f * 10 + range) * size.x / 10;
                pos = transform.TransformPoint(new Vector3(localX, 0, localY));
                if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, layerMask))
                {
                    hitY = hit.point.y;
                    interiorPos.Add(new Vector3(pos.x, hitY, pos.z));
                }
            }

        } else
        {
            print("normal building");
            if (instantiate)
            {
                //otherwise, choose from the normal buildings
                clone = PrefabUtility.InstantiatePrefab(buildings[Random.Range(0, buildings.Count)]) as GameObject;
                clone.transform.rotation = rots[Random.Range(0, rots.Count)];
                clone.transform.localScale = new Vector3(
                    transform.localScale.x,
                    transform.localScale.y * Random.Range(8, 13) / 10,
                    //transform.localScale.y * (float)System.Math.Round(Random.Range(0.8f, 1.2f), 1),
                    transform.localScale.z);
            }
            else
            {
                Vector3 pos;
                RaycastHit hit;
                float hitY;
                int layerMask = 1 << 31;
                float localX = ((x / size.x) * 10 - 0.5f * 10 + range) * size.x / 10;
                float localY = ((y / size.x) * 10 - 0.5f * 10 + range) * size.x / 10;
                pos = transform.TransformPoint(new Vector3(localX, 0, localY));
                if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, layerMask))
                {
                    hitY = hit.point.y;
                    buildingPos.Add(new Vector3(pos.x, hitY, pos.z));
                }
            }

        }
    }

    IEnumerator SpawnBuildingsLoop()
    {
        int i = 0;
        bool go = true;
        while (go)
        {
            if (i < buildingPos.Count)
            {
                clone = PrefabUtility.InstantiatePrefab(buildings[Random.Range(0, buildings.Count)]) as GameObject;
                clone.transform.rotation = rots[Random.Range(0, rots.Count)];
                clone.transform.localScale = new Vector3(
                    transform.localScale.x,
                    transform.localScale.y * Random.Range(8, 13) / 10,
                    //transform.localScale.y * (float)System.Math.Round(Random.Range(0.8f, 1.2f), 1),
                    transform.localScale.z);
                clone.transform.position = new Vector3(buildingPos[i].x, buildingPos[i].y, buildingPos[i].z);
                clone.transform.SetParent(cityParent);
                i++;
            }
            else
            {
                go = false;
            }
            yield return null;
        }

        i = 0;
        go = true;

        while (go)
        {
            if (i < interiorPos.Count)
            {
                clone = PrefabUtility.InstantiatePrefab(interiors[Random.Range(0, interiors.Count)]) as GameObject;
                clone.transform.rotation = rots[Random.Range(0, rots.Count)];
                if (Random.value > 0.5f)
                {
                    clone.transform.GetChild(0).rotation = Quaternion.Euler(90, 0, 0);
                }
                clone.transform.localScale = new Vector3(
                    transform.localScale.x,
                    transform.localScale.y * Random.Range(8, 13) / 10,
                    //transform.localScale.y * (float)System.Math.Round(Random.Range(0.8f, 1.2f), 1),
                    transform.localScale.z);
                clone.transform.position = new Vector3(interiorPos[i].x, interiorPos[i].y, interiorPos[i].z);
                clone.transform.SetParent(cityParent);
                i++;
            }
            else
            {
                go = false;
            }
            yield return null;
        }
        yield break;
    }
 #endif
    /*
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
    }*/
}