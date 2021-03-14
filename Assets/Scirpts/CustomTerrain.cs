using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]

public class CustomTerrain : MonoBehaviour
{

    public Vector2 randomHeightRange = new Vector2(0, 0.1f);
    public Texture2D heightMapImage;
    public Vector3 heightMapScale = new Vector3(1, 1, 1);

    // Perlin Noise
    public float perlinXScale = 0.01f;
    public float perlinYScale = 0.01f;
    public int perlinOffsetX = 0;
    public int perlinOffsetY = 0;
    public int perlinOctaves = 3;
    public float perlinPersistance = 8;
    public float perlinHeightScale = 0.09f;

    public Terrain terrain;
    public TerrainData terrainData;

    public bool resetTerrain = true;

    public float GetSteepness(float[,] heightmap, int x, int y, int width , int height ) {
        float h = heightmap[x, y];
        int nx = x + 1;
        int ny = y + 1;

        if(nx > width - 1) {
            nx = x - 1;
        }
        if (ny > height - 1)
        {
            ny = x - 1;
        }
        float dx = heightmap[nx, y] - h;
        float dy = heightmap[x, ny] - h;



        Vector2 graident = new Vector2(dx, dy);
        float steep = graident.magnitude;

        return steep;


    }
  


    public float voronoiFallOff = 0.2f;
    public float voronoiDropOff = 0.6f;
    public float voronoiMinHeight = 0.1f;
    public float voronoiMaxHeight = 0.5f;
    public int voronoiPeaks = 1;
    public enum VornoiTypes
    {
        Linear = 0,
        Power = 1,
        SinPower = 2,
        Combined = 3


    }
    public VornoiTypes vornoiType = VornoiTypes.Linear;



    public float MPDheightMin = -2f;
    public float MPDhieghtmax = 2f;
    public float MPDhieghtDampenerPower = 2.0f;
    public float MPDRoughness = 2.0f;

    public float roughness = 2.0f;

    public int SmoothAmount = 2;



    //MULTIPLE PERLIN --------------------
    [System.Serializable]
    public class PerlinParameters
    {
        public float mPerlinXScale = 0.01f;
        public float mPerlinYScale = 0.01f;
        public int mPerlinOctaves = 3;
        public float mPerlinPersistance = 8;
        public float mPerlinHeightScale = 0.09f;
        public int mPerlinOffsetX = 0;
        public int mPerlinOffsetY = 0;
        public bool remove = false;
    }

    public List<PerlinParameters> perlinParameters = new List<PerlinParameters>() {
        new PerlinParameters()
    };

    [System.Serializable]
    public class SplathHeights {
        public Texture2D texture = null;
        public Texture2D normalmap = null;
        public float Smoothness;
        public float minslope = 0;
        public float maxslope = 1.5f;
        public float Metalic;
        public float splatNoiseXscale = 0.01f;
        public float splatNoiseYscale = 0.01f;
        public float SplatOffset = 0.01f;
        public float SplatScaler = 0.01f;
        public float minHeight = 0.1f;
        public float Maxheight = 0.2f;
        public Vector2 tileOffset = new Vector2(0, 0);
        public Vector2 TileSize = new Vector2(0, 0);

        public bool remove;


    }
   


    public List<SplathHeights> splatHeights = new List<SplathHeights>() {
        new SplathHeights()

    };

    #region Functions

    float[,] GetHeightMap()
    {
        if (!resetTerrain)
        {
            return terrainData.GetHeights(0, 0, terrainData.heightmapWidth,
                                                terrainData.heightmapHeight);
        }
        else
            return new float[terrainData.heightmapWidth,
                             terrainData.heightmapHeight];

    }


    float[,,] Saves;



    public void AddNewSplatHeight() {
        splatHeights.Add(new SplathHeights());


      


    }
    public void RemoveSplathHeight() {
        List<SplathHeights> keptSplatHeights = new List<SplathHeights>();
        for (int i = 0; i < splatHeights.Count; i++) {
            if(!splatHeights[i].remove) {
                keptSplatHeights.Add(splatHeights[i]);


            }

        }
        if (keptSplatHeights.Count == 0) {
            keptSplatHeights.Add(splatHeights[0]);

        }
        splatHeights = keptSplatHeights;

    }

