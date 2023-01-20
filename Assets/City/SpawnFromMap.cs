using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFromMap : MonoBehaviour
{
    public Vector2 tiling = new Vector2(10, 10);
    public Vector2 size;
    public Texture2D tex;
    float coorX;
    float coorY;
    float range;
    Color white = Color.white;
    Color red = Color.red;
    Color blue = Color.blue;
    Color green = Color.green;
    Color black = Color.black;
    Color pixel_colour;
    List<Vector3> worldPos;
    List<Color> colors;

    public List<GameObject> buildings;

    void Start()
    {
        //GetComponent<Collider>().bounds.min.x;
    }

    void Reset()
    {
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        worldPos.Clear();
        colors.Clear();

        tex = Resources.Load("map grid") as Texture2D;
        size.x = tex.width;
        size.y = tex.height;
        range = ((1 / size.x) * 10)/2;

        buildings.Add(Resources.Load("Buildings/building 1") as GameObject);

        SpawnGrid();
    }


    void SpawnGrid()
    {
        //int layerMask = 1 << 31;
        //RaycastHit hit;
        Vector3 pos;

        for (int x = 0; x < tiling.x; x++)
        {
            for (int z = 0; z < tiling.y; z++)
            {
                coorX = (size.x * (x / tiling.x))+range;
                coorY = (size.y * (z / tiling.y))+range;
                pixel_colour = tex.GetPixel(x,z);
                //print(x + " + " + z + " = " + pixel_colour);

                float localX = (x / size.x) * 10 - 0.5f * 10 + range;
                float localY = (z / size.x) * 10 - 0.5f * 10 + range;

                pos = transform.TransformPoint(new Vector3(localX, 0, localY));
                worldPos.Add(pos);
                colors.Add(pixel_colour);

                if (pixel_colour == red)
                {
                    GameObject clone = Instantiate(buildings[0], pos, transform.rotation);
                    clone.transform.SetParent(this.transform);
                }

                /*GameObject clone = Instantiate(prefabToSpawn,
                    transform.position + gridOrigin + new Vector3(gridOffset * x, 0, gridOffset * z), transform.rotation);
                clone.transform.SetParent(this.transform);*/
            }
        }
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < worldPos.Count; i++)
        {
            Gizmos.color = colors[i];
            Gizmos.DrawSphere(worldPos[i], 0.3f);
        }
    }
}
