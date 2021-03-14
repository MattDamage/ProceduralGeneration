using UnityEditor;
using UnityEngine;
using EditorGUITable;

[CustomEditor(typeof(CustomTerrain))]
[CanEditMultipleObjects]

public class CustomTerrainEditor : Editor {

    // Properties
    SerializedProperty randomHeightRange;
    SerializedProperty heightMapScale;
    SerializedProperty heightMapImage;
    SerializedProperty perlinXScale;
    SerializedProperty perlinYScale;
    SerializedProperty perlinOffsetX;
    SerializedProperty perlinOffsetY;
    SerializedProperty perlinOctaves;
    SerializedProperty perlinPersistance;
    SerializedProperty perlinHeightScale;
    SerializedProperty resetTerrain;
    SerializedProperty voronoiFallOff;
    SerializedProperty voronoiDropOff;
    SerializedProperty voronoiMinHeight;
    SerializedProperty voronoiMaxHeight;
    SerializedProperty voronoiPeaks;
    SerializedProperty VornoiTypes;

    GUITableState perlinParameterTable;
    SerializedProperty perlinParameters;

    //MPD
    SerializedProperty MPDheightMin;
    SerializedProperty MPDhieghtmax;
    SerializedProperty MPDheightDampenerPower;
    SerializedProperty MPDroughness;

    SerializedProperty SmoothAmount;

    GUITableState splatMapTable;
    SerializedProperty splatHeights;

    SerializedProperty splatNoiseYscale;
    SerializedProperty splatNoiseXscale;
    SerializedProperty SplatOffset;
    SerializedProperty SplatScaler;


    // Fold outs 
    bool showRandom = false;
    bool showLoadHeights = false;
    bool showPerlinNoise = false;
    bool showMultiplePerlin = false;
    bool showVoronoi = false;
    bool showSplatMaps;
    bool showMPD = false;
    bool showSmooth = false;

    private void OnEnable() {
        randomHeightRange = serializedObject.FindProperty("randomHeightRange");
        heightMapScale = serializedObject.FindProperty("heightMapScale");
        heightMapImage = serializedObject.FindProperty("heightMapImage");
        perlinXScale = serializedObject.FindProperty("perlinXScale");
        perlinYScale = serializedObject.FindProperty("perlinYScale");
        perlinOffsetX = serializedObject.FindProperty("perlinOffsetX");
        perlinOffsetY = serializedObject.FindProperty("perlinOffsetY");
        perlinOctaves = serializedObject.FindProperty("perlinOctaves");
        perlinPersistance = serializedObject.FindProperty("perlinPersistance");
        perlinHeightScale = serializedObject.FindProperty("perlinHeightScale");
        resetTerrain = serializedObject.FindProperty("resetTerrain");
        perlinParameterTable = new GUITableState("perlinParameterTable");
        perlinParameters = serializedObject.FindProperty("perlinParameters");


        voronoiDropOff = serializedObject.FindProperty("voronoiDropOff");
        voronoiFallOff = serializedObject.FindProperty("voronoiFallOff");
        voronoiMinHeight =   serializedObject.FindProperty("voronoiMinHeight");
        voronoiMaxHeight = serializedObject.FindProperty("voronoiMaxHeight");
        voronoiPeaks = serializedObject.FindProperty("voronoiPeaks");
        VornoiTypes = serializedObject.FindProperty("vornoiType"); 

        MPDhieghtmax = serializedObject.FindProperty("MPDhieghtmax");
        MPDheightMin = serializedObject.FindProperty("MPDheightMin");
        MPDheightDampenerPower = serializedObject.FindProperty("MPDhieghtDampenerPower");
        MPDroughness = serializedObject.FindProperty("MPDRoughness");
        SmoothAmount = serializedObject.FindProperty("SmoothAmount");


        splatMapTable = new GUITableState("splatMapTable");
        splatHeights = serializedObject.FindProperty("splatHeights");
      //  SplatOffset = serializedObject.FindProperty("SplatOffset");
        //SplatScaler = serializedObject.FindProperty("SplatScaler");

    }

    Vector2 scrollPos;

    public override void OnInspectorGUI() {
        serializedObject.Update();

        CustomTerrain terrain = (CustomTerrain)target;

        Rect r = EditorGUILayout.BeginVertical();
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(r.width), GUILayout.Height(r.height));
        EditorGUI.indentLevel++;




        EditorGUILayout.PropertyField(resetTerrain);




        ShowRandom(terrain);
        ShowLoadHeights(terrain);
        ShowPerlinNoise(terrain);
        ShowMultiplePerlinNoise(terrain);
        ShowVornoi(terrain);
        ShowMPD(terrain);
        ShowSplatMap(terrain);
        ShowSmooth(terrain);
    
       
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        if(GUILayout.Button("Reset Terrain")) {
            terrain.ResetTerrain();
        }



        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();


