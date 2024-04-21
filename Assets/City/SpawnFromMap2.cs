using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpawnFromMap2 : MonoBehaviour
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
    public List<GameObject> child;
    public List<Vector3> buildingPos;
    public List<Vector3> interiorPos;
    public bool InstantiateOnStart = false;
    public bool ActivateInteriorsLoop = false;
#if UNITY_EDITOR
    private void Start()
    {

    }

    public float Multiplier = 10;
    public float multiplier2 = 1;
    [ContextMenu("Spawn City")]
    void SpawnCity()
    {
        child.Clear();

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
        range = ((1 / size.x) * multiplier2) / 2;

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
        range = ((1 / size.x) * Multiplier) / 2;

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
                pixel_colour = tex.GetPixel(x, z);

                float localX = ((x / size.x) * Multiplier - 0.5f * Multiplier + range) * size.x / Multiplier;
                float localY = ((z / size.x) * Multiplier - 0.5f * Multiplier + range) * size.x / Multiplier;

                pos = transform.TransformPoint(new Vector3(localX, 0, localY));
                worldPos.Add(pos);
                colors.Add(pixel_colour);

                if (pixel_colour == white)
                {
                    SetBuilding(x, z, true);
                }
                /*
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
                }*/
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
                    BombPosition(clone.transform);
                    child.Add(clone);
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
                //clone = Instantiate(interiors[Random.Range(0, interiors.Count)]);
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
            }
            else
            {
                Vector3 pos;
                RaycastHit hit;
                float hitY;
                int layerMask = 1 << 31;
                float localX = ((x / size.x) * Multiplier - 0.5f * Multiplier + range) * size.x / Multiplier;
                float localY = ((y / size.x) * Multiplier - 0.5f * Multiplier + range) * size.x / Multiplier;
                pos = transform.TransformPoint(new Vector3(localX, 0, localY));
                if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, layerMask))
                {
                    hitY = hit.point.y;
                    interiorPos.Add(new Vector3(pos.x, hitY, pos.z));
                }
            }

        }
        else
        {
            print("normal building");
            if (instantiate)
            {
                //otherwise, choose from the normal buildings
                //clone = Instantiate(interiors[Random.Range(0, interiors.Count)]);
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
                float localX = ((x / size.x) * Multiplier - 0.5f * Multiplier + range) * size.x / Multiplier;
                float localY = ((y / size.x) * Multiplier - 0.5f * Multiplier + range) * size.x / Multiplier;
                pos = transform.TransformPoint(new Vector3(localX, 0, localY));
                if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity, layerMask))
                {
                    hitY = hit.point.y;
                    buildingPos.Add(new Vector3(pos.x, hitY, pos.z));
                }
            }

        }
    }

    float qX;
    float qY;
    float qZ;
    public float howManyHits = 0;
    float angleRange = 20;
    void BombPosition(Transform tr)
    {
        howManyHits = Random.Range(1, 3f);
        Vector3 newRot;


        if (Random.value < 0.07f)
        {
            qX = 0 - Random.Range(-angleRange, angleRange);
            qY = 0 - Random.Range(-4, 4);
            qZ = 0 - Random.Range(-angleRange, angleRange);

            newRot = new Vector3(qX, qY, qZ);
            //newRot = new Vector3(Random.Range(-angleRange, angleRange), Random.Range(-4f, 4f), Random.Range(-angleRange, angleRange));
        }
        else
        {
            qX = 0 - Random.Range(-angleRange, angleRange);
            qY = 0 - Random.Range(-5, 5);
            qZ = 0 - Random.Range(-angleRange, angleRange);

            newRot = new Vector3(qX * howManyHits, qY * howManyHits, qZ * howManyHits);

            tr.localPosition = new Vector3(tr.localPosition.x + Random.Range(-15, 15), tr.localPosition.y - Random.Range(15, 30) * howManyHits, tr.localPosition.z + Random.Range(-15, 15));
            tr.eulerAngles = newRot;
        }
    }


    GameObject meshCombined;
    public Transform[] childs;
    /*
    [ContextMenu("Combine Meshes")]
    void CombineMeshes()
    {
        DestroyImmediate(meshCombined);
        
        meshCombined = new GameObject();
        meshCombined.name = "Mesh Combined";
        meshCombined.AddComponent<MeshFilter>();
        meshCombined.AddComponent<MeshRenderer>();
        meshCombined.GetComponent<MeshRenderer>().material = mats[0];
        meshCombined.transform.parent = transform;
        meshCombined.transform.localPosition = new Vector3(0, 0, 0);

        for (int j = 0; j < child.Count; j++)
        {
            child[j].transform.GetChild(0).parent = meshCombined.transform;
        }

        int childN = meshCombined.transform.childCount;
        childs = new Transform[childN];
        for (int j = 0; j < childN; ++j)
        {
            childs[j] = meshCombined.transform.GetChild(j);
        }

        Vector3 scale = transform.localScale;
        transform.localScale = new Vector3(1, 1, 1);

        GameObject combinedObj = meshCombined;
        MeshFilter[] meshFilters = combinedObj.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            //meshFilters[i].gameObject.SetActive(false);
            i++;
        }

        for (int j = 0; j < childN; ++j)
        {
            childs[j].gameObject.SetActive(false);
        }

        var meshFilter = combinedObj.transform.GetComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine);


        //Mesh m = meshFilter.sharedMesh;
        //GetComponent<MeshCollider> ().sharedMesh = meshFilter.mesh;
        combinedObj.SetActive(true);
        combinedObj.transform.localScale = new Vector3(1, 1, 1);
        combinedObj.transform.rotation = Quaternion.identity;
        combinedObj.transform.position = Vector3.zero;

        transform.localScale = scale;
    }*/

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