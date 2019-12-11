using UnityEngine;

public class TerrainGenerator : MonoBehaviour {

    public int width = 256;
    public int height = 256;

    public int depth = 20;

    public float scale = 20f;

    public float offsetX = 100f;
    public float offsetY = 100f;

    public float envMoveSpeed;

    public float sinWaveNumX = 5f;
    public float sinWaveNumY = 5f;

    Terrain terrain;

    public bool perlinOrSin;

    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    void Update()
    {
        offsetX += Time.deltaTime * envMoveSpeed;
        //offsetY += Time.deltaTime * envMoveSpeed;

        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;

        terrainData.size = new Vector3(width, depth, height);

        terrainData.SetHeights(0, 0, GenerateHeights());

        return terrainData;
    }

    float [,] GenerateHeights()
    {
        float[,] heights = new float[width, height];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }

        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        if (perlinOrSin)
        {
            //perlin noise
            return Mathf.PerlinNoise(xCoord, yCoord);
        }
        else
        {
            //sin wave 
            return Mathf.Sin(sinWaveNumX * Mathf.PI * xCoord) * Mathf.Sin(sinWaveNumY * Mathf.PI * yCoord);
        }
    }

    //called to set terrain for dif. looking environments
    public void SetTerrainValues(int newDepth, float newScale, float scrollSpeed)
    {
        depth = newDepth;
        scale = newScale;
        envMoveSpeed = scrollSpeed;
    }

}