        serializedObject.ApplyModifiedProperties();
    }

    public void ShowRandom(CustomTerrain terrain) {
        showRandom = EditorGUILayout.Foldout(showRandom, "Random");
        if (showRandom)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Set Heights between Random Values", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(randomHeightRange);
            if (GUILayout.Button("Random Heights"))
            {
                terrain.RandomTerrain();
            }
        }
    }

    public void ShowLoadHeights(CustomTerrain terrain) {
        showLoadHeights = EditorGUILayout.Foldout(showLoadHeights, "Load Heights");
        if (showLoadHeights)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Load Heights From Teture", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(heightMapImage);
            EditorGUILayout.PropertyField(heightMapScale);
            if (GUILayout.Button("Load Texture"))
            {
                terrain.LoadTexture();
            }
        }
    }

    public void ShowPerlinNoise(CustomTerrain terrain) {
        showPerlinNoise = EditorGUILayout.Foldout(showPerlinNoise, "Single Perlin Noise");
        if (showPerlinNoise)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Perlin Noise", EditorStyles.boldLabel);
            EditorGUILayout.Slider(perlinXScale, 0, 1, new GUIContent("X Scale"));
            EditorGUILayout.Slider(perlinYScale, 0, 1, new GUIContent("Y Scale"));
            EditorGUILayout.IntSlider(perlinOffsetX, 0, 10000, new GUIContent("Offset X"));
            EditorGUILayout.IntSlider(perlinOffsetY, 0, 10000, new GUIContent("Offset Y"));
            EditorGUILayout.IntSlider(perlinOctaves, 1, 10, new GUIContent("Octaves"));
            EditorGUILayout.Slider(perlinPersistance, 0.1f, 10, new GUIContent("Persistance"));
            EditorGUILayout.Slider(perlinHeightScale, 0, 1, new GUIContent("Height Scale"));

            if (GUILayout.Button("Perlin"))
            {
                terrain.Perlin();
            }
        }
    }

    public void ShowMultiplePerlinNoise(CustomTerrain terrain) {
        showMultiplePerlin = EditorGUILayout.Foldout(showMultiplePerlin, "Multiple Perlin Noise");
        if(showMultiplePerlin) {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
           
            GUILayout.Label("MultiplePerlin Noise", EditorStyles.boldLabel);


            perlinParameterTable = GUITableLayout.DrawTable(perlinParameterTable,  
                                                            serializedObject.FindProperty("perlinParameters"));




            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                terrain.AddNewPerlin();
            }
            if (GUILayout.Button("-"))
            {
                terrain.RemovePerlin();
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Apply Multiple Perlin"))
            {
                terrain.MultiplePerlinTerrain();
            }
        }
    }



    public void ShowVornoi(CustomTerrain terrian) {
        showVoronoi = EditorGUILayout.Foldout(showVoronoi, "Vornoi");
        if(showVoronoi) {
            EditorGUILayout.IntSlider(voronoiPeaks, 1,10, new GUIContent("Peak Count"));
            EditorGUILayout.Slider(voronoiFallOff,0,10 , new  GUIContent("FallOff"));
            EditorGUILayout.Slider(voronoiDropOff, 0, 10 ,  new GUIContent("DropOff"));
            EditorGUILayout.Slider(voronoiMinHeight, 0 , 1, new GUIContent("Min height"));
            EditorGUILayout.Slider(voronoiMaxHeight,0 , 1, new GUIContent("Max height"));
            EditorGUILayout.PropertyField(VornoiTypes);

            if(GUILayout.Button("Vornoi")) {
                terrian.Vornoi();
            }


        } else {



        }



    }

    public void ShowMPD(CustomTerrain terrian) {
        
        showMPD = EditorGUILayout.Foldout(showMPD, "Mid Point Displacement");
        if (showMPD)
        {
            EditorGUILayout.PropertyField(MPDheightMin);
           
            EditorGUILayout.PropertyField(MPDhieghtmax);
            EditorGUILayout.PropertyField(MPDheightDampenerPower);
            EditorGUILayout.PropertyField(MPDroughness);
            if (GUILayout.Button("MPD"))
            {
                terrian.MidPointDisplacement();

            }
        }

    }

    public void ShowSmooth(CustomTerrain terrian)
    {
        showSmooth = EditorGUILayout.Foldout(showSmooth, "Smooth Terrian");
        if (showSmooth)
        {
            EditorGUILayout.IntSlider(SmoothAmount, 1,10, new GUIContent("SmoothAmount"));
          
            if (GUILayout.Button("Smooth"))
            {
                terrian.Smooth();


            }

        }

    }

    public void ShowSplatMap(CustomTerrain terrian) {
        showSplatMaps = EditorGUILayout.Foldout(showSplatMaps, "Apply Splat Maps");
        if(showSplatMaps) {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            GUILayout.Label("Splat Maps", EditorStyles.boldLabel);
            splatMapTable = GUITableLayout.DrawTable(splatMapTable, serializedObject.FindProperty("splatHeights"));

            //EditorGUILayout.Slider(SplatOffset,0,0.1f, new GUIContent("Offset"));
            //EditorGUILayout.Slider(splatNoiseXscale,0.001f, 1, new GUIContent("Noise X Scale"));
            //EditorGUILayout.Slider(splatNoiseYscale, 0.001f, 1, new GUIContent("Noise Y Scale"));
            //EditorGUILayout.Slider(SplatScaler, 0.001f, 1, new GUIContent("Noise Y Scale"));
            GUILayout.Space(20);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(100);
            if(GUILayout.Button("+")) {
                terrian.AddNewSplatHeight();


            }
            if(GUILayout.Button("-")){
                terrian.RemoveSplathHeight();

            }
            GUILayout.Space(100);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Apply SplatMaps"))
            {
                terrian.SplatMap();

            }

        }


    }



}