    public void SplatMap() {
        SplatPrototype[] newSplatPrototype;


        newSplatPrototype = new SplatPrototype[splatHeights.Count];
        int spindex = 0;
        foreach(SplathHeights sh in splatHeights) {
            newSplatPrototype[spindex] = new SplatPrototype();
            newSplatPrototype[spindex].texture = sh.texture;
            newSplatPrototype[spindex].normalMap = sh.normalmap;
            newSplatPrototype[spindex].smoothness = sh.Smoothness;
            newSplatPrototype[spindex].metallic = sh.Metalic;
          
            newSplatPrototype[spindex].tileOffset = sh.tileOffset;
            newSplatPrototype[spindex].tileSize = sh.TileSize;
            newSplatPrototype[spindex].texture.Apply(true); 
            spindex++;


        }

        terrainData.splatPrototypes = newSplatPrototype;

        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);

        float[,,] splatMapdata = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float[] splat = new float[terrainData.alphamapLayers];
                for (int i = 0; i < splatHeights.Count; i++)
                {

                    float noise = Mathf.PerlinNoise(x * splatHeights[i].splatNoiseXscale, y * splatHeights[i].splatNoiseYscale) * splatHeights[i].SplatScaler;

                    float offset = splatHeights[i].SplatOffset + noise;

                    float steepness = GetSteepness(heightMap, x, y,
                                                   terrainData.heightmapWidth,
                                                   terrainData.heightmapHeight);
                    float thisHeightStart = splatHeights[i].minHeight - offset ; 
                    float thisHeightStop = splatHeights[i].Maxheight + offset ;


                    if ((heightMap[x, y] >= thisHeightStart && heightMap[x, y] <= thisHeightStop) 
                        && steepness >= splatHeights[i].minslope &&
                        steepness <= splatHeights[i].maxslope) 
                    {
                        splat[i] = 1;

                    }
                }
                    NormalizeVector(splat);
                    for (int j = 0; j < splatHeights.Count(); j++)
                    {
                        splatMapdata[x, y, j] = splat[j];

                    }


            }
        }
        terrainData.SetAlphamaps(0,0,splatMapdata);

    }


    void NormalizeVector(float[] v) {
        float total = 0;
        for (int i = 0; i < v.Length; i++) {
            total += v[i];

        }
        for (int i = 0; i < v.Length; i++)
        {
            total /= total;

        }



    }

    public void Load(float [,] heightmapLoad)
    {
        float[,] heightmap = GetHeightMap();

        heightmap = heightmapLoad;

    }

    public void MidPointDisplacement () {

        float[,] heightmap = GetHeightMap();
        int width = terrainData.heightmapWidth - 1;
        int Squaresize = width      ;

        int ConrorX, ConrorY;
        int midX, midY;
        int pmidXL, pmidXR, pmidYU, PmidYD;


        float Heightmin = MPDheightMin;
        float HeightMax = MPDhieghtmax;
        float HeightDapener = (float)Mathf.Pow(MPDhieghtDampenerPower, -1 * MPDRoughness);

        //heightmap[0,0] = UnityEngine.Random.Range(0f, 0.2f);
        //heightmap[0, terrainData.heightmapHeight - 1] = UnityEngine.Random.Range(0f, 0.2f);
       //heightmap[terrainData.heightmapWidth - 1, 0] = UnityEngine.Random.Range(0f, 0.2f);
       // heightmap[terrainData.heightmapWidth - 1, terrainData.heightmapHeight - 1] = UnityEngine.Random.Range(0f, 0.2f);

        while (Squaresize > 0)
        {
           

            for (int x = 0; x < width; x += Squaresize)
            {
                for (int y = 0; y < width; y += Squaresize)
                {

                    ConrorX = (x + Squaresize);
                    ConrorY = (y + Squaresize);

                    midX = (int)(x + Squaresize / 2.0f);
                    midY = (int)(y + Squaresize / 2.0f);

                    heightmap[midX, midY] = (float)((heightmap[x, y]
                                                     + heightmap[ConrorX, y]
                                                     + heightmap[x, ConrorY] +
                                                     heightmap[ConrorX, ConrorY]) / 4.0f + UnityEngine.Random.Range(Heightmin, HeightMax));
                }


            }
            for (int x = 0; x < width; x += Squaresize)
            {
                for (int y = 0; y < width; y += Squaresize)
                {

                    ConrorX = (x + Squaresize);
                    ConrorY = (y + Squaresize);

                    midX = (int)(x + Squaresize / 2.0f);
                    midY = (int)(y + Squaresize / 2.0f);

                    pmidXR = (int)(midX + Squaresize);
                    pmidYU = (int)(midY + Squaresize);
                    pmidXL = (int)(midX - Squaresize);
                    PmidYD = (int)(midY - Squaresize);

                    if (pmidXL <= 0 || PmidYD <= 0 || pmidXR >= width - 1 || pmidYU >= width - 1)
                    {
                        continue;

                    }


                    heightmap[midX, y] = (float)((heightmap[midX, midY]
                                                     + heightmap[x, y]
                                                     + heightmap[midX, PmidYD] +
                                                  heightmap[ConrorX, y]) / 4.0f + UnityEngine.Random.Range(Heightmin, HeightMax));


                    heightmap[midX, ConrorY] = (float)((heightmap[x, ConrorY]
                                                        + heightmap[midX, midY]
                                                        + heightmap[ConrorX, ConrorY] +
                                                        heightmap[midX, pmidYU]) / 4.0f + UnityEngine.Random.Range(Heightmin, HeightMax));



                    heightmap[x, midY] = (float)((heightmap[x, y]
                                                        + heightmap[pmidXL, midY]
                                                  + heightmap[x, ConrorY]
                                                  + heightmap[midX, midY])
                                                  / 4.0f + UnityEngine.Random.Range(Heightmin, HeightMax));


                    heightmap[ConrorX, midY] = (float)((heightmap[midX, y]
                                                        + heightmap[midX, midY]
                                                        + heightmap[ConrorX, ConrorY]
                                                        + heightmap[pmidXR, midY] )
                                                       / 4.0f + UnityEngine.Random.Range(Heightmin, HeightMax));

                }   


                }


               



                    
                


            



            Squaresize = (int)(Squaresize / 2.0f);
            Heightmin *= HeightDapener;   
            HeightMax *= HeightDapener;    
        }
        terrainData.SetHeights(0, 0, heightmap);
    }



    public void Vornoi() {

        float[,] heightMap = GetHeightMap();


        for (int p = 0; p < voronoiPeaks; p++) {


          Vector3 peak = new Vector3(UnityEngine.Random.Range(0, terrainData.heightmapWidth), UnityEngine.Random.Range(0.0f, 1.0f),
                                   UnityEngine.Random.Range(0, terrainData.heightmapHeight));



            //Vector3 peak = new Vector3(256, 0.2f, 256 );

            if (heightMap[(int)peak.x, (int)peak.z] < peak.y) 
            heightMap[(int)peak.x, (int)peak.z] = peak.y;

        heightMap[(int)peak.x, (int)peak.z] = peak.y;
        Vector2 peakLocation = new Vector2(peak.x, peak.z);
        float maxDistance = Vector2.Distance(new Vector2(0, 0), new Vector2(terrainData.heightmapWidth,terrainData.heightmapHeight   ));
            

        for (int y = 0; y < terrainData.heightmapHeight; y++)
        {
            for (int x = 0; x < terrainData.heightmapWidth; x++)
            {
                    if (!(x == peak.x && y == peak.z))
                    {
                        float distanceToPeak = Vector2.Distance(peakLocation, new Vector2(x, y)) / maxDistance;
                        float h = peak.y - distanceToPeak * voronoiFallOff - Mathf.Pow(distanceToPeak, voronoiDropOff);

                        if(vornoiType == VornoiTypes.Combined) {

                            h = peak.y - distanceToPeak * voronoiFallOff -
                                    Mathf.Pow(distanceToPeak, voronoiDropOff);
                            

                        } else if (vornoiType == VornoiTypes.Power) {

                            h = peak.y - Mathf.Pow(distanceToPeak, voronoiDropOff) * voronoiFallOff; // power


                        }else if (vornoiType == VornoiTypes.SinPower)
                        {

                            h = peak.y - Mathf.Pow(distanceToPeak * 3, voronoiDropOff ) * voronoiFallOff - Mathf.Sin(distanceToPeak * 2 * Mathf.PI)/ voronoiDropOff;
                        } else {
                            h = peak.y - distanceToPeak * voronoiFallOff;  // Linear 
                        }


                        if (heightMap[x, y] < h) {

                            heightMap[x, y] = h;

                        }

                    
                }
            }
        }


            }
        terrainData.SetHeights(0,0,heightMap);


    }

    List<Vector2> GenerateNeighbours(Vector2 pos, int width, int height)
    {
        List<Vector2> neighbours = new List<Vector2>();
        for (int y = -1; y < 2; y++)
        {
            for (int x = -1; x < 2; x++)
            {
                if (!(x == 0 && y == 0))
                {
                    Vector2 nPos = new Vector2(Mathf.Clamp(pos.x + x, 0, width - 1),
                                                Mathf.Clamp(pos.y + y, 0, height - 1));
                    if (!neighbours.Contains(nPos))
                        neighbours.Add(nPos);
                }
            }
        }
        return neighbours;
    }


    public void Smooth()
    {
        float[,] heightMap = GetHeightMap();
        float smoothProgress = 0;
        EditorUtility.DisplayProgressBar("Smoothing Terrain",
                                 "Progress",
                                 smoothProgress);

        for (int s = 0; s < SmoothAmount; s++)
        {
            for (int y = 0; y < terrainData.heightmapHeight; y++)
            {
                for (int x = 0; x < terrainData.heightmapWidth; x++)
                {
                    float avgHeight = heightMap[x, y];
                    List<Vector2> neighbours = GenerateNeighbours(new Vector2(x, y),
                                                                  terrainData.heightmapWidth,
                                                                  terrainData.heightmapHeight);
                    foreach (Vector2 n in neighbours)
                    {
                        avgHeight += heightMap[(int)n.x, (int)n.y];
                    }

                    heightMap[x, y] = avgHeight / ((float)neighbours.Count + 1);
                }
            }
            smoothProgress++;
            EditorUtility.DisplayProgressBar("Smoothing Terrain",
                                             "Progress",
                                             smoothProgress / SmoothAmount);

        }
        terrainData.SetHeights(0, 0, heightMap);
        EditorUtility.ClearProgressBar();
    }







    /*
    public void SmoothOLD()     {         float[,] heightMap = GetHeightMap();         float smoothProgress = 0;         EditorUtility.DisplayProgressBar("Smoothing Terrain",                                  "Progress",                                  smoothProgress);          for (int s = 0; s < SmoothAmount; s++)         {             for (int y = 0; y < terrainData.heightmapHeight; y++)             {                 for (int x = 0; x < terrainData.heightmapWidth; x++)                 {                     float avgHeight = heightMap[x, y];                     List<Vector2> neighbours = GenerateNeighbors(new Vector2(x, y),                                                                   terrainData.heightmapWidth,                                                                   terrainData.heightmapHeight);                     foreach (Vector2 n in neighbours)                     {                         avgHeight += heightMap[(int)n.x, (int)n.y];                     }                      heightMap[x, y] = avgHeight / ((float)neighbours.Count + 1);                 }             }             smoothProgress++;             EditorUtility.DisplayProgressBar("Smoothing Terrain",                                              "Progress",                                              smoothProgress / SmoothAmount);          }         terrainData.SetHeights(0, 0, heightMap);         EditorUtility.ClearProgressBar();     } 

*/






    public void Perlin()
    {
        float[,] heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth,
                                                    terrainData.heightmapHeight);

        for (int y = 0; y < terrainData.heightmapHeight; y++)
        {
            for (int x = 0; x < terrainData.heightmapWidth; x++)
            {
                heightMap[x, y] = Utils.fBM((x + perlinOffsetX) * perlinXScale,
                                            (y + perlinOffsetY) * perlinYScale,
                                            perlinOctaves,
                                            perlinPersistance) * perlinHeightScale;

                /*Mathf.PerlinNoise((x + perlinOffsetX) * perlinXScale, 
                                                (y + perlinOffsetY) * perlinYScale);*/
            }
        }

        terrainData.SetHeights(0, 0, heightMap);
    }

    public void MultiplePerlinTerrain()
    {
        float[,] heightMap = GetHeightMap();
        for (int y = 0; y < terrainData.heightmapHeight; y++)
        {
            for (int x = 0; x < terrainData.heightmapWidth; x++)
            {
                foreach (PerlinParameters p in perlinParameters)
                {
                    heightMap[x, y] += Utils.fBM((x + p.mPerlinOffsetX) * p.mPerlinXScale,
                                                 (y + p.mPerlinOffsetY) * p.mPerlinYScale,
                                                    p.mPerlinOctaves,
                                                    p.mPerlinPersistance) * p.mPerlinHeightScale;
                }
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void AddNewPerlin() {
        perlinParameters.Add(new PerlinParameters());
    }

    public void RemovePerlin() {
        List<PerlinParameters> keptPerlinParameters = new List<PerlinParameters>();
        for (int i = 0; i < perlinParameters.Count; i++) {
            if(!perlinParameters[i].remove) {
                keptPerlinParameters.Add(perlinParameters[i]);
            }
        }

        if(keptPerlinParameters.Count == 0) { // dont want to keep any
            keptPerlinParameters.Add(perlinParameters[0]); // add at least 1
        }
        perlinParameters = keptPerlinParameters;
    }

    public void RandomTerrain()
    {
        float[,] heightMap = new float[terrainData.heightmapWidth, terrainData.heightmapHeight];

        for (int x = 0; x < terrainData.heightmapWidth; x++)
        {
            for (int y = 0; y < terrainData.heightmapHeight; y++)
            {
                heightMap[x, y] = UnityEngine.Random.Range(randomHeightRange.x, randomHeightRange.y);
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void LoadTexture()
    {
        float[,] heightMap = new float[terrainData.heightmapWidth, terrainData.heightmapHeight];

        for (int x = 0; x < terrainData.heightmapWidth; x++)
        {
            for (int z = 0; z < terrainData.heightmapHeight; z++)
            {
                heightMap[x, z] = heightMapImage.GetPixel((int)(x * heightMapScale.x),
                                                          (int)(z * heightMapScale.z)).grayscale
                                                            * heightMapScale.y;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    public void ResetTerrain()
    {
        float[,] heightMap = new float[terrainData.heightmapWidth, terrainData.heightmapHeight];
        for (int x = 0; x < terrainData.heightmapWidth; x++)
        {
            for (int z = 0; z < terrainData.heightmapHeight; z++)
            {
                heightMap[x, z] = 0;
            }
        }
        terrainData.SetHeights(0, 0, heightMap);
    }

    private void OnEnable()
    {
        Debug.Log("Initialising Terrain Data");
        terrain = this.GetComponent<Terrain>();
        terrainData = Terrain.activeTerrain.terrainData;
        Debug.Log(terrainData.heightmapWidth);
    }

    // Use this for initialization
    void Awake()
    {
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        AddTag(tagsProp, "Terrain");
        AddTag(tagsProp, "Cloud");
        AddTag(tagsProp, "Shore");

        //apply tag changes to the tag database
        tagManager.ApplyModifiedProperties();

        // take this object
        this.gameObject.tag = "Terrain";
    }

    void AddTag(SerializedProperty tagsProp, string newTag)
    {
        bool found = false;

        // Ensure the tag doesn't already exist
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(newTag))
            {
                found = true;
                break;
            }
        }

        // add yoru new tag
        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(0);
            newTagProp.stringValue = newTag;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion
}
